using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace TIEBomber
    {
        public class TIEBomber : GenericShip, TIE, ISecondEditionShip
        {

            public TIEBomber() : base()
            {
                Type = "TIE Bomber";
                IconicPilots.Add(Faction.Imperial, typeof(TomaxBren));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/9/96/MI_TIE-BOMBER.png";

                Firepower = 2;
                Agility = 2;
                MaxHull = 6;
                MaxShields = 0;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEBomberTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "Blue";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
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
                Maneuvers.Add("2.L.T", MovementComplexity.Complex);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Complex);
                Maneuvers.Add("2.F.R", MovementComplexity.None);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.F.R", MovementComplexity.None);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.None);
                Maneuvers.Add("5.F.S", MovementComplexity.None);
                Maneuvers.Add("5.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                //TODO: Maneuvers

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Gunner);

                ActionBar.RemovePrintedAction(typeof(BarrelRollAction));
                ActionBar.AddPrintedAction(new BarrelRollAction() { LinkedRedAction = new TargetLockAction() { IsRed = true } });
                ActionBar.AddPrintedAction(new ReloadAction() { IsRed = true });

                IconicPilots[Faction.Imperial] = typeof(ScimitarSquadronPilot);
            }

        }
    }
}
