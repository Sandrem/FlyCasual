/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace TIESilencer
    {
        public class TIESilencer : GenericShip, TIE
        {

            public TIESilencer() : base()
            {
                Type = "TIE Silencer";

                //ManeuversImageUrl = "";

                Firepower = 3;
                Agility = 3;
                MaxHull = 4;
                MaxShields = 2;

                SubFaction = SubFaction.FirstOrder;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Tech);

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new BarrelRollAction());
                PrintedActions.Add(new BoostAction());

                AssignTemporaryManeuvers();
                //HotacManeuverTable = new AI.TIEInterceptorTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "First Order";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.White);
                Maneuvers.Add("1.R.T", ManeuverColor.White);
                Maneuvers.Add("2.L.T", ManeuverColor.Green);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.Green);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.Green);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.Green);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("3.L.E", ManeuverColor.Red);
                Maneuvers.Add("3.R.E", ManeuverColor.Red);
                Maneuvers.Add("4.F.S", ManeuverColor.Green);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
                Maneuvers.Add("5.F.S", ManeuverColor.Green);
            }

        }
    }
}*/
