using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace M12LKimogila
    {
        public class M12LKimogila : GenericShip
        {

            public M12LKimogila() : base()
            {
                Type = "M12-L Kimogila Fighter";

                ShipBaseArcsType = Arcs.BaseArcsType.ArcBullseye;

                //ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/4/48/MS_M3-A-INTERCEPTOR.png";

                Firepower = 3;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.SalvagedAstromech);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new BarrelRollAction());
                PrintedActions.Add(new ReloadAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = null;

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Hutt Cartel";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.Red);
                Maneuvers.Add("1.L.B", ManeuverColor.White);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.White);
                Maneuvers.Add("1.R.T", ManeuverColor.Red);
                Maneuvers.Add("2.L.T", ManeuverColor.Red);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.Red);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
            }

        }
    }
}
