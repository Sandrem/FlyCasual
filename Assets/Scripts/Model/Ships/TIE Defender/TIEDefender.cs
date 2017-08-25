using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

namespace Ship
{
    namespace TIEDefender
    {
        public class TIEDefender : GenericShip
        {

            public TIEDefender() : base()
            {
                Type = "TIE Defender";
                ManeuversImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/8/85/MI_TIE-ADVANCED.png";

                Firepower = 3;
                Agility = 3;
                MaxHull = 3;
                MaxShields = 3;

                AddUpgradeSlot(Upgrade.UpgradeType.Cannon);
                AddUpgradeSlot(Upgrade.UpgradeType.Missiles);

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEDefenderTable();

                factions.Add(Faction.Empire);
                faction = Faction.Empire;

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 3;

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
                Maneuvers.Add("1.L.T", ManeuverColor.Red);
                Maneuvers.Add("1.L.B", ManeuverColor.White);
                Maneuvers.Add("1.F.S", ManeuverColor.None);
                Maneuvers.Add("1.R.B", ManeuverColor.White);
                Maneuvers.Add("1.R.T", ManeuverColor.Red);
                Maneuvers.Add("1.F.R", ManeuverColor.None);
                Maneuvers.Add("2.L.T", ManeuverColor.Red);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.Red);
                Maneuvers.Add("2.F.R", ManeuverColor.None);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("3.F.R", ManeuverColor.None);
                Maneuvers.Add("4.F.S", ManeuverColor.Green);
                Maneuvers.Add("4.F.R", ManeuverColor.White);
                Maneuvers.Add("5.F.S", ManeuverColor.Green);
                Maneuvers.Add("5.F.R", ManeuverColor.None);
            }

        }
    }
}
