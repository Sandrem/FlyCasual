using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace Z95
    {
        public class Z95 : GenericShip, ISecondEditionShip
        {

            public Z95() : base()
            {
                Type = "Z-95 Headhunter";
                IconicPilots.Add(Faction.Rebel, typeof(AirenCracken));
                IconicPilots.Add(Faction.Scum, typeof(NdruSuhlak));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/3/39/MR_Z-95.png";

                Firepower = 2;
                Agility = 2;
                MaxHull = 2;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.Z95Table();

                factions.Add(Faction.Rebel);
                factions.Add(Faction.Scum);

                ActionBar.AddPrintedAction(new TargetLockAction());

                SkinName = "Blue";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 2;

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
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.F.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
            }

            public void AdaptShipToSecondEdition()
            {
                //TODO: Maneuvers

                ActionBar.AddPrintedAction(new BarrelRollAction() { IsRed = true });

                IconicPilots[Faction.Scum] = typeof(BinayrePirate);
                IconicPilots[Faction.Rebel] = typeof(BanditSquadronPilot);
            }

        }
    }
}
