using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuarterCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //a quarter of the cell is required for correct offset
    //public static QuarterCell Instance;
    public static Vector2Int quarterOffset;
    //public static Vector2 parentCoordinates;
    public GameObject parent;
    [SerializeField]
    private int xOffset;
    [SerializeField]
    private int yOffset;
    int totalOffsetX;
    int totalOffsetY;

    private void Start()
    {
        //quarterOffset = new Vector2Int(xOffset, yOffset);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        quarterOffset = new Vector2Int(xOffset, yOffset);
        if (ItemDragManager.isItemHolder)
        {
            //totalOffsetX = parent.GetComponent<InventoryCell>().cellCoordinates.x + (ItemDragManager.currentItemSize.x % 2 == 0 ? quarterOffset.x : 0);
            //totalOffsetY = parent.GetComponent<InventoryCell>().cellCoordinates.y + (ItemDragManager.currentItemSize.y % 2 == 0 ? quarterOffset.y : 0);
            totalOffsetX = parent.GetComponent<InventoryCell>().cellCoordinates.x -
                (ItemDragManager.currentItemSize.x % 2 == 0 ? ItemDragManager.currentItemSize.x / 2 + quarterOffset.x : (ItemDragManager.currentItemSize.x - 1) / 2);
            totalOffsetY = parent.GetComponent<InventoryCell>().cellCoordinates.y -
                (ItemDragManager.currentItemSize.y % 2 == 0 ? ItemDragManager.currentItemSize.y / 2 + quarterOffset.y : (ItemDragManager.currentItemSize.y - 1) / 2);
            for (int x = totalOffsetX; x < totalOffsetX + ItemDragManager.currentItemSize.x; x++)
            {
                for (int y = totalOffsetY; y < totalOffsetY + ItemDragManager.currentItemSize.y; y++)
                {
                    InventoryCell inventoryCell = InventoryManager.inventoryCellsGrid[x, y].GetComponent<InventoryCell>();
                    inventoryCell.SetColor();
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemDragManager.isItemHolder)
        {
            for (int x = totalOffsetX; x < totalOffsetX + ItemDragManager.currentItemSize.x; x++)
            {
                for (int y = totalOffsetY; y < totalOffsetY + ItemDragManager.currentItemSize.y; y++)
                {
                    InventoryCell inventoryCell = InventoryManager.inventoryCellsGrid[x, y].GetComponent<InventoryCell>();
                    inventoryCell.SetDefaultColor();
                }
            }
        }
        //quarterOffset = Vector2Int.zero;
    }
}
