using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class LambdaShuttle : GenericShip
        {

            public LambdaShuttle() : base()
            {
                Type = "Lambda-class Shuttle";
                ShipBaseSize = BaseSize.Large;

                ManeuversImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/c/c3/MI_LAMBDA-SHUTTLE.png";

                Firepower = 3;
                Agility = 1;
                MaxHull = 5;
                MaxShields = 5;

                AddUpgradeSlot(Upgrade.UpgradeType.System);
                AddUpgradeSlot(Upgrade.UpgradeType.Cannon);
                AddUpgradeSlot(Upgrade.UpgradeType.Crew);
                AddUpgradeSlot(Upgrade.UpgradeType.Crew);

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.LambdaShuttleTable();

                factions.Add(Faction.Empire);
                faction = Faction.Empire;

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
                
            }

            public override void InitializeShip()
            {
                base.InitializeShip();
                BuiltInActions.Add(new ActionsList.TargetLockAction());
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.None);
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.Green);
                Maneuvers.Add("1.R.T", ManeuverColor.None);
                Maneuvers.Add("1.F.R", ManeuverColor.None);
                Maneuvers.Add("2.L.T", ManeuverColor.Red);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.Red);
                Maneuvers.Add("2.F.R", ManeuverColor.None);
                Maneuvers.Add("3.L.T", ManeuverColor.None);
                Maneuvers.Add("3.L.B", ManeuverColor.Red);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.Red);
                Maneuvers.Add("3.R.T", ManeuverColor.None);
                Maneuvers.Add("3.F.R", ManeuverColor.None);
                Maneuvers.Add("4.F.S", ManeuverColor.None);
                Maneuvers.Add("4.F.R", ManeuverColor.None);
                Maneuvers.Add("5.F.S", ManeuverColor.None);
                Maneuvers.Add("5.F.R", ManeuverColor.None);
            }

        }
    }
}
