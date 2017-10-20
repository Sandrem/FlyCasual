using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class AlphaClassStarWing : GenericShip
        {

            public AlphaClassStarWing() : base()
            {
                Type = "Alpha-class Star Wing";

                //TODO: Use table instead of dial
                ManeuversImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/40/0b/400bd56b-4bb9-4046-a5b3-2575f7a40088/swx69_maneuver_dial.png";

                Firepower = 2;
                Agility = 2;
                MaxHull = 4;
                MaxShields = 3;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);

                AssignTemporaryManeuvers();
                HotacManeuverTable = null;

                factions.Add(Faction.Empire);
                faction = Faction.Empire;

                SkinName = "Alpha-class Star Wing";

                SoundShotsPath = "Slave1-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 3; i++)
                {
                    SoundFlyPaths.Add("Slave1-Fly" + i);
                }

            }

            public override void InitializeShip()
            {
                base.InitializeShip();
                BuiltInActions.Add(new ActionsList.TargetLockAction());
                BuiltInActions.Add(new ActionsList.SlamAction());
                BuiltInActions.Add(new ActionsList.ReloadAction());
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
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("4.F.S", ManeuverColor.Red);
            }

        }
    }
}
