using UnityEngine;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Bombs
{
    public class GenericDeviceGameObject : MonoBehaviour
    {
        protected DeviceObjectInfoPanel infoPanel;

        protected int fuses = 0;
        public int Fuses
        {
            get => fuses;
            set
            {
                var oldValue = fuses;
                var newValue = value;
                FusesChanging?.Invoke(this, oldValue, ref newValue);
                fuses = newValue;
            }
        }
        public delegate void DeviceValueChanging(GenericDeviceGameObject deviceGameObject, int oldValue, ref int newValue);
        public event DeviceValueChanging FusesChanging;
        public bool IsFused => Fuses > 0;

        public GameObject Model => gameObject;
        public GenericBomb ParentUpgrade { get; set; }

        public void Initialize(GenericBomb parentUpgrade, int fuses = 0)
        {
            ParentUpgrade = parentUpgrade;
            this.fuses = fuses;
            FusesChanging += DebugLog;
        }

        private void DebugLog(GenericDeviceGameObject deviceGameObject, int oldValue, ref int newValue)
        {
            Debug.Log($"{deviceGameObject.ParentUpgrade.UpgradeInfo.Name}: Fuses {oldValue}->{newValue}");
        }

        private void Start()
        {
            var infoPanelPrefab = Resources.Load<DeviceObjectInfoPanel>("Prefabs/Bombs/Helpers/DeviceInfoPanel");
            infoPanel = Instantiate(infoPanelPrefab, transform);
        }
    }
}