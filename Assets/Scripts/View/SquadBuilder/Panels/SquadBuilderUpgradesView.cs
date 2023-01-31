using Editions;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Upgrade;

namespace SquadBuilderNS
{
    public class SquadBuilderUpgradesView
    {
        public void ShowAvailableUpgrades(UpgradeSlot slot)
        {
            UpgradePanelSquadBuilder.WaitingToLoad = 0;

            List<UpgradeRecord> filteredUpgrades = null;

            if (slot.Type != UpgradeType.Omni)
            {
                filteredUpgrades = Global.SquadBuilder.Database.AllUpgrades.Where(n =>
                     n.Instance.HasType(slot.Type)
                     && n.Instance.UpgradeInfo.Restrictions.IsAllowedForShip(Global.SquadBuilder.CurrentShip.Instance)
                     && n.Instance.IsAllowedForShip(Global.SquadBuilder.CurrentShip.Instance)
                     && n.Instance.HasEnoughSlotsInShip(Global.SquadBuilder.CurrentShip.Instance)
                     && Content.XWingFormats.IsLegalForFormat(n.Instance)
                     && ShipDoesntHaveUpgradeWithSameName(Global.SquadBuilder.CurrentShip.Instance, n.Instance)
                ).ToList();
            }
            else
            {
                filteredUpgrades = Global.SquadBuilder.Database.AllUpgrades;
            }

            int filteredUpgradesCount = filteredUpgrades.Count();

            //Clear search text
            GameObject.Find("UI/Panels/SelectUpgradePanel/TopPanel/InputField").GetComponent<InputField>().text = "";

            Transform contentTransform = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform;
            SquadBuilderView.DestroyChildren(contentTransform);

            if (filteredUpgradesCount > 0)
            {
                contentTransform.localPosition = new Vector3(0, contentTransform.localPosition.y, contentTransform.localPosition.z);
                contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(filteredUpgradesCount * (Edition.Current.UpgradeCardSize.x * 1.5f + SquadBuilderView.DISTANCE_MEDIUM) + 2 * SquadBuilderView.DISTANCE_MEDIUM, 0);

                filteredUpgrades = filteredUpgrades.OrderBy(n => n.Instance.UpgradeInfo.Cost).ToList();

                foreach (UpgradeRecord upgrade in filteredUpgrades)
                {
                    ShowAvailableUpgrade(upgrade);
                }

                GridLayoutGroup grid = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").GetComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(Edition.Current.UpgradeCardSize.x * 1.5f, Edition.Current.UpgradeCardSize.y * 1.5f);
            }
            else
            {
                SquadBuilderView.ShowNoContentInfo("Upgrade");
            }
        }

        private bool ShipDoesntHaveUpgradeWithSameName(GenericShip ship, GenericUpgrade upgrade)
        {
            return !ship.UpgradeBar.GetUpgradesAll().Any(n => n.UpgradeInfo.Name == upgrade.UpgradeInfo.Name);
        }

        public void FilterVisibleUpgrades(string text)
        {
            int upgradesCount = 0;
            foreach (Transform transform in GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform)
            {
                if (text == "")
                {
                    transform.gameObject.SetActive(true);
                    upgradesCount++;
                }
                else
                {
                    if (transform.gameObject.name.ToLower().Contains(text))
                    {
                        transform.gameObject.SetActive(true);
                        upgradesCount++;
                    }
                    else
                    {
                        transform.gameObject.SetActive(false);
                    }
                }
            }

            Transform contentTransform = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform;

            if (upgradesCount > 0)
            {
                contentTransform.localPosition = new Vector3(0, contentTransform.localPosition.y, contentTransform.localPosition.z);
                contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(upgradesCount * (Edition.Current.UpgradeCardSize.x * 1.5f + SquadBuilderView.DISTANCE_MEDIUM) + 2 * SquadBuilderView.DISTANCE_MEDIUM, 0);
            }
        }

        private void ShowAvailableUpgrade(UpgradeRecord upgrade)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/UpgradePanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform;
            GameObject newUpgradePanel = MonoBehaviour.Instantiate(prefab, contentTransform);

            string upgradeType = Global.SquadBuilder.Database.AllUpgrades.Find(n => n.UpgradeNameCanonical == upgrade.UpgradeNameCanonical && n.UpgradeType == upgrade.UpgradeType).UpgradeTypeName;
            GenericUpgrade newUpgrade = (GenericUpgrade)System.Activator.CreateInstance(Type.GetType(upgradeType));
            Edition.Current.AdaptUpgradeToRules(newUpgrade);

            UpgradePanelSquadBuilder script = newUpgradePanel.GetComponent<UpgradePanelSquadBuilder>();
            script.Initialize(upgrade.UpgradeName, Global.SquadBuilder.CurrentUpgradeSlot, newUpgrade, SelectUpgradeClicked, true);
            newUpgradePanel.name = upgrade.UpgradeName;
        }

        private void SelectUpgradeClicked(UpgradeSlot slot, GenericUpgrade upgrade)
        {
            Global.SquadBuilder.CurrentShip.InstallUpgrade(slot, upgrade);
            MainMenu.CurrentMainMenu.ChangePanel("ShipSlotsPanel");
        }
    }
}
