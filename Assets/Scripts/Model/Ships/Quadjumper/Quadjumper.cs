using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace Quadjumper
    {
        public class Quadjumper : GenericShip //, ISecondEditionShip
        {

            public Quadjumper() : base()
            {
                Type = "Quadjumper";
                IconicPilots.Add(Faction.Scum, typeof(JakkuGunrunner));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/4/4d/MS_QUADJUMPER.png";

                Firepower = 2;
                Agility = 2;
                MaxHull = 5;
                MaxShields = 0;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Tech);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                ActionBar.AddPrintedAction(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.QuadjumperTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Quadjumper";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 2;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Normal);
                Maneuvers.Add("1.F.S", MovementComplexity.Normal);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.R", MovementComplexity.Complex);
                Maneuvers.Add("2.R.R", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
            }

            public void AdaptShipToSecondEdition()
            {
                //TODO: Maneuvers
                //TODO: Ship ability

                ActionBar.AddPrintedAction(new EvadeAction() { IsRed = true });
            }

        }
    }
}
