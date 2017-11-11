using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace VT49Decimator
    {
        public class VT49Decimator : GenericShip
        {

            public VT49Decimator() : base()
            {
                Type = "VT-49 Decimator";
                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.Arc360;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/f/fe/MI_VT-49-DECIMATOR.png";

                Firepower = 3;
                Agility = 0;
                MaxHull = 12;
                MaxShields = 4;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);

                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.VT49DecimatorTable();

                factions.Add(Faction.Empire);
                faction = Faction.Empire;

                SkinName = "Gray";

                SoundShotsPath = "Slave1-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 3; i++)
                {
                    SoundFlyPaths.Add("Slave1-Fly" + i);
                }

            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.None);
                Maneuvers.Add("1.L.B", ManeuverColor.White);
                Maneuvers.Add("1.F.S", ManeuverColor.White);
                Maneuvers.Add("1.R.B", ManeuverColor.White);
                Maneuvers.Add("1.R.T", ManeuverColor.None);
                Maneuvers.Add("1.F.R", ManeuverColor.None);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("2.F.R", ManeuverColor.None);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("3.F.R", ManeuverColor.None);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.None);
                Maneuvers.Add("5.F.S", ManeuverColor.None);
                Maneuvers.Add("5.F.R", ManeuverColor.None);
            }

        }
    }
}
