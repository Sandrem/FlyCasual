using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace BWing
    {
        public class BWing : GenericShip, ISecondEditionShip
        {

            public BWing() : base()
            {
                Type = FullType = "B-Wing";
                IconicPilots.Add(Faction.Rebel, typeof(KeyanFarlander));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/3/32/MR_B-WING.png";

                Firepower = 3;
                Agility = 1;
                MaxHull = 3;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Cannon);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.BWingTable();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "Teal";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Complex);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.T", MovementComplexity.Complex);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.F.R", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Complex);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                FullType = "A/SF-01 B-wing";

                Maneuvers["3.F.S"] = MovementComplexity.Easy;

                MaxHull = 4;
                MaxShields = 4;

                ActionBar.AddActionLink(typeof(FocusAction), new BarrelRollAction() { IsRed = true });

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Cannon);

                IconicPilots[Faction.Rebel] = typeof(BraylenStramm);
            }

        }
    }
}
