using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public static ItemDragManager instance;//item that is carried
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
    }

    private void OnDestroy()
    {
        OnMovingItem -= IsRaycasting;//disable track raycast changes
    }

    //allow move item on click
    public void OnPointerClick(PointerEventData eventData)
    {
        StartMoveItem();
        OnMovingItem(false);
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
        if (instance == this && QuarterCell.itemsInCells.Count == 0)
        {
            SetItemInCells(QuarterCell.totalItemPos);
            currentItemSize = Vector2Int.zero;
            OnMovingItem(true);
        }
        //if there is only 1 item present, switch places with it
        else if (instance == this && QuarterCell.itemsInCells != null &&
            QuarterCell.itemsInCells.Count == 1 &&
            QuarterCell.itemsInCells[0] != gameObject)
        {
            QuarterCell.quarterCellInstance.CalcOffset();
            nextInstace = QuarterCell.itemsInCells[0].GetComponent<ItemDragManager>();
            SetItemInCells(QuarterCell.totalItemPos);
            instance = null;
        }
    }

    private void Update()
    {
        //set item position on drag
        if (instance == this)
            transform.position = Input.mousePosition;

        //if the cells are empty put the item in inventory
        if (instance == this && Input.GetMouseButtonDown(0) && QuarterCell.itemsInCells.Count == 0)
        {
            SetItemInCells(QuarterCell.totalItemPos);
            currentItemSize = Vector2Int.zero;
            OnMovingItem(true);
        }
        //if there is only 1 item present, switch places with it
        if (instance == this && Input.GetMouseButtonDown(0) &&
            QuarterCell.itemsInCells != null &&
            QuarterCell.itemsInCells.Count == 1 &&
            QuarterCell.itemsInCells[0] != gameObject)
        {
            QuarterCell.quarterCellInstance.CalcOffset();
            nextInstace = QuarterCell.itemsInCells[0].GetComponent<ItemDragManager>();
            SetItemInCells(QuarterCell.totalItemPos);
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
            QuarterCell.quarterCellInstance.CalcOffset(); //allows to spam the replace
            Debug.Log("поменяли местами");
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
        for (int y = cellOfItem.y; y < cellOfItem.y + itemSize.y; y++)
            for (int x = cellOfItem.x; x < cellOfItem.x + itemSize.x; x++)
                if (InventoryManager.inventoryCellsGrid[x, y].storedItem == gameObject)
                    InventoryManager.inventoryCellsGrid[x, y].storedItem = null;
    }

    /// <summary>
    /// place the item in the specified inventory cell
    /// </summary>
    /// <param name="posInInventory">cell position in inventory</param>
    public void SetItemInCells(Vector2Int posInInventory)
    {
        itemRectTransform.SetParent(itemLayer);
        itemRectTransform.pivot = inventoryPivot;
        instance = null;
        itemRectTransform.anchoredPosition = InventoryManager.inventoryCellsGrid[posInInventory.x, posInInventory.y].
            gameObject.GetComponent<RectTransform>().anchoredPosition;

        image.color = defaultColor;

        cellOfItem = posInInventory;

        Debug.Log($"Item: {gameObject.name} added in: {cellOfItem} ");
        //set cells as occupied
        for (int y = posInInventory.y; y < posInInventory.y + itemSize.y; y++)
        {
            for (int x = posInInventory.x; x < posInInventory.x + itemSize.x; x++)
            {
                InventoryManager.inventoryCellsGrid[x, y].storedItem = gameObject;
            }
        }
    }

    /// <summary>
    /// enable/disable RaycastTarget
    /// </summary>
    /// <param name="isRaycastTarget">make raycast target?</param>
    private void IsRaycasting(bool isRaycastTarget)
    {
        image.raycastTarget = isRaycastTarget;
    }
}
