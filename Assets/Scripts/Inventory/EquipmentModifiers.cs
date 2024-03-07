using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentModifiers : MonoBehaviour
{
    public enum EquipmentType { Helmet, BodyArmour, Weapon, Gloves, Boots }; // all available types of equipment
    public EquipmentType equipmentType { get; private set; } //defines the occupied slot
    [SerializeField] private EquipmentType _equipmentType;


}
