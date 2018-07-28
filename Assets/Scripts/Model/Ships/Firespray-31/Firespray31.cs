using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace Firespray31
    {
        public class Firespray31 : GenericShip, ISecondEditionShip
        {

            public Firespray31() : base()
            {
                Type = FullType = "Firespray-31";
                IconicPilots.Add(Faction.Imperial, typeof(BobaFettEmpire));
                IconicPilots.Add(Faction.Scum, typeof(EmonAzzameen));
                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.ArcRear;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/4/4e/Firespray_31_Move.png";

                ShipIconLetter = 'f';

                Firepower = 3;
                Agility = 2;
                MaxHull = 6;
                MaxShields = 4;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Cannon);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new EvadeAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.Firespray31Table();

                factions.Add(Faction.Imperial);
                factions.Add(Faction.Scum);

                SoundShotsPath = "Slave1-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 3; i++)
                {
                    SoundFlyPaths.Add("Slave1-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.None);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.T", MovementComplexity.None);
                Maneuvers.Add("1.F.R", MovementComplexity.None);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.F.R", MovementComplexity.None);
                Maneuvers.Add("3.L.E", MovementComplexity.None);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.R.E", MovementComplexity.None);
                Maneuvers.Add("3.F.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
                Maneuvers.Add("5.F.S", MovementComplexity.None);
                Maneuvers.Add("5.F.R", MovementComplexity.None);
            }

            public void AdaptShipToSecondEdition()
            {
                FullType = "Firespray-class Patrol Craft";

                ShipBaseSize = BaseSize.Medium;

                Maneuvers["1.L.T"] = MovementComplexity.Normal;
                Maneuvers["1.R.T"] = MovementComplexity.Normal;
                Maneuvers["3.L.T"] = MovementComplexity.None;
                Maneuvers["3.R.T"] = MovementComplexity.None;
                Maneuvers["3.L.E"] = MovementComplexity.Complex;
                Maneuvers["3.R.E"] = MovementComplexity.Complex;
                Maneuvers["3.F.S"] = MovementComplexity.Easy;
                Maneuvers["3.F.R"] = MovementComplexity.None;

                ActionBar.RemovePrintedAction(typeof(EvadeAction));

                ActionBar.AddPrintedAction(new ReinforceAftAction() {Host = this, IsRed = true});
                ActionBar.AddPrintedAction(new ReinforceForeAction() {Host = this, IsRed = true});
                ActionBar.AddPrintedAction(new BoostAction());

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                factions.Remove(Faction.Imperial);
            }

        }
    }
}
