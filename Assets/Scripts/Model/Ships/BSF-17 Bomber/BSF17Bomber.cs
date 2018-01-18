using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace BSF17Bomber
    {
        public class BSF17Bomber : GenericShip
        {

            public BSF17Bomber() : base()
            {
                Type = "B/SF-17 Bomber";
                IconicPilot = "Crimson Squadron Pilot";
                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.Arc360;

                ManeuversImageUrl = "https://i.imgur.com/jvKq5hI.jpg";

                Firepower = 2;
                Agility = 1;
                MaxHull = 9;
                MaxShields = 3;

                SubFaction = SubFaction.Resistance;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Tech);

                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                //HotacManeuverTable = new AI.YT2400Table();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "Crimson";

                SoundShotsPath = "Falcon-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("Falcon-Fly" + i);
                }

            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("0.S.S", ManeuverColor.Red);
                Maneuvers.Add("1.L.T", ManeuverColor.Red);
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.Green);
                Maneuvers.Add("1.R.T", ManeuverColor.Red);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
            }

        }
    }
}
