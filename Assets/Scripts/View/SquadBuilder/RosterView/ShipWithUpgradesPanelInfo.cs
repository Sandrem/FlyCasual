using Editions;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace SquadBuilderNS
{
    public class ShipWithUpgradesPanelInfo : IRosterViewPanelInfo
    {
        private const float PILOT_CARD_WIDTH = 300;
        private const float PILOT_CARD_STANDARDLAYOUT_WIDTH = 722;
        private const float PILOT_CARD_HEIGHT = 418;

        public GameObject Panel;
        public Vector2 Size = new Vector2(PILOT_CARD_WIDTH, PILOT_CARD_HEIGHT);
        public SquadListShip Ship;
        private bool HasConfigurationInstalled = false;

        private int AvailableUpgradesCounter { get; set; }
        private GameObject PilotPanel { get; set; }

        public ShipWithUpgradesPanelInfo(SquadListShip ship)
        {
            Ship = ship;

            CreateHolderPanel(ship);
            ShowPilot(ship);
            ShowUpgradesOfPilot(ship);
            SetSize();
        }

        private void CreateHolderPanel(SquadListShip ship)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipWithUpgradesPanel", typeof(GameObject));
            Panel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels").transform.Find("SquadBuilderPanel").Find("Panel").Find("SquadListPanel").transform);

            RectTransform contentRect = Panel.transform.GetComponent<RectTransform>();
            if (!(Ship.Instance.PilotInfo as PilotCardInfo25).IsStandardLayout)
            {
                int installedUpgradesCount = ship.Instance.UpgradeBar.GetUpgradesAll().Count;
                contentRect.sizeDelta = new Vector2(SquadBuilderView.PILOT_CARD_WIDTH + (Edition.Current.UpgradeCardCompactSize.x + SquadBuilderView.DISTANCE_SMALL) * installedUpgradesCount, contentRect.sizeDelta.y);
            }
            else
            {
                contentRect.sizeDelta = new Vector2(PILOT_CARD_STANDARDLAYOUT_WIDTH, contentRect.sizeDelta.y);
            }
        }

        private void ShowPilot(SquadListShip ship)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            PilotPanel = MonoBehaviour.Instantiate(prefab, Panel.transform);
            PilotPanel.transform.localPosition = Vector3.zero;

            if ((Ship.Instance.PilotInfo as PilotCardInfo25).IsStandardLayout)
            {
                PilotPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(PILOT_CARD_STANDARDLAYOUT_WIDTH, PILOT_CARD_HEIGHT);
            }

            PilotPanelSquadBuilder script = PilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.Initialize(ship.Instance, Global.SquadBuilder.View.OpenShipInfo);
        }

        private void ShowUpgradesOfPilot(SquadListShip ship)
        {
            if (!(Ship.Instance.PilotInfo as PilotCardInfo25).IsStandardLayout)
            {
                AvailableUpgradesCounter = 0;
                UpgradePanelSquadBuilder.WaitingToLoad = 0;

                List<GenericUpgrade> shipUpgrades = ship.Instance.UpgradeBar.GetUpgradesAll().OrderBy(s => s.UpgradeInfo.UpgradeTypes[0]).ToList();

                if (shipUpgrades.Any(n => n.HasType(UpgradeType.Configuration)))
                {
                    HasConfigurationInstalled = true;
                    ShowConfigurationUpgrade();
                }

                foreach (GenericUpgrade upgrade in shipUpgrades)
                {
                    if (!upgrade.HasType(UpgradeType.Configuration)) ShowUpgradeOfPilot(upgrade, ship);
                }
            }
        }

        private void ShowConfigurationUpgrade()
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/UpgradePanel", typeof(GameObject));
            GameObject newUpgradePanel = MonoBehaviour.Instantiate(prefab, Panel.transform);

            newUpgradePanel.transform.localPosition = new Vector2(0, 0);
            PilotPanel.transform.localPosition = new Vector2(Edition.Current.UpgradeCardCompactSize.x, 0);

            GenericUpgrade upgrade = Ship.Instance.UpgradeBar.GetInstalledUpgrade(UpgradeType.Configuration);
            UpgradePanelSquadBuilder script = newUpgradePanel.GetComponent<UpgradePanelSquadBuilder>();
            script.Initialize(upgrade.UpgradeInfo.Name, null, upgrade, compact: true);
        }

        private void ShowUpgradeOfPilot(GenericUpgrade upgrade, SquadListShip ship)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/UpgradePanel", typeof(GameObject));
            GameObject newUpgradePanel = MonoBehaviour.Instantiate(prefab, Panel.transform);

            float configurationOffset = (HasConfigurationInstalled) ? Edition.Current.UpgradeCardCompactSize.x : 0;
            newUpgradePanel.transform.localPosition = new Vector2(
                configurationOffset + SquadBuilderView.PILOT_CARD_WIDTH + (Edition.Current.UpgradeCardCompactSize.x * AvailableUpgradesCounter), 
                0
            );

            UpgradePanelSquadBuilder script = newUpgradePanel.GetComponent<UpgradePanelSquadBuilder>();
            script.Initialize(upgrade.UpgradeInfo.Name, null, upgrade, compact: true);

            AvailableUpgradesCounter++;
        }

        private void SetSize()
        {
            float configurationOffset = (HasConfigurationInstalled) ? Edition.Current.UpgradeCardCompactSize.x : 0;

            if (!(Ship.Instance.PilotInfo as PilotCardInfo25).IsStandardLayout)
            {
                Size = new Vector2
                (
                    configurationOffset + SquadBuilderView.PILOT_CARD_WIDTH + (Edition.Current.UpgradeCardCompactSize.x * AvailableUpgradesCounter),
                    SquadBuilderView.PILOT_CARD_HEIGHT
                );
            }
            else
            {
                Size = new Vector2
                (
                    PILOT_CARD_STANDARDLAYOUT_WIDTH,
                    PILOT_CARD_HEIGHT
                );
            }
            
            Panel.GetComponent<RectTransform>().sizeDelta = Size;
        }
    }
}
