using System.Collections.Generic;
using UnityEngine;

static partial class DamageNumbers
{
    public static void CreateDamageNumbersPanel(Ship.GenericShip host, int hullChange, int shieldsChange)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/DamageNumbers", typeof(GameObject));
        GameObject MessagePanelsHolder = GameObject.Find("UI/DamageNumbersHolder");
        GameObject Message = MonoBehaviour.Instantiate(prefab, MessagePanelsHolder.transform);
        Message.GetComponent<DamageNumbersPanel>().Initialize(host, hullChange, shieldsChange);
    }
}
