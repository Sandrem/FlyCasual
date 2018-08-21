using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace ScumYT1300
    {
        public class ScumYT1300 : GenericShip, ISecondEditionShip
        {

            public ScumYT1300() : base()
            {
                Type = FullType = "Customized YT-1300 Light Freighter";

                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.ArcMobileDual;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/a/a0/YT_1300_Move.png";

                Firepower = 2;
                Agility = 1;
                MaxHull = 8;
                MaxShields = 3;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Gunner);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new RotateArcAction());
                ActionBar.AddPrintedAction(new BoostAction() { IsRed = true });

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.YT1300Table();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Default";

                SoundShotsPath = "Falcon-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("Falcon-Fly" + i);
                }

                ShipRuleType = typeof(SecondEdition);

                IconicPilots.Add(Faction.Scum, typeof(LandoCalrissian));
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.R", MovementComplexity.Complex);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.R.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                //Not required
            }
        }
    }
}
