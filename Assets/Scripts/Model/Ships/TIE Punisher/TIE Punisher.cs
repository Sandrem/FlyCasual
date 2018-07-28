using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace TIEPunisher
    {
        public class TIEPunisher : GenericShip, TIE, ISecondEditionShip
        {

            public TIEPunisher() : base()
            {
                Type = FullType = "TIE Punisher";
                IconicPilots.Add(Faction.Imperial, typeof(BlackEightSqPilot));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/7/72/MI_TIE-PUNISHER.png";

                Firepower = 2;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 3;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new BoostAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEPunisherTable();

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
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.L.T", MovementComplexity.Complex);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Complex);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                FullType = "TIE/ca Punisher";

                Maneuvers.Add("0.S.S", MovementComplexity.Complex);
                Maneuvers["2.L.T"] = MovementComplexity.Normal;
                Maneuvers["2.R.T"] = MovementComplexity.Normal;
                Maneuvers["3.L.T"] = MovementComplexity.Complex;
                Maneuvers["3.R.T"] = MovementComplexity.Complex;

                ShipBaseSize = BaseSize.Medium;

                ActionBar.RemovePrintedAction(typeof(BoostAction));
                ActionBar.AddPrintedAction(new BoostAction() { LinkedRedAction = new TargetLockAction() { IsRed = true } });
                ActionBar.AddPrintedAction(new BarrelRollAction() { IsRed = true });
                ActionBar.AddPrintedAction(new ReloadAction());

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Gunner);

                IconicPilots[Faction.Imperial] = typeof(CutlassSquadronPilot);
            }

        }
    }
}
