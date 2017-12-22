using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace AuzituckGunship
    {
        public class AuzituckGunship : GenericShip
        {

            public AuzituckGunship() : base()
            {
                Type = "Auzituck Gunship";
                ShipBaseArcsType = Arcs.BaseArcsType.Arc180;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/e/e3/36-36_R-SPACE.PNG";

                Firepower = 3;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 3;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                PrintedActions.Add(new ReinforceForeAction() { Host = this });
                PrintedActions.Add(new ReinforceAftAction() { Host = this });

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.AuzituckGunshipTable();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "Kashyyyk Defender";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
                
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("5.F.S", ManeuverColor.Red);
            }

        }
    }
}
