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

                ShipIconLetter = 'y';

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
                Maneuvers.Add("1.L.T", MovementComplexity.None);
                Maneuvers.Add("1.L.B", MovementComplexity.Normal);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Normal);
                Maneuvers.Add("1.R.T", MovementComplexity.None);
                Maneuvers.Add("1.F.R", MovementComplexity.None);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.F.R", MovementComplexity.None);
                Maneuvers.Add("3.L.T", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Complex);
                Maneuvers.Add("3.F.R", MovementComplexity.None);
                Maneuvers.Add("4.F.S", MovementComplexity.Complex);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
                Maneuvers.Add("5.F.S", MovementComplexity.None);
                Maneuvers.Add("5.F.R", MovementComplexity.None);
            }

            public void AdaptShipToSecondEdition()
            {
                MaxHull = 6;
                MaxShields = 2;

                Maneuvers["1.L.B"] = MovementComplexity.Easy;
                Maneuvers["1.R.B"] = MovementComplexity.Easy;

                PrintedActions.Add(new BarrelRollAction() { IsRed = true });
                PrintedActions.Add(new ReloadAction() { IsRed = true });

                factions.Remove(Faction.Scum);

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
            }

        }
    }
}
