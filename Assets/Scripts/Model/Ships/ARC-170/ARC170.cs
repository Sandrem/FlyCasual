using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace ARC170
    {
        public class ARC170 : GenericShip, ISecondEditionShip
        {

            public ARC170() : base()
            {
                Type = FullType = "ARC-170";
                IconicPilots.Add(Faction.Rebel, typeof(NorraWexley));

                ShipBaseArcsType = Arcs.BaseArcsType.ArcRear;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/2/2f/MR_ARC-170.png";

                Firepower = 2;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 3;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                ActionBar.AddPrintedAction(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.Arc170Table();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "ARC-170";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 2;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
                
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.T", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Complex);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                FullType = "ARC-170 Starfighter";

                Firepower = 3;

                ActionBar.AddPrintedAction(new BarrelRollAction() { IsRed = true });

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Gunner);
            }

        }
    }
}
