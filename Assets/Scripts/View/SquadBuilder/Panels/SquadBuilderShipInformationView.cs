using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SquadBuilderNS
{
    public class SquadBuilderShipInformationView
    {
        public void ShowShipInformation()
        {
            GenericShip ship = (MainMenu.CurrentMainMenu.PreviousPanelName == "SelectPilotPanel") ? Global.SquadBuilder.Database.AllShips.Find(n => n.ShipName == Global.SquadBuilder.CurrentShipName).Instance : Global.SquadBuilder.CurrentShip.Instance;

            Text shipNameText = GameObject.Find("UI/Panels/ShipInfoPanel/Content/TopPanel/ShipTypeText").GetComponent<Text>();
            shipNameText.text = ship.ShipInfo.ShipName;

            Text sizeText = GameObject.Find("UI/Panels/ShipInfoPanel/Content/TopPanel/ShipSizeText").GetComponent<Text>();
            switch (ship.ShipInfo.BaseSize)
            {
                case BaseSize.Small:
                    sizeText.text = "Small Ship";
                    break;
                case BaseSize.Medium:
                    sizeText.text = "Medium Ship";
                    break;
                case BaseSize.Large:
                    sizeText.text = "Large Ship";
                    break;
                default:
                    break;
            }

            Transform parentTransform = GameObject.Find("UI/Panels").transform
                .Find("ShipInfoPanel")
                .Find("Content")
                .Find("CenterPanel");

            GameObject oldDial = parentTransform.Find("ManeuversDialView")?.gameObject;
            if (oldDial != null) GameObject.Destroy(oldDial);

            GameObject dial = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ManeuversDial/ManeuversDialView"), parentTransform);
            dial.name = "ManeuversDialView";
            dial.GetComponent<ManeuversDialView>().Initialize(ship.DialInfo.PrintedDial, isDisabled: true);
        }
    }
}
