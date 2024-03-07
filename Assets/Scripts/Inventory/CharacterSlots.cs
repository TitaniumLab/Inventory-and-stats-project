using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSlots : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject storedItem;//the item that is in the cell
    public static CharacterSlots instance;
    private Image image;//image of cell
    public Color defaultColor;//default item color
    public static Color greenCellColor = new Color(0.7f, 1f, 0), redCellColor = new Color(1f, 0.23f, 0);//slot color on hover

    public EquipmentModifiers.EquipmentType equipmentType { get; private set; }
    [SerializeField] private EquipmentModifiers.EquipmentType _equipmentType; //determines what this slot is for

    private void Start()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;

        equipmentType = _equipmentType;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        //change color when hovering with an item
        if (ItemDragManager.instance != null)
        {
            instance = this;
            EquipmentModifiers equipmentModifiers = ItemDragManager.instance.GetComponent<EquipmentModifiers>();
            if (equipmentModifiers.equipmentType == this.equipmentType)
                SetColor(greenCellColor);
            else
                SetColor(redCellColor);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //change color back
        if (ItemDragManager.instance != null)
        {
            instance = null;
            SetColor(defaultColor);
        }
    }

    /// <summary>
    /// set slot color
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(Color color)
    {
        image.color = color;
    }
}
