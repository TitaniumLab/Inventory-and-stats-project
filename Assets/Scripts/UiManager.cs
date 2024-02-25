using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject inventory;
    [SerializeField]
    private GameObject itemLayer;
    [SerializeField]
    private GameObject defaultItem;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    void Start()
    {
        //not moving on ui click
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();

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
        Transform cellTransform = InventoryManager.inventoryCellsGrid[0, 0].transform;
        GameObject item = Instantiate(defaultItem, itemLayer.transform);
        item.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            inventory.GetComponent<InventoryManager>().inventoryCellSize * item.GetComponent<ItemDragManager>().itemSize[0]);
        item.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
            inventory.GetComponent<InventoryManager>().inventoryCellSize * item.GetComponent<ItemDragManager>().itemSize[1]);
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
