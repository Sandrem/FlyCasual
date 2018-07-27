using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace YT1300
    {
        public class YT1300 : GenericShip, ISecondEditionShip
        {

            public YT1300() : base()
            {
                Type = "YT-1300";
                IconicPilots.Add(Faction.Rebel, typeof(HanSolo));
                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.Arc360;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/a/a0/YT_1300_Move.png";

                Firepower = 2;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 4;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                ActionBar.AddPrintedAction(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.YT1300Table();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "YT-1300";

                SoundShotsPath = "Falcon-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("Falcon-Fly" + i);
                }

            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Normal);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("1.F.R", MovementComplexity.None);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                //TODO: Arcs

                Maneuvers.Remove("1.L.T");
                Maneuvers["1.L.B"] = MovementComplexity.Normal;
                Maneuvers["1.R.B"] = MovementComplexity.Normal;
                Maneuvers.Remove("1.R.T");
                Maneuvers["2.L.B"] = MovementComplexity.Easy;
                Maneuvers["2.R.B"] = MovementComplexity.Easy;
                Maneuvers["3.L.R"] = MovementComplexity.Complex;
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers["3.R.R"] = MovementComplexity.Complex;

                MaxHull = 8;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Gunner);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                ActionBar.AddPrintedAction(new RotateArcAction());
                ActionBar.AddPrintedAction(new BoostAction() { IsRed = true });

                IconicPilots[Faction.Rebel] = typeof(OuterRimSmuggler);
            }
        }
    }
}
