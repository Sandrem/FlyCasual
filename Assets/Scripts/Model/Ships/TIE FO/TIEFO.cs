using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

namespace Ship
{
    namespace TIEFO
    {
        public class TIEFO : GenericShip, TIE
        {

            public TIEFO() : base()
            {
                Type = "TIE/FO Fighter";

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/4/4f/MI_TIE-FO-FIGHTER.png";

                Firepower = 2;
                Agility = 3;
                MaxHull = 3;
                MaxShields = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Tech);

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEFighterTable();

                factions.Add(Faction.Empire);
                faction = Faction.Empire;

                SkinName = "First Order";

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
                BuiltInActions.Add(new ActionsList.EvadeAction());
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
                Maneuvers.Add("2.L.R", ManeuverColor.Red);
                Maneuvers.Add("2.R.R", ManeuverColor.Red);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
                Maneuvers.Add("5.F.S", ManeuverColor.White);
            }

        }
    }
}
