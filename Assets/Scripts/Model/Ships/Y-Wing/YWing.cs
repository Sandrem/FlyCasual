using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace YWing
    {
        public class YWing : GenericShip, ISecondEditionShip
        {

            public YWing() : base()
            {
                Type = "Y-Wing";
                IconicPilots.Add(Faction.Rebel, typeof(HortonSalm));
                IconicPilots.Add(Faction.Scum, typeof(SyndicateThug));

                ManeuversImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/1/18/MR_Y-WING.png";

                Firepower = 2;
                Agility = 1;
                MaxHull = 5;
                MaxShields = 3;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Turret);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);

                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.YWingTable();

                factions.Add(Faction.Rebel);
                factions.Add(Faction.Scum);

                SkinName = "Yellow";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 2;

                for (int i = 1; i < 3; i++)
                {
                    SoundFlyPaths.Add("YWing-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.None);
                Maneuvers.Add("1.L.B", ManeuverColor.White);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.White);
                Maneuvers.Add("1.R.T", ManeuverColor.None);
                Maneuvers.Add("1.F.R", ManeuverColor.None);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("2.F.R", ManeuverColor.None);
                Maneuvers.Add("3.L.T", ManeuverColor.Red);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.Red);
                Maneuvers.Add("3.F.R", ManeuverColor.None);
                Maneuvers.Add("4.F.S", ManeuverColor.Red);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
                Maneuvers.Add("5.F.S", ManeuverColor.None);
                Maneuvers.Add("5.F.R", ManeuverColor.None);
            }

            public void AdaptShipToSecondEdition()
            {
                MaxHull = 6;
                MaxShields = 2;

                Maneuvers["1.L.B"] = ManeuverColor.Green;
                Maneuvers["1.R.B"] = ManeuverColor.Green;

                PrintedActions.Add(new BarrelRollAction());
                PrintedActions.Add(new ReloadAction());

                factions.Remove(Faction.Scum);

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

        }
    }
}
