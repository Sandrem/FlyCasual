using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class Mg100StarFortress : FirstEdition.BSF17Bomber.BSF17Bomber
        {
            public Mg100StarFortress() : base()
            {
                ShipInfo.ShipName = "MG-100 Star Fortress";

                ShipInfo.ArcInfo.Firepower = 3;
                // Arcs

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(RotateArcAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReloadAction)));

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Crew);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);

                ShipInfo.DefaultShipFaction = Faction.Resistance;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.Resistance };

                IconicPilots[Faction.Resistance] = typeof(CobaltSquadronBomber);

                // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/cf/Maneuver_t-65_x-wing.png";
            }
        }
    }
}
