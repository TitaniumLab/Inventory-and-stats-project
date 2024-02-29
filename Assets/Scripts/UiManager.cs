using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject inventory;
    private InventoryManager inventoryManager;
    [SerializeField]
    private GameObject itemLayer;
    [SerializeField]
    private GameObject defaultItem;
    private int itemCounter;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    void Start()
    {
        //not moving on ui click
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
        inventoryManager = inventory.GetComponent<InventoryManager>();
        itemCounter = 0;
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            inventory.SetActive(false);
    }

    /// <summary>
    /// Get some item in inventory
    /// </summary>
    public void GetItem()
    {
        GameObject item = Instantiate(defaultItem, itemLayer.transform);
        ItemDragManager itemDragManager = item.GetComponent<ItemDragManager>();
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();

        //-----------
        item.name += itemCounter;
        itemCounter++;

        //set item size (pixel)
        itemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryManager.inventoryCellSize * itemDragManager.itemSize[0]);
        itemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryManager.inventoryCellSize * itemDragManager.itemSize[1]);

        Vector2Int pose = FindPoseForItem(itemDragManager);
        if (pose == -Vector2Int.one)
            Destroy(item);
        else
            itemDragManager.SetItemInCells(pose);
    }

    private Vector2Int FindPoseForItem(ItemDragManager item)
    {
        for (int x = 0; x < InventoryManager.inventoryCellsGrid.GetLength(0); x++)
        {
            for (int y = 0; y < InventoryManager.inventoryCellsGrid.GetLength(1); y++)
            {
                //if cell empty check other cells by item size
                if (InventoryManager.inventoryCellsGrid[x, y].storedItem == null &&
                    (x + item.itemSize.x - 1) < InventoryManager.inventoryCellsGrid.GetLength(0) &&
                    (y + item.itemSize.y - 1) < InventoryManager.inventoryCellsGrid.GetLength(1))
                {
                    for (int localX = x; localX < x + item.itemSize.x; localX++)
                    {
                        for (int localY = y; localY < y + item.itemSize.y; localY++)
                        {
                            //if one of cells not empty start next iteration
                            if (InventoryManager.inventoryCellsGrid[localX, localY].storedItem != null)
                            {
                                localY = y + item.itemSize.y;
                                localX = x + item.itemSize.x;
                            }
                            else if (localY == y + item.itemSize.y - 1 &&
                                localX == x + item.itemSize.x - 1 &&
                                InventoryManager.inventoryCellsGrid[localX, localY].storedItem == null)
                            { return new Vector2Int(x, y); }
                        }
                    }
                }
            }
        }
        return -Vector2Int.one;
    }

    /// <summary>
    /// check if cursor on ui
    /// </summary>
    /// <returns></returns>
    public bool IsCursorOnUI()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        m_Raycaster.Raycast(m_PointerEventData, results);

        if (results.Count > 0)
            return true;
        else
            return false;
    }


    /// <summary>
    /// allow inventory button open and close inventory
    /// </summary>
    public void OpenCloseInventory()
    {
        if (inventory.activeInHierarchy)
            inventory.SetActive(false);
        else
            inventory.SetActive(true);
    }
}
