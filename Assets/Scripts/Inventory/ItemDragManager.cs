using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool isItemHolder = false;
    public Vector2Int itemSize;
    public static Vector2Int currentItemSize;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        GetComponent<RectTransform>().pivot = new Vector2(0, 1);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        currentItemSize = itemSize;
        isItemHolder = true;
        image.raycastTarget = false;
        GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

        Color color = image.color;
        color.a = 0.5f;
        image.color = color;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //QuarterCell.Instance.SetDefaultCellColor();
        currentItemSize = Vector2Int.zero;
        isItemHolder = false;
        image.raycastTarget = true;
        GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        //transform.position = QuarterCell.parentCoordinates;
        int totalOffsetX = InventoryCell.cellPos.x - ((itemSize.x - QuarterCell.quarterOffset.x) % 2 == 0 ? 0 : 1);
        int totalOffsetY = InventoryCell.cellPos.y - ((itemSize.y - QuarterCell.quarterOffset.y) % 2 == 0 ? 0 : 1);
        GetComponent<RectTransform>().anchoredPosition = InventoryManager.inventoryCellsGrid[totalOffsetX, totalOffsetY].GetComponent<RectTransform>().anchoredPosition;
        Color color = image.color;
        color.a = 1f;
        image.color = color;
    }

}
