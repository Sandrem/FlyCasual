using Editions;
using ExtraOptions;
using ExtraOptions.ExtraOptionsList;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SquadBuilderNS
{
    public class SquadBuilderRosterView : MonoBehaviour
    {
        private List<ShipWithUpgradesPanelInfo> ShipWithUpgradesPanels { get; set; }
        private AddShipPanelInfo AddShipButtonPanel { get; set; }

        public void GenerateShipWithUpgradesPanels()
        {
            Cleanup();
            CreateShipWithUpgradesPanels();
            ShowAddShipPanel();
            OrganizeShipWithUpgradesPanels();
        }

        private void Cleanup()
        {
            ShipWithUpgradesPanels = new List<ShipWithUpgradesPanelInfo>();
            SquadBuilderView.DestroyChildren(GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/SquadListPanel").transform);
        }

        private void CreateShipWithUpgradesPanels()
        {
            PilotPanelSquadBuilder.WaitingToLoad = 0;
            foreach (SquadListShip ship in Global.SquadBuilder.CurrentSquad.Ships)
            {
                AddShipWithUpgradesPanel(ship);
            }
        }

        private void AddShipWithUpgradesPanel(SquadListShip ship)
        {
            ShipWithUpgradesPanelInfo shipWithUpgradesPanel = new ShipWithUpgradesPanelInfo(ship);
            ShipWithUpgradesPanels.Add(shipWithUpgradesPanel);
        }

        private void ShowAddShipPanel()
        {
            AddShipButtonPanel = (HasEnoughFreePointsToAddCheapestShip() || NoPointsLimit()) ? new AddShipPanelInfo() : null;
        }

        private static bool NoPointsLimit()
        {
            return ExtraOptionsManager.ExtraOptions[typeof(NoSquadBuilderLimitsExtraOption)].IsOn;
        }

        private static bool HasEnoughFreePointsToAddCheapestShip()
        {
            return Global.SquadBuilder.CurrentSquad.Points <= Edition.Current.MaxPoints - Edition.Current.MinShipCost(Global.SquadBuilder.CurrentSquad.SquadFaction);
        }

        private void OrganizeShipWithUpgradesPanels()
        {
            float totalAlailableWidth = GameObject.Find("UI").GetComponent<RectTransform>().sizeDelta.x;
            float alailableWidth = totalAlailableWidth - 2 * SquadBuilderView.DISTANCE_MEDIUM;

            float allPanelsWidth = 0;

            foreach (ShipWithUpgradesPanelInfo panel in ShipWithUpgradesPanels)
            {
                allPanelsWidth = allPanelsWidth + panel.Size.x;
            }
            if (ShipWithUpgradesPanels.Count > 1)
            {
                allPanelsWidth += SquadBuilderView.DISTANCE_LARGE * (ShipWithUpgradesPanels.Count - 1);
            }

            if (AddShipButtonPanel != null)
            {
                allPanelsWidth += AddShipButtonPanel.Size.x;
                if (ShipWithUpgradesPanels.Count > 0)
                {
                    allPanelsWidth += SquadBuilderView.DISTANCE_LARGE;
                }
            }

            if (alailableWidth > allPanelsWidth)
            {
                ArrangeShipsWithUpgradesInOneLine(allPanelsWidth);
            }
            else
            {
                ArrangeShipsWithUpgradesInTwoLines(allPanelsWidth);
            }

        }

        private void ArrangeShipsWithUpgradesInOneLine(float allPanelsWidth)
        {
            float offset = SquadBuilderView.DISTANCE_MEDIUM;

            GameObject rosterHolder = GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/SquadListPanel");
            rosterHolder.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            foreach (ShipWithUpgradesPanelInfo panel in ShipWithUpgradesPanels)
            {
                panel.Panel.transform.localPosition = new Vector2(offset, -SquadBuilderView.DISTANCE_MEDIUM);
                offset += panel.Size.x + SquadBuilderView.DISTANCE_LARGE;
            }

            if (AddShipButtonPanel != null)
            {
                AddShipButtonPanel.Panel.transform.localPosition = new Vector2(offset, -SquadBuilderView.DISTANCE_MEDIUM);
            }

            rosterHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(allPanelsWidth + 2 * SquadBuilderView.DISTANCE_MEDIUM, SquadBuilderView.PILOT_CARD_HEIGHT + 2 * SquadBuilderView.DISTANCE_MEDIUM);
            MainMenu.ScalePanel(rosterHolder.transform, maxScale: 1.5f, twoBorders: true);
        }

        private void ArrangeShipsWithUpgradesInTwoLines(float allPanelsWidth)
        {
            float offset = SquadBuilderView.DISTANCE_MEDIUM;

            Dictionary<ShipWithUpgradesPanelInfo, int> panelsByRow = GetArrangeShipsWithUpgradesIntoRowNumbers();
            float row1width = panelsByRow.Where(n => n.Value == 1).Sum(m => m.Key.Size.x) + SquadBuilderView.DISTANCE_LARGE * (panelsByRow.Count(n => n.Value == 1) - 1);
            float row2width = panelsByRow.Where(n => n.Value == 2).Sum(m => m.Key.Size.x) + SquadBuilderView.DISTANCE_LARGE * (panelsByRow.Count(n => n.Value == 2) - 1);
            if (AddShipButtonPanel != null) row2width += AddShipButtonPanel.Size.x + SquadBuilderView.DISTANCE_LARGE;
            float maxWidth = Mathf.Max(row1width, row2width);

            GameObject rosterViewPanel = GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/SquadListPanel");
            rosterViewPanel.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            offset = SquadBuilderView.DISTANCE_MEDIUM + ((maxWidth - row1width) / 2);
            foreach (var panel in panelsByRow.Where(n => n.Value == 1))
            {
                panel.Key.Panel.transform.localPosition = new Vector2(offset, -SquadBuilderView.DISTANCE_MEDIUM);
                offset += panel.Key.Size.x + SquadBuilderView.DISTANCE_LARGE;
            }

            offset = SquadBuilderView.DISTANCE_MEDIUM + ((maxWidth - row2width) / 2);
            foreach (var panel in panelsByRow.Where(n => n.Value == 2))
            {
                panel.Key.Panel.transform.localPosition = new Vector2(offset, -(SquadBuilderView.PILOT_CARD_HEIGHT + 2 * SquadBuilderView.DISTANCE_MEDIUM));
                offset += panel.Key.Size.x + SquadBuilderView.DISTANCE_LARGE;
            }

            if (AddShipButtonPanel != null)
            {
                AddShipButtonPanel.Panel.transform.localPosition = new Vector2(offset, -(SquadBuilderView.PILOT_CARD_HEIGHT + 2 * SquadBuilderView.DISTANCE_MEDIUM));
            }

            rosterViewPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth + 2 * SquadBuilderView.DISTANCE_MEDIUM, 2 * SquadBuilderView.PILOT_CARD_HEIGHT + 3 * SquadBuilderView.DISTANCE_MEDIUM);
            MainMenu.ScalePanel(rosterViewPanel.transform, maxScale: 1.25f, twoBorders: true);
        }

        private Dictionary<ShipWithUpgradesPanelInfo, int> GetArrangeShipsWithUpgradesIntoRowNumbers()
        {
            Dictionary<ShipWithUpgradesPanelInfo, int> result = new Dictionary<ShipWithUpgradesPanelInfo, int>();

            bool isAddShipPanelVisible = AddShipButtonPanel != null;

            ShipWithUpgradesPanelInfo maxSizePanel = ShipWithUpgradesPanels.Find(n => n.Size.x == ShipWithUpgradesPanels.Max(m => m.Size.x));
            result.Add(maxSizePanel, 1);

            float fullSingleRowWidth = ShipWithUpgradesPanels.Sum(n => n.Size.x);
            if (isAddShipPanelVisible) fullSingleRowWidth += AddShipButtonPanel.Size.x;
            float difference = Mathf.Abs(fullSingleRowWidth - maxSizePanel.Size.x - maxSizePanel.Size.x);

            bool finished = false;
            while (!finished)
            {
                List<ShipWithUpgradesPanelInfo> shipPanelsNotProcessed = ShipWithUpgradesPanels.Where(n => !result.ContainsKey(n)).ToList();
                ShipWithUpgradesPanelInfo minSizePanel = shipPanelsNotProcessed.Find(n => n.Size.x == shipPanelsNotProcessed.Min(m => m.Size.x));

                if (minSizePanel == null) return result;

                float firstRowWidth = result.Where(n => n.Value == 1).Sum(n => n.Key.Size.x) + minSizePanel.Size.x;
                float secondRowWidth = fullSingleRowWidth - result.Where(n => n.Value == 1).Sum(n => n.Key.Size.x) - minSizePanel.Size.x;
                float newDifference = Mathf.Abs(firstRowWidth - secondRowWidth);

                if (newDifference > difference)
                {
                    foreach (ShipWithUpgradesPanelInfo panel in shipPanelsNotProcessed)
                    {
                        if (!result.ContainsKey(panel)) result.Add(panel, 2);
                    }
                    finished = true;
                }
                else
                {
                    result.Add(minSizePanel, 1);
                    difference = newDifference;
                }
            }

            return result;
        }
    }
}
