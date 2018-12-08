using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class T70XWing : FirstEdition.T70XWing.T70XWing
        {
            public T70XWing() : base()
            {
                ShipInfo.ShipName = "T-70 X-wing";
                ShipInfo.Hull = 4;

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Configuration);
                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Torpedo);

                ShipInfo.DefaultShipFaction = Faction.Resistance;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.Resistance };

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);

                IconicPilots[Faction.Resistance] = typeof(BlueSquadronRookie);

                ShipAbilities.Add(new Abilities.FirstEdition.HardPointAbility());

                // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/cf/Maneuver_t-65_x-wing.png";
            }
        }
    }
}
