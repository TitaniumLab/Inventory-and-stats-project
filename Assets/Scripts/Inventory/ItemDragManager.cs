using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    public static ItemDragManager instance { get; private set; }//item that is carried
    private static ItemDragManager nextInstace;//the next item carried if another item is placed in its place

    public Vector2Int itemSize; //item size (cellsX x cellsY)
    public static Vector2Int currentItemSize; // size of carried item
    private Vector2Int cellOfItem; //coordinates of the upper left cell in which the item is stored

    private Vector2 dragPivot = new Vector2(0.5f, 0.5f), // pivot while carried
        inventoryPivot = new Vector2(0, 1); // pivot while in inventory

    private Image image; //image of item
    private Color defaultColor, onDragColor; //colors of item in inventory and of drag

    private Transform itemLayer;//storage layer
    private Transform dragLayer; //transfer layer

    private RectTransform itemRectTransform;//rect transform of item object

    private delegate void RaycastOnDrag(bool isRaycastTarget);
    private static event RaycastOnDrag OnMovingItem; //an event that makes all items not a raycast target

    private CharacterSlots currentSlot;//link to the equipment slot in which the item is located
    private static CharacterSlots[] characterSlots;//array of all equipment slot
    private EquipmentModifiers equipmentModifiers;

    private void Awake()
    {
        //set transparancy on drag
        image = GetComponent<Image>();
        defaultColor = image.color;
        onDragColor = defaultColor;
        onDragColor.a = 0.5f;

        itemRectTransform = GetComponent<RectTransform>();

        //find layers for storing and draging item
        Transform[] transforms = transform.parent.parent.GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms)
        {
            if (t.gameObject.name == "Item Layer")
                itemLayer = t;
            else if (t.gameObject.name == "Drag Layer")
                dragLayer = t;
        }

        OnMovingItem += IsRaycasting; //track raycast changes

        equipmentModifiers = GetComponent<EquipmentModifiers>();
        ////Get all equipment slots
        if (characterSlots == null)
            characterSlots = transform.parent.parent.GetComponentsInChildren<CharacterSlots>();
    }

    private void OnDestroy()
    {
        OnMovingItem -= IsRaycasting;//disable track raycast changes
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = new Color(0.7f, 0.7f, 0.7f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = defaultColor;
    }

    //allow move item on click
    public void OnPointerClick(PointerEventData eventData)
    {
        //start move item
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            StartMoveItem();
            OnMovingItem(false);
        }
        //set item in character equipment slot
        else if (eventData.button == PointerEventData.InputButton.Right && currentSlot == null)
        {
            foreach (CharacterSlots slot in characterSlots)
            {
                //find slot 
                if (slot.equipmentType == equipmentModifiers.equipmentType)
                {
                    //check if slot empty
                    if (slot.storedItem == null)
                    {
                        StartMoveItem();
                        SetItemInCells(slot.transform, dragPivot);
                    }
                    //if slot not empty -> swap
                    else if (slot.storedItem != null && currentSlot == null)
                    {
                        Vector2Int _pose = cellOfItem;
                        ItemDragManager swappedItem = slot.storedItem.GetComponent<ItemDragManager>();
                        StartMoveItem();
                        swappedItem.StartMoveItem();
                        swappedItem.SetItemInCells(itemLayer, inventoryPivot, _pose);
                        SetItemInCells(slot.transform, dragPivot);
                    }
                }
            }
        }
        //pull item from character equipment slot
        else if (eventData.button == PointerEventData.InputButton.Right && currentSlot != null)
        {
            Vector2Int _pose = FindPoseForItem();
            //if inventory not out of space 
            if (_pose != -Vector2Int.one)
            {
                StartMoveItem();
                SetItemInCells(itemLayer, inventoryPivot, _pose);
            }
        }

    }

    //allow move item on drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        StartMoveItem();
        OnMovingItem(false);
    }

    public void OnDrag(PointerEventData eventData) { }//functionality is implemented in the update function

    //drop item on end drag in cell
    public void OnEndDrag(PointerEventData eventData)
    {
        //if the cells are empty put the item in inventory
        if (instance == this && QuarterCell.itemsInCells.Count == 0 && QuarterCell.quarterCellInstance != null)
        {
            SetItemInCells(itemLayer, inventoryPivot, QuarterCell.totalItemPos);
            currentItemSize = Vector2Int.zero;
            OnMovingItem(true);
        }
        //if there is only 1 item present, switch places with it
        else if (instance == this && QuarterCell.itemsInCells != null &&
            QuarterCell.itemsInCells.Count == 1 &&
            QuarterCell.itemsInCells[0] != gameObject &&
            QuarterCell.quarterCellInstance != null)
        {
            QuarterCell.quarterCellInstance.CalcOffset();
            nextInstace = QuarterCell.itemsInCells[0].GetComponent<ItemDragManager>();
            SetItemInCells(itemLayer, inventoryPivot, QuarterCell.totalItemPos);
            instance = null;
        }
        //put in character equipment slots if its empty
        else if (instance == this && CharacterSlots.instance != null && CharacterSlots.instance.storedItem == null)
        {
            SetItemInCells(CharacterSlots.instance.transform, dragPivot);
            currentItemSize = Vector2Int.zero;
            OnMovingItem(true);
        }
        //put in character equipment slots if its empty
        else if (instance == this && CharacterSlots.instance != null && CharacterSlots.instance.storedItem == null &&
            equipmentModifiers.equipmentType == CharacterSlots.instance.equipmentType)
        {
            SetItemInCells(CharacterSlots.instance.transform, dragPivot);
            currentItemSize = Vector2Int.zero;
            OnMovingItem(true);

        }
        //swap item in equipment slot
        else if (instance == this && CharacterSlots.instance != null && CharacterSlots.instance.storedItem != null &&
            equipmentModifiers.equipmentType == CharacterSlots.instance.equipmentType)
        {
            nextInstace = CharacterSlots.instance.storedItem.GetComponent<ItemDragManager>();
            SetItemInCells(CharacterSlots.instance.transform, dragPivot);
            instance = null;
        }
    }

    private void Update()
    {
        //set item position on drag
        if (instance == this)
            transform.position = Input.mousePosition;

        //if the cells are empty put the item in inventory
        if (instance == this && Input.GetMouseButtonDown(0) && QuarterCell.quarterCellInstance != null && QuarterCell.itemsInCells.Count == 0)
        {
            SetItemInCells(itemLayer, inventoryPivot, QuarterCell.totalItemPos);
            currentItemSize = Vector2Int.zero;
            OnMovingItem(true);
        }
        //if there is only 1 item present, switch places with it
        else if (instance == this && Input.GetMouseButtonDown(0) && QuarterCell.quarterCellInstance != null && QuarterCell.itemsInCells != null &&
            QuarterCell.itemsInCells.Count == 1 && QuarterCell.itemsInCells[0] != gameObject)
        {
            QuarterCell.quarterCellInstance.CalcOffset();
            nextInstace = QuarterCell.itemsInCells[0].GetComponent<ItemDragManager>();
            SetItemInCells(itemLayer, inventoryPivot, QuarterCell.totalItemPos);
            instance = null;
        }
        //put in character equipment slots if its empty
        else if (instance == this && CharacterSlots.instance != null && Input.GetMouseButtonDown(0)
            && CharacterSlots.instance.storedItem == null && equipmentModifiers.equipmentType == CharacterSlots.instance.equipmentType)
        {
            SetItemInCells(CharacterSlots.instance.transform, dragPivot);
            currentItemSize = Vector2Int.zero;
            OnMovingItem(true);
        }
        //swap item in equipment slot
        else if (instance == this && CharacterSlots.instance != null && Input.GetMouseButtonDown(0)
            && CharacterSlots.instance.storedItem != null && equipmentModifiers.equipmentType == CharacterSlots.instance.equipmentType)
        {
            nextInstace = CharacterSlots.instance.storedItem.GetComponent<ItemDragManager>();
            SetItemInCells(CharacterSlots.instance.transform, dragPivot);
            instance = null;
        }
    }

    private void LateUpdate()
    {
        //start moving the item when replacing
        if (nextInstace != null)
        {
            nextInstace.StartMoveItem();
            nextInstace.transform.position = Input.mousePosition;//prevents visual glitch when replacing
            nextInstace = null;
            if (QuarterCell.quarterCellInstance != null)
                QuarterCell.quarterCellInstance.CalcOffset(); //allows to spam the replace
        }
    }

    /// <summary>
    /// start moving the item with the mouse
    /// </summary>
    public void StartMoveItem()
    {
        itemRectTransform.SetParent(dragLayer);
        itemRectTransform.pivot = dragPivot;
        instance = this;
        image.color = onDragColor;
        currentItemSize = itemSize;

        //clear cells information about item
        if (cellOfItem != -Vector2Int.one)
        {
            for (int y = cellOfItem.y; y < cellOfItem.y + itemSize.y; y++)
                for (int x = cellOfItem.x; x < cellOfItem.x + itemSize.x; x++)
                    if (InventoryManager.inventoryCellsGrid[x, y].storedItem == gameObject)
                        InventoryManager.inventoryCellsGrid[x, y].storedItem = null;
            cellOfItem = -Vector2Int.one;
        }
        else
        {
            if (currentSlot.storedItem == gameObject)
                currentSlot.storedItem = null;
            currentSlot = null;
        }


    }

    /// <summary>
    /// place the item in the specified inventory cell
    /// </summary>
    /// <param name="posInInventory">cell position in inventory</param>
    public void SetItemInCells(Transform parent, Vector2 pivot, Vector2Int posInInventory)
    {
        itemRectTransform.SetParent(parent);////
        itemRectTransform.pivot = pivot;///
        instance = null;
        image.color = defaultColor;



        itemRectTransform.anchoredPosition = InventoryManager.inventoryCellsGrid[posInInventory.x, posInInventory.y].
            gameObject.GetComponent<RectTransform>().anchoredPosition;

        cellOfItem = posInInventory;

        Debug.Log($"Item: {gameObject.name} added in: {cellOfItem} ");

        //set cells as occupied
        for (int y = posInInventory.y; y < posInInventory.y + itemSize.y; y++)
            for (int x = posInInventory.x; x < posInInventory.x + itemSize.x; x++)
            {
                InventoryCell cell = InventoryManager.inventoryCellsGrid[x, y];
                cell.storedItem = gameObject;
                cell.SetColorDefault();
            }

    }

    public void SetItemInCells(Transform parent, Vector2 pivot)
    {
        itemRectTransform.SetParent(parent);////
        itemRectTransform.pivot = pivot;///
        instance = null;
        image.color = defaultColor;

        itemRectTransform.localPosition = Vector2.zero; //parent.GetComponent<RectTransform>().anchoredPosition;

        currentSlot = parent.GetComponent<CharacterSlots>();
        if (CharacterSlots.instance != null)
            currentSlot.SetColor(CharacterSlots.instance.defaultColor);

        cellOfItem = -Vector2Int.one;
        CharacterSlots slot = parent.GetComponent<CharacterSlots>();
        slot.storedItem = gameObject;
        Debug.Log($"Item: {gameObject.name} added in: {parent.name} ");

    }
    /// <summary>
    /// enable/disable RaycastTarget
    /// </summary>
    /// <param name="isRaycastTarget">make raycast target?</param>
    private void IsRaycasting(bool isRaycastTarget)
    {
        image.raycastTarget = isRaycastTarget;
    }

    /// <summary>
    /// finds space in inventory based on item size
    /// </summary>
    /// <returns></returns>
    private Vector2Int FindPoseForItem()
    {
        for (int x = 0; x < InventoryManager.inventoryCellsGrid.GetLength(0); x++)
        {
            for (int y = 0; y < InventoryManager.inventoryCellsGrid.GetLength(1); y++)
            {
                //if cell empty check other cells by item size
                if (InventoryManager.inventoryCellsGrid[x, y].storedItem == null &&
                    (x + itemSize.x - 1) < InventoryManager.inventoryCellsGrid.GetLength(0) &&
                    (y + itemSize.y - 1) < InventoryManager.inventoryCellsGrid.GetLength(1))
                {
                    for (int localX = x; localX < x + itemSize.x; localX++)
                    {
                        for (int localY = y; localY < y + itemSize.y; localY++)
                        {
                            //if one of cells not empty start next iteration
                            if (InventoryManager.inventoryCellsGrid[localX, localY].storedItem != null)
                            {
                                localY = y + itemSize.y;
                                localX = x + itemSize.x;
                            }
                            else if (localY == y + itemSize.y - 1 &&
                                localX == x + itemSize.x - 1 &&
                                InventoryManager.inventoryCellsGrid[localX, localY].storedItem == null)
                            { return new Vector2Int(x, y); }
                        }
                    }
                }
            }
        }
        //inventory full mark
        return -Vector2Int.one;
    }


}
