using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SquadBuilderNS
{
    static partial class SquadBuilder
    {
        private class ShipWithUpgradesPanel
        {
            public GameObject Panel;
            public Vector2 Size;
            public SquadBuilderShip Ship;

            public ShipWithUpgradesPanel(SquadBuilderShip ship, GameObject panel)
            {
                Ship = ship;
                Panel = panel;
                Size = new Vector2(300, 418);
            }
        }

        private static int availableShipsCounter;
        private static int availablePilotsCounter;

        private static List<ShipWithUpgradesPanel> ShipWithUpgradesPanels;
        private static ShipWithUpgradesPanel AddShipButtonPanel;

        private static void ShowAvailableShips(Faction faction)
        {
            DeleteOldShips();
            availableShipsCounter = 0;

            foreach (ShipRecord ship in AllShips)
            {
                if (ship.Instance.factions.Contains(faction))
                {
                    ShowAvailableShip(ship);
                }
            }
        }

        private static void DeleteOldShips()
        {
            foreach (Transform oldShipPanel in GameObject.Find("UI/Panels/SelectShipPanel/Panel").transform)
            {
                GameObject.Destroy(oldShipPanel.gameObject);
            }
        }

        private static void ShowAvailableShip(ShipRecord ship)
        {
            int columnsCount = 5;

            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipPanel", typeof(GameObject));
            GameObject newShipPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SelectShipPanel/Panel").transform);

            ShipPanelSquadBuilder script = newShipPanel.GetComponent<ShipPanelSquadBuilder>();
            script.ImageUrl = GetImageOfIconicPilot(ship);
            script.ShipName = ship.ShipName;

            int row = availableShipsCounter / columnsCount;
            int column = availableShipsCounter - (row * columnsCount);

            newShipPanel.transform.localPosition = new Vector3(25 + column * 300 + 25 * (column), - (20 + row * 184 + 20 * (row)), 0);

            availableShipsCounter++;
        }

        private static string GetImageOfIconicPilot(ShipRecord ship)
        {
            string image = null;

            if (ship.Instance.IconicPilot != null)
            {
                image = AllPilots.Find(n => n.PilotName == ship.Instance.IconicPilot).Instance.ImageUrl;
            }

            return image;
        }

        private static void ShowAvailablePilots(Faction faction, string shipName)
        {
            DeleteOldPilots();
            availablePilotsCounter = 0;
            ShipRecord shipRecord = AllShips.Find(n => n.ShipName == shipName);

            List<PilotRecord> AllPilotsFiltered = AllPilots.Where(n => n.PilotShip == shipRecord && n.Instance.faction == faction).OrderByDescending(n => n.PilotSkill).ToList();

            foreach (PilotRecord pilot in AllPilotsFiltered)
            {
                ShowAvailablePilot(pilot);
            }
        }

        private static void DeleteOldPilots()
        {
            foreach (Transform oldPilotPanel in GameObject.Find("UI/Panels/SelectPilotPanel/Panel/Scroll View/Viewport/Content").transform)
            {
                GameObject.Destroy(oldPilotPanel.gameObject);
            }
        }

        private static void ShowAvailablePilot(PilotRecord pilotRecord)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/SelectPilotPanel/Panel/Scroll View/Viewport/Content").transform;
            GameObject newPilotPanel = MonoBehaviour.Instantiate(prefab, contentTransform);

            PilotPanelSquadBuilder script = newPilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.ImageUrl = pilotRecord.Instance.ImageUrl;
            script.PilotName = pilotRecord.PilotName;
            script.ShipName = CurrentShipToBrowsePilots;
            script.OnClick = AddPilotToSquadAndReturn;

            int column = availablePilotsCounter;

            newPilotPanel.transform.localPosition = new Vector3(20 + (300 + 20) * column, 209, 0);
            contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(newPilotPanel.transform.localPosition.x + 300f, 0);

            availablePilotsCounter++;
        }

        public static void AddPilotToSquadAndReturn(string pilotName, string shipName)
        {
            AddPilotToSquad(pilotName, shipName);
            MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
        }

        private static void GenerateShipWithUpgradesPanels()
        {
            RemoveOldShipWithUpgradesPanels();

            CreateShipWithUpgradesPanels();

            ShowAddShipPanel();

            OrganizeShipWithUpgradesPanels();
        }

        private static void RemoveOldShipWithUpgradesPanels()
        {
            ShipWithUpgradesPanels = new List<ShipWithUpgradesPanel>();
            foreach (Transform oldShipWithUpgrades in GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered/SquadListPanel").transform)
            {
                GameObject.Destroy(oldShipWithUpgrades.gameObject);
            }
        }

        private static void CreateShipWithUpgradesPanels()
        {
            foreach (SquadBuilderShip ship in CurrentSquadList.GetShips())
            {
                AddShipWithUpgradesPanel(ship);
            }
        }

        private static void AddShipWithUpgradesPanel(SquadBuilderShip ship)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipWithUpgradesPanel", typeof(GameObject));
            GameObject shipWithUpgradesPanelGO = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered/SquadListPanel").transform);
            ShipWithUpgradesPanel shipWithUpgradesPanel = new ShipWithUpgradesPanel(ship, shipWithUpgradesPanelGO);
            ShipWithUpgradesPanels.Add(shipWithUpgradesPanel);

            prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            GameObject pilotPanel = MonoBehaviour.Instantiate(prefab, shipWithUpgradesPanelGO.transform);
            pilotPanel.transform.localPosition = Vector3.zero;
            PilotPanelSquadBuilder script = pilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.PilotName = ship.Instance.PilotName;
            script.ImageUrl = ship.Instance.ImageUrl;
            script.OnClick = OpenShipInfo;
        }

        private static void ShowAddShipPanel()
        {
            if (GetSquadCost() < 89)
            {
                GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipWithUpgradesPanel", typeof(GameObject));
                GameObject addShipButtonPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered/SquadListPanel").transform);
                AddShipButtonPanel = new ShipWithUpgradesPanel(null, addShipButtonPanel);

                prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/AddShipButton", typeof(GameObject));
                GameObject addShipButton = MonoBehaviour.Instantiate(prefab, addShipButtonPanel.transform);

                EventTrigger trigger = addShipButton.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener(delegate { OpenSelectShip(); });
                trigger.triggers.Add(entry);
            }
            else
            {
                AddShipButtonPanel = null;
            }                
        }

        private static void OrganizeShipWithUpgradesPanels()
        {
            float allPanelsWidth = 0;
            float distanceBetweenX = 20;
            float defaultWidth = 1326;

            foreach (ShipWithUpgradesPanel panel in ShipWithUpgradesPanels)
            {
                allPanelsWidth = allPanelsWidth + panel.Size.x;
            }
            if (ShipWithUpgradesPanels.Count > 2)
            {
                allPanelsWidth += distanceBetweenX * (ShipWithUpgradesPanels.Count - 1);
            }

            if (AddShipButtonPanel != null)
            {
                allPanelsWidth += AddShipButtonPanel.Size.x;
            }
            
            if (ShipWithUpgradesPanels.Count > 0)
            {
                allPanelsWidth += distanceBetweenX;
            }

            if (defaultWidth/allPanelsWidth > 0.75f)
            {
                ArrangeShipsWithUpgradesInOneLine(allPanelsWidth);
            }
            else
            {
                ArrangeShipsWithUpgradesInTwoLines(allPanelsWidth);
            }
            
        }

        private static void ArrangeShipsWithUpgradesInOneLine(float allPanelsWidth)
        {
            float defaultWidth = 1326;
            float defaultCardHeight = 418;
            float distanceBetweenX = 20;
            float offset = 0;

            GameObject centerPanel = GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered");
            centerPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(allPanelsWidth, defaultCardHeight);

            foreach (ShipWithUpgradesPanel panel in ShipWithUpgradesPanels)
            {
                panel.Panel.transform.localPosition = new Vector2(offset, 0);
                offset += panel.Size.x + distanceBetweenX;
            }

            if (AddShipButtonPanel != null)
            {
                AddShipButtonPanel.Panel.transform.localPosition = new Vector2(offset, 0);
            }

            float scale = Mathf.Min(defaultWidth / allPanelsWidth, 1);
            centerPanel.transform.localScale = new Vector2(scale, scale);
        }

        private static void ArrangeShipsWithUpgradesInTwoLines(float allPanelsWidth)
        {
            float defaultHeight = 600;
            float defaultCardHeight = 418;
            float distanceBetweenX = 20;
            float distanceBetweenY = 20;
            float offset = 0;

            Dictionary<ShipWithUpgradesPanel, int> panelsByRow = GetArrangeShipsWithUpgradesIntoRowNumbers();
            float row1width = panelsByRow.Where(n => n.Value == 1).Sum(m => m.Key.Size.x) + distanceBetweenX * (panelsByRow.Count(n => n.Value == 1) - 1);
            float row2width = panelsByRow.Where(n => n.Value == 2).Sum(m => m.Key.Size.x) + distanceBetweenX * (panelsByRow.Count(n => n.Value == 1) - 1);
            if (AddShipButtonPanel != null) row2width += AddShipButtonPanel.Size.x;
            float maxWidth = Mathf.Max(row1width, row2width);

            GameObject centerPanel = GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered");
            centerPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth, 2*defaultCardHeight + distanceBetweenY);

            offset = (maxWidth - row1width) / 2;
            foreach (var panel in panelsByRow.Where(n => n.Value == 1))
            {
                panel.Key.Panel.transform.localPosition = new Vector2(offset, 0);
                offset += panel.Key.Size.x + distanceBetweenX;
            }

            offset = (maxWidth - row2width) / 2;
            foreach (var panel in panelsByRow.Where(n => n.Value == 2))
            {
                panel.Key.Panel.transform.localPosition = new Vector2(offset, -(defaultCardHeight + distanceBetweenY));
                offset += panel.Key.Size.x + distanceBetweenX;
            }

            if (AddShipButtonPanel != null)
            {
                AddShipButtonPanel.Panel.transform.localPosition = new Vector2(offset, -(defaultCardHeight + distanceBetweenY));
            }

            float scale = Mathf.Min(defaultCardHeight / defaultHeight, 1);
            centerPanel.transform.localScale = new Vector2(scale, scale);
        }

        private static Dictionary<ShipWithUpgradesPanel, int> GetArrangeShipsWithUpgradesIntoRowNumbers()
        {
            Dictionary<ShipWithUpgradesPanel, int> result = new Dictionary<ShipWithUpgradesPanel, int>();

            ShipWithUpgradesPanel maxSizePanel = ShipWithUpgradesPanels.Find(n => n.Size.x == ShipWithUpgradesPanels.Max(m => m.Size.x));
            result.Add(maxSizePanel, 1);

            float maxWidth = ShipWithUpgradesPanels.Sum(n => n.Size.x) - maxSizePanel.Size.x;
            if (AddShipButtonPanel != null) maxWidth += AddShipButtonPanel.Size.x;
            float difference = Mathf.Abs(maxWidth - maxSizePanel.Size.x);

            bool finished = false;
            while (!finished)
            {
                ShipWithUpgradesPanel minSizePanel = ShipWithUpgradesPanels.Find(n => n.Size.x == ShipWithUpgradesPanels.Min(m => m.Size.x) && !result.ContainsKey(n));
                float newDifference = Mathf.Abs(result.Sum(n => n.Key.Size.x) + minSizePanel.Size.x - maxWidth + (result.Sum(n => n.Key.Size.x) + minSizePanel.Size.x));
                if (newDifference > difference)
                {
                    foreach (ShipWithUpgradesPanel panel in ShipWithUpgradesPanels)
                    {
                        if (!result.ContainsKey(panel)) result.Add(panel, 2);
                    }
                    finished = true;
                }
                else
                {
                    result.Add(minSizePanel, 1);
                    difference -= minSizePanel.Size.x;
                    maxWidth -= minSizePanel.Size.x;
                }
            }

            return result;
        }

        private static void UpdateSquadCost(int squadCost)
        {
            Text squadCostText = GameObject.Find("UI/Panels/SquadBuilderPanel/ControlsPanel/SquadCostText").GetComponent<Text>();
            squadCostText.text = squadCost.ToString() + " / 100";

            squadCostText.color = (squadCost > 100) ? Color.red : Color.black;
        }

    }
}
