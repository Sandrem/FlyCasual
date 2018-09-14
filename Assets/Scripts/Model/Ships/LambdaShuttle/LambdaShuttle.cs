﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class LambdaShuttle : GenericShip, ISecondEditionShip
        {

            public LambdaShuttle() : base()
            {
                Type = FullType = "Lambda-class Shuttle";
                IconicPilots.Add(Faction.Imperial, typeof(CaptainYorr));
                ShipBaseSize = BaseSize.Large;

                ManeuversImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/c/c3/MI_LAMBDA-SHUTTLE.png";

                Firepower = 3;
                Agility = 1;
                MaxHull = 5;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Cannon);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                ActionBar.AddPrintedAction(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.LambdaShuttleTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "Default";

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
                Maneuvers.Add("3.L.B", MovementComplexity.Complex);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                FullType = "Lambda-class T-4a Shuttle";

                ShipBaseArcsType = Arcs.BaseArcsType.ArcRear;
                ShipAbilities.Add(new Abilities.SecondEdition.WeakNonPrimaryArc());

                ActionBar.AddPrintedAction(new ReinforceForeAction());
                ActionBar.AddPrintedAction(new ReinforceAftAction());
                ActionBar.AddPrintedAction(new CoordinateAction());
                ActionBar.AddPrintedAction(new JamAction() { IsRed = true });

                IconicPilots[Faction.Imperial] = typeof(ColonelJendon);
            }

        }
    }
}
