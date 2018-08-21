using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace YV666
    {
        public class YV666 : GenericShip, ISecondEditionShip
        {

            public YV666() : base()
            {
                Type = FullType = "YV-666";
                IconicPilots.Add(Faction.Scum, typeof(Bossk));
                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.ArcSpecial180;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/b/be/MS_YV-666-FREIGHTER.png";

                Firepower = 3;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 6;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Cannon);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                ActionBar.AddPrintedAction(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.YV666Table();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Brown";

                SoundShotsPath = "Slave1-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 3; i++)
                {
                    SoundFlyPaths.Add("Slave1-Fly" + i);
                }

            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("0.S.S", MovementComplexity.Complex);
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
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
            }

            public void AdaptShipToSecondEdition()
            {
                FullType = "YV-666 Light Freighter";

                MaxHull = 9;
                MaxShields = 3;

                ActionBar.AddPrintedAction(new ReinforceForeAction());
                ActionBar.AddPrintedAction(new ReinforceAftAction());

                IconicPilots[Faction.Scum] = typeof(TrandoshanSlaver);
            }

        }
    }
}
