using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
    public Vector2Int cellCoordinates;//cell coordinates in the array
    private Image image;//image of cell
    public GameObject storedItem;//the item that is in the cell
    private Color defaultColor;//default item color
    private static Color greenCellColor = new Color(0.7f, 1f, 0), redCellColor = new Color(1f, 0.23f, 0);//item colors on moving


    private void Start()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    public void SetColorGreen()
    {
        image.color = greenCellColor;
    }

    public void SetColorDefault()
    {
        image.color = defaultColor;
    }

    public void SetColorRed()
    {
        image.color = redCellColor;
    }
}