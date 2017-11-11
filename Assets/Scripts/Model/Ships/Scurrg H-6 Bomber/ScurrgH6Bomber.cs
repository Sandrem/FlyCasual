using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class ScurrgH6Bomber : GenericShip
        {

            public ScurrgH6Bomber() : base()
            {
                Type = "Scurrg H-6 Bomber";

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/5/55/36-36_S-SPACE.PNG";

                Firepower = 3;
                Agility = 1;
                MaxHull = 5;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Turret);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);

                AssignTemporaryManeuvers();
                HotacManeuverTable = null;

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Lok Revenant";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            public override void InitializeShip()
            {
                base.InitializeShip();
                BuiltInActions.Add(new ActionsList.TargetLockAction());
                BuiltInActions.Add(new ActionsList.BarrelRollAction());
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.B", ManeuverColor.White);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.White);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("3.L.T", ManeuverColor.Red);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.Red);
                Maneuvers.Add("3.L.E", ManeuverColor.Red);
                Maneuvers.Add("3.R.E", ManeuverColor.Red);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("5.F.S", ManeuverColor.Red);
            }

        }
    }
}
