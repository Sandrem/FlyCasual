using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace LancerClassPursuitCraft
    {
        public class LancerClassPursuitCraft : GenericShip
        {

            public LancerClassPursuitCraft() : base()
            {
                Type = "Lancer-class Pursuit Craft";
                IconicPilot = "Asajj Ventress";
                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.ArcMobile;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/f/f5/MS_LANCER-CLASS.png";

                Firepower = 3;
                Agility = 2;
                MaxHull = 7;
                MaxShields = 3;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new EvadeAction());
                PrintedActions.Add(new RotateArcAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.LancerPursuitCraftTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Lancer-class Pursuit Craft";

                SoundShotsPath = "Slave1-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 3; i++)
                {
                    SoundFlyPaths.Add("Slave1-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.B", ManeuverColor.White);
                Maneuvers.Add("1.F.S", ManeuverColor.White);
                Maneuvers.Add("1.R.B", ManeuverColor.White);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("3.L.T", ManeuverColor.Green);
                Maneuvers.Add("3.L.B", ManeuverColor.Green);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.Green);
                Maneuvers.Add("3.R.T", ManeuverColor.Green);
                Maneuvers.Add("4.F.S", ManeuverColor.Green);
                Maneuvers.Add("5.F.S", ManeuverColor.White);
                Maneuvers.Add("5.F.R", ManeuverColor.Red);
            }

        }
    }
}
