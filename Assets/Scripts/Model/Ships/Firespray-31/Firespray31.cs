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
                Type = "Firespray-31";
                IconicPilots.Add(Faction.Imperial, typeof(BobaFettEmpire));
                IconicPilots.Add(Faction.Scum, typeof(EmonAzzameen));
                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.ArcRear;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/4/4e/Firespray_31_Move.png";

                Firepower = 3;
                Agility = 2;
                MaxHull = 6;
                MaxShields = 4;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Cannon);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new EvadeAction());

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
                Maneuvers.Add("1.L.T", ManeuverColor.None);
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.Green);
                Maneuvers.Add("1.R.T", ManeuverColor.None);
                Maneuvers.Add("1.F.R", ManeuverColor.None);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("2.F.R", ManeuverColor.None);
                Maneuvers.Add("3.L.E", ManeuverColor.None);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("3.R.E", ManeuverColor.None);
                Maneuvers.Add("3.F.R", ManeuverColor.Red);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
                Maneuvers.Add("5.F.S", ManeuverColor.None);
                Maneuvers.Add("5.F.R", ManeuverColor.None);
            }

            public void AdaptShipToSecondEdition()
            {
                MaxHull = 6;
                MaxShields = 2;

                Maneuvers["1.L.T"] = ManeuverColor.White;
                Maneuvers["1.R.T"] = ManeuverColor.White;
                Maneuvers["3.L.T"] = ManeuverColor.None;
                Maneuvers["3.R.T"] = ManeuverColor.None;
                Maneuvers["3.L.E"] = ManeuverColor.Red;
                Maneuvers["3.R.E"] = ManeuverColor.Red;
                Maneuvers["3.F.S"] = ManeuverColor.Green;
                Maneuvers["3.F.R"] = ManeuverColor.None;

                PrintedUpgradeIcons.Remove(PrintedUpgradeIcons.Find(n => n.GetType() == typeof(EvadeAction)));

                PrintedActions.Add(new ReinforceAftAction());
                PrintedActions.Add(new ReinforceForeAction());
                PrintedActions.Add(new BoostAction());
            }

        }
    }
}
