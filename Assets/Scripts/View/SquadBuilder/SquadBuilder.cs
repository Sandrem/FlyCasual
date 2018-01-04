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

        private static void ShowAvailableShips(Faction faction)
        {
            DeleteOldShips();
            availableShipsCounter = 0;

            foreach (var ship in AllShips)
            {
                if (ship.Instance.factions.Contains(faction))
                {
                    ShowAvailableShips(ship);
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

        private static void ShowAvailableShips(ShipRecord ship)
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
    }
}
