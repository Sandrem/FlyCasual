using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace JumpMaster5000
    {
        public class JumpMaster5000 : GenericShip, ISecondEditionShip
        {

            public JumpMaster5000() : base()
            {
                Type = "JumpMaster 5000";
                IconicPilots.Add(Faction.Scum, typeof(Dengar));
                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.Arc360;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/9/90/MS_JUMPMASTER-5000.png";

                Firepower = 2;
                Agility = 2;
                MaxHull = 5;
                MaxShields = 4;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.Jumpmaster5000Table();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "JumpMaster 5000";

                SoundShotsPath = "Falcon-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("Falcon-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Easy);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Normal);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Easy);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.R", MovementComplexity.Normal);
                Maneuvers.Add("2.R.R", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                MaxHull = 6;
                MaxShields = 3;

                ActionBar.RemovePrintedAction(typeof(FocusAction));
                ActionBar.RemovePrintedAction(typeof(TargetLockAction));

                ActionBar.AddPrintedAction(new FocusAction() { LinkedRedAction = new RotateArcAction() { IsRed = true } });
                ActionBar.AddPrintedAction(new TargetLockAction() { LinkedRedAction = new RotateArcAction() { IsRed = true } });

                ActionBar.AddPrintedAction(new BarrelRollAction() { IsRed = true });

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Elite);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);

                ShipBaseArcsType = Arcs.BaseArcsType.ArcMobileOnly;

                Maneuvers["1.L.T"] = MovementComplexity.Normal;
                Maneuvers["1.R.T"] = MovementComplexity.Complex;
                Maneuvers.Remove("2.L.R");
                Maneuvers["2.L.T"] = MovementComplexity.Normal;
                Maneuvers["2.R.T"] = MovementComplexity.Complex;
                Maneuvers.Remove("2.R.R");
                Maneuvers.Add("3.L.R", MovementComplexity.Complex);
                Maneuvers["3.L.B"] = MovementComplexity.Easy;
                Maneuvers["3.F.S"] = MovementComplexity.Easy;

                IconicPilots[Faction.Scum] = typeof(ContractedScout);
            }

        }
    }
}
