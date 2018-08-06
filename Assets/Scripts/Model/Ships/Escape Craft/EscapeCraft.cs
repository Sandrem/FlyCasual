using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace EscapeCraft
    {
        public class EscapeCraft : GenericShip, ISecondEditionShip
        {

            public EscapeCraft() : base()
            {
                Type = FullType = "Escape Craft";
                IconicPilots.Add(Faction.Scum, typeof(AutopilotDrone));

                //ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/1/12/MR_ATTACK-SHUTTLE.png";

                Firepower = 2;
                Agility = 2;
                MaxHull = 2;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                ActionBar.AddPrintedAction(new BarrelRollAction());
                ActionBar.AddPrintedAction(new CoordinateAction() { IsRed = true});

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.AttackShuttleTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Default";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 2;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }

                ShipRuleType = typeof(SecondEdition);

            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Complex);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.T", MovementComplexity.Complex);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.T", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Complex);
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
