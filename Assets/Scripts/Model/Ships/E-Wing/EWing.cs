using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace EWing
    {
        public class EWing : GenericShip, ISecondEditionShip
        {

            public EWing() : base()
            {
                Type = FullType = "E-Wing";
                IconicPilots.Add(Faction.Rebel, typeof(KnaveSquadronPilot));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/a/aa/MR_E-WING.png";

                Firepower = 3;
                Agility = 3;
                MaxHull = 2;
                MaxShields = 3;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new EvadeAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.EWingTable();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "Red";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
                
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.B", MovementComplexity.Normal);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.F.R", MovementComplexity.None);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.F.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
                Maneuvers.Add("5.F.S", MovementComplexity.Normal);
            }

            public void AdaptShipToSecondEdition()
            {
                FullType = "E-wing";

                Maneuvers.Add("1.L.T", MovementComplexity.Complex);
                Maneuvers["1.L.B"] = MovementComplexity.Easy;
                Maneuvers["1.R.B"] = MovementComplexity.Easy;
                Maneuvers.Add("1.R.T", MovementComplexity.Complex);
                Maneuvers["2.L.B"] = MovementComplexity.Normal;
                Maneuvers["2.R.B"] = MovementComplexity.Normal;
                Maneuvers.Remove("3.F.R");
                Maneuvers.Add("3.L.R", MovementComplexity.Complex);
                Maneuvers.Add("3.R.R", MovementComplexity.Complex);
                Maneuvers["4.F.S"] = MovementComplexity.Easy;

                MaxHull = 3;

                ActionBar.AddPrintedAction(new BoostAction());
                ActionBar.AddActionLink(typeof(BarrelRollAction), new TargetLockAction() { IsRed = true });
                ActionBar.AddActionLink(typeof(BoostAction), new TargetLockAction() { IsRed = true });

                SetTargetLockRange(2, int.MaxValue);

                IconicPilots[Faction.Rebel] = typeof(GavinDarklighter);
            }

        }
    }
}
