using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class AlphaClassStarWing : GenericShip
        {

            public AlphaClassStarWing() : base()
            {
                Type = "Alpha-class Star Wing";
                IconicPilots.Add(Faction.Imperial, typeof(MajorVynder));

                ManeuversImageUrl = "https://i.imgur.com/aiSqTZA.jpg";

                Firepower = 2;
                Agility = 2;
                MaxHull = 4;
                MaxShields = 3;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new SlamAction());
                PrintedActions.Add(new ReloadAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.AlphaClassStarWingTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "Gray";

                SoundShotsPath = "Slave1-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 3; i++)
                {
                    SoundFlyPaths.Add("Slave1-Fly" + i);
                }

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
