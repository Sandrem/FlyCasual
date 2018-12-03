using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;
using Ship;
using Tokens;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class TIEVnSilencer : FirstEdition.TIESilencer.TIESilencer, TIE
        {
            public TIEVnSilencer() : base()
            {
                ShipInfo.ShipName = "TIE/vn Silencer";

                ShipInfo.DefaultShipFaction = Faction.FirstOrder;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.FirstOrder };

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.System);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Missile);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Torpedo);

                IconicPilots[Faction.FirstOrder] = typeof(FirstOrderTestPilot);

                ShipAbilities.Add(new Abilities.SecondEdition.AutoThrustersAbility());

                // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/44/Maneuver_tie_phantom.png";
            }
        }
    }
}