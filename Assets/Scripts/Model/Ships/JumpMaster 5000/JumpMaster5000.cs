using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace JumpMaster5000
    {
        public class JumpMaster5000 : GenericShip
        {

            public JumpMaster5000() : base()
            {
                Type = "JumpMaster 5000";
                IconicPilot = "Contracted Scout";
                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.Arc360;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/9/90/MS_JUMPMASTER-5000.png";

                Firepower = 2;
                Agility = 2;
                MaxHull = 5;
                MaxShields = 4;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.Jumpmaster5000Table();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "JumpMaster 5000";

                SoundShotsPath = "Falcon-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("Falcon-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.Green);
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.White);
                Maneuvers.Add("1.R.T", ManeuverColor.White);
                Maneuvers.Add("2.L.T", ManeuverColor.Green);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("2.L.R", ManeuverColor.White);
                Maneuvers.Add("2.R.R", ManeuverColor.Red);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
            }

        }
    }
}
