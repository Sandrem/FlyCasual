using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace VT49Decimator
    {
        public class VT49Decimator : GenericShip, ISecondEditionShip
        {

            public VT49Decimator() : base()
            {
                Type = "VT-49 Decimator";
                IconicPilots.Add(Faction.Imperial, typeof(RearAdmiralChiraneau));
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

                ActionBar.AddPrintedAction(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.VT49DecimatorTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

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
                Maneuvers.Add("1.L.B", MovementComplexity.Normal);
                Maneuvers.Add("1.F.S", MovementComplexity.Normal);
                Maneuvers.Add("1.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
            }

            public void AdaptShipToSecondEdition()
            {
                ShipBaseArcsType = Arcs.BaseArcsType.ArcMobileDual;

                Maneuvers.Add("1.L.T", MovementComplexity.Complex);
                Maneuvers["1.L.B"] = MovementComplexity.Easy;
                Maneuvers["1.F.S"] = MovementComplexity.Easy;
                Maneuvers["1.R.B"] = MovementComplexity.Easy;
                Maneuvers.Add("1.R.T", MovementComplexity.Complex);
                Maneuvers["2.L.B"] = MovementComplexity.Normal;
                Maneuvers["2.R.B"] = MovementComplexity.Normal;
                Maneuvers["3.F.S"] = MovementComplexity.Normal;

                ActionBar.AddPrintedAction(new ReinforceForeAction());
                ActionBar.AddPrintedAction(new ReinforceAftAction());
                ActionBar.AddPrintedAction(new RotateArcAction());
                ActionBar.AddPrintedAction(new CoordinateAction() { IsRed = true });

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Gunner);

                IconicPilots[Faction.Imperial] = typeof(PatrolLeader);
            }

        }
    }
}
