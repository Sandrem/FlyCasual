using System.Collections;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Upgrade;

namespace Ship.SecondEdition.UT60DUWing
{
    public class UT60DUWing : FirstEdition.UWing.UWing
    {
        public UT60DUWing() : base()
        {
            ShipInfo.ShipName = "UT-60D U-wing";
            ShipInfo.BaseSize = BaseSize.Medium;

            ShipInfo.Agility = 2;
            ShipInfo.Hull = 5;
            ShipInfo.Shields = 3;

            ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Configuration);

            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(CoordinateAction), ActionColor.Red));

            IconicPilots[Faction.Rebel] = typeof(BlueSquadronScout);

            ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/6/61/Maneuver_u-wing.png";

            OldShipTypeName = "U-wing";
        }
    }
}
