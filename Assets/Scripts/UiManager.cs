using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject inventory;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    void Start()
    {
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            inventory.SetActive(false);

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
