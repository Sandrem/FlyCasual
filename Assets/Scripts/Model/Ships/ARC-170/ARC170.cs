using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace ARC170
    {
        public class ARC170 : GenericShip
        {

            public ARC170() : base()
            {
                Type = "ARC-170";

                ShipBaseArcsType = Arcs.BaseArcsType.ArcRear;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/2/2f/MR_ARC-170.png";

                Firepower = 2;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 3;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = null;

                factions.Add(Faction.Rebels);
                faction = Faction.Rebels;

                SkinName = "ARC-170";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 2;

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
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("3.L.T", ManeuverColor.Red);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.Red);
                Maneuvers.Add("4.F.S", ManeuverColor.Red);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
            }

        }
    }
}
