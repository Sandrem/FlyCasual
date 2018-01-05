using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SquadBuilderNS
{
    static partial class SquadBuilder
    {
        private static int availableShipsCounter;
        private static int availablePilotsCounter;

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

            newShipPanel.GetComponent<ShipPanelSquadBuilder>().ImageUrl = GetImageOfIconicPilot(ship);
            newShipPanel.GetComponent<ShipPanelSquadBuilder>().ShipName = ship.ShipName;

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

            newPilotPanel.GetComponent<PilotPanelSquadBuilder>().ImageUrl = pilotRecord.Instance.ImageUrl;
            newPilotPanel.GetComponent<PilotPanelSquadBuilder>().PilotName = pilotRecord.PilotName;

            int column = availablePilotsCounter;

            newPilotPanel.transform.localPosition = new Vector3(20 + (300 + 20) * column, 209, 0);
            contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(newPilotPanel.transform.localPosition.x + 300f, 0);

            availablePilotsCounter++;
        }

    }
}
