using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace G1AStarfighter
    {
        public class G1AStarfighter : GenericShip, ISecondEditionShip
        {

            public G1AStarfighter() : base()
            {
                Type = FullType = "G-1A Starfighter";
                IconicPilots.Add(Faction.Scum, typeof(Zuckuss));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/6/67/MS_G-1A-STARFIGHTER.png";

                Firepower = 3;
                Agility = 1;
                MaxHull = 4;
                MaxShields = 4;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new EvadeAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.G1AStarfighterTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "G-1A Starfighter";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
                
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
                Maneuvers.Add("3.L.B", MovementComplexity.Complex);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Complex);
                Maneuvers.Add("3.F.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                Maneuvers.Add("0.S.S", MovementComplexity.Complex);
                Maneuvers.Add("2.F.R", MovementComplexity.Complex);
                Maneuvers.Remove("3.F.R");
                Maneuvers["3.F.S"] = MovementComplexity.Normal;
                Maneuvers["4.F.S"] = MovementComplexity.Complex;

                MaxHull = 5;
                MaxShields = 4;

                ShipBaseSize = BaseSize.Medium;

                ActionBar.RemovePrintedAction(typeof(EvadeAction));
                ActionBar.AddPrintedAction(new JamAction());
            }

        }
    }
}
