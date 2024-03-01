using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public float inventoryCellSize;
    [SerializeField] private Vector2Int inventoryWindowGrid;
    public static InventoryCell[,] inventoryCellsGrid;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private RectTransform inventoryStorage;
    [SerializeField] private RectTransform itemLayer;
    [SerializeField] private RectTransform characterSlots;
    //private RectTransform canvasRectTransform;

    private void Awake()
    {
        //set inventory size
        float inventoryWidth = inventoryCellSize * inventoryWindowGrid.x;
        float inventoryHight = inventoryCellSize * inventoryWindowGrid.y;
        //canvasRectTransform = GetComponentInParent<RectTransform>();

        RectTransform inventoryRect = GetComponent<RectTransform>();
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidth);
        //inventoryStorage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidth);
        inventoryStorage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHight);
        Debug.Log($"{inventoryStorage.sizeDelta.y}");
        characterSlots.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryRect.rect.height - inventoryHight);
        //Debug.Log($"{canvasRectTransform.sizeDelta.y}");
        itemLayer.sizeDelta = inventoryStorage.sizeDelta;
        //inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHight);

        //Create cells
        GridLayoutGroup gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        gridLayoutGroup.cellSize = new Vector2(inventoryCellSize, inventoryCellSize);
        inventoryCellsGrid = new InventoryCell[inventoryWindowGrid.x, inventoryWindowGrid.y];
        for (int y = 0; y < inventoryWindowGrid.y; y++)
        {
            for (int x = 0; x < inventoryWindowGrid.x; x++)
            {
                inventoryCellsGrid[x, y] = Instantiate(cellPrefab, gridLayoutGroup.gameObject.transform).GetComponent<InventoryCell>();
                inventoryCellsGrid[x, y].cellCoordinates = new Vector2Int(x, y);
                inventoryCellsGrid[x, y].name += $" ({x};{y})";
                //set quarter of cell size
                inventoryCellsGrid[x, y].gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(inventoryCellSize / 2, inventoryCellSize / 2); ;
            }
        }
    }
}
