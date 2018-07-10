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
                Type = "B-Wing";
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

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new BarrelRollAction());

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
                Maneuvers.Add("1.F.R", MovementComplexity.None);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.F.R", MovementComplexity.Complex);
                Maneuvers.Add("3.L.T", MovementComplexity.None);
                Maneuvers.Add("3.L.B", MovementComplexity.Complex);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Complex);
                Maneuvers.Add("3.R.T", MovementComplexity.None);
                Maneuvers.Add("3.F.R", MovementComplexity.None);
                Maneuvers.Add("4.F.S", MovementComplexity.Complex);
                Maneuvers.Add("4.F.R", MovementComplexity.None);
                Maneuvers.Add("5.F.S", MovementComplexity.None);
                Maneuvers.Add("5.F.R", MovementComplexity.None);
            }

            public void AdaptShipToSecondEdition()
            {
                // TODO: Maneuvers

                MaxHull = 4;
                MaxShields = 4;

                PrintedActions.RemoveAll(a => a is FocusAction);
                PrintedActions.Add(new FocusAction() { LinkedRedAction = new BarrelRollAction() { IsRed = true } });

                IconicPilots[Faction.Rebel] = typeof(BlueSquadronPilot);
            }

        }
    }
}
