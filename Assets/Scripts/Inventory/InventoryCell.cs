using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //[HideInInspector]
    public Vector2Int cellCoordinates;
    public static Vector2Int cellPos;
    private Image image;
    private Color defaultColor;
    private static Color greenCellColor = new Color(0.7f, 1f, 0),
    redCellColor = new Color(1f, 0.23f, 0);


    private void Start()
    {

        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemDragManager.isItemHolder)
        {

            cellPos = cellCoordinates;




        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemDragManager.isItemHolder)
        {
        }
    }

    public void SetColor()
    {
        image.color = greenCellColor;
    }

    public void SetDefaultColor()
    {
        image.color = defaultColor;
    }
}