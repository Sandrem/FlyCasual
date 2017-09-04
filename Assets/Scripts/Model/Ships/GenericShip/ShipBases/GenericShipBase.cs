using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ship
{
    public enum BaseSize
    {
        Small,
        Large
    }

    public class GenericShipBase
    {
        public GenericShip Host { get; protected set; }
        public BaseSize Size { get; protected set; }
        public string PrefabPath { get; protected set; }
        public string TemporaryPrefabPath { get; protected set; }

        public GenericShipBase(GenericShip host)
        {
            Host = host;
        }

        protected virtual void CreateShipBase()
        {
            GameObject prefab = (GameObject)Resources.Load(PrefabPath, typeof(GameObject));
            GameObject shipBase = MonoBehaviour.Instantiate(
                prefab,
                Host.Model.transform.position,
                Host.Model.transform.rotation,
                Host.GetShipAllPartsTransform()
            );
            shipBase.transform.localEulerAngles = shipBase.transform.localEulerAngles + new Vector3(0, 180, 0);
            shipBase.transform.localPosition = Vector3.zero;
            shipBase.name = "ShipBase";
        }
    }
}
