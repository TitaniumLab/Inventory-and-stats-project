using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuarterCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static QuarterCell quarterCellInstance;//active quarter cell
    [SerializeField] private Vector2Int quarterOffset; //contains information about where the item should move
    [SerializeField] private InventoryCell parent; //link to parent cell
    public static Vector2Int totalItemPos; //contains the final position of the item
    public static List<GameObject> itemsInCells; //items under the carried item


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemDragManager.instance != null)
        {
            quarterCellInstance = this;
            CalcOffset();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        quarterCellInstance = null;
    }

    public void CalcOffset()
    {
        //get X pos
        int _itemSizeX = ItemDragManager.currentItemSize.x;
        int halfOffsetX = _itemSizeX % 2 == 0 ? _itemSizeX / 2 + quarterOffset.x : (_itemSizeX - 1) / 2;
        totalItemPos.x = parent.cellCoordinates.x - halfOffsetX;

        //get Y pos
        int _itemSizeY = ItemDragManager.currentItemSize.y;
        int halfOffsetY = _itemSizeY % 2 == 0 ? _itemSizeY / 2 + quarterOffset.y : (_itemSizeY - 1) / 2;
        totalItemPos.y = parent.cellCoordinates.y - halfOffsetY;

        int _inventorySizeX = InventoryManager.inventoryCellsGrid.GetLength(0);
        int _inventorySizeY = InventoryManager.inventoryCellsGrid.GetLength(1);

        itemsInCells = new List<GameObject>();

        //preventing array boundaries from leaving
        if (totalItemPos.x < 0)
            totalItemPos.x = 0;
        else if (totalItemPos.x + _itemSizeX > _inventorySizeX)
            totalItemPos.x = _inventorySizeX - _itemSizeX;

        //preventing array boundaries from leaving
        if (totalItemPos.y < 0)
            totalItemPos.y = 0;
        else if (totalItemPos.y + ItemDragManager.currentItemSize.y > _inventorySizeY)
            totalItemPos.y = _inventorySizeY - ItemDragManager.currentItemSize.y;

        //reset inventory color
        for (int x = 0; x < _inventorySizeX; x++)
        {
            for (int y = 0; y < _inventorySizeY; y++)
            {
                InventoryManager.inventoryCellsGrid[x, y].SetColorDefault();
            }
        }

        //check other items under carried item
        for (int x = totalItemPos.x; x < totalItemPos.x + ItemDragManager.currentItemSize.x; x++)
        {
            for (int y = totalItemPos.y; y < totalItemPos.y + ItemDragManager.currentItemSize.y; y++)
            {
                InventoryCell cell = InventoryManager.inventoryCellsGrid[x, y];
                if (cell.storedItem != null && !itemsInCells.Contains(cell.storedItem))
                    itemsInCells.Add(cell.storedItem);
            }
        }

        //set cell color
        for (int x = totalItemPos.x; x < totalItemPos.x + ItemDragManager.currentItemSize.x; x++)
        {
            for (int y = totalItemPos.y; y < totalItemPos.y + ItemDragManager.currentItemSize.y; y++)
            {
                InventoryCell cell = InventoryManager.inventoryCellsGrid[x, y];
                //if <2 items under carried item set green color
                if (itemsInCells.Count < 2)
                    cell.SetColorGreen();
                //if 2 or more set color red
                else
                    cell.SetColorRed();

            }
        }
        Debug.Log($"Items Under dragged item: {itemsInCells.Count} Position: {totalItemPos}");
    }
}
