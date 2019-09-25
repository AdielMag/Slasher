using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItem : MonoBehaviour
{
    public enum itemType { Character, Weapon }
    public itemType type;

    public int price;
    public bool bought;
    public bool equipped;
}
