using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

namespace Ship
{
    namespace Firespray31
    {
        public class Firespray31 : GenericShip
        {

            public Firespray31() : base()
            {
                Type = "Firespray-31";
                ShipBaseSize = BaseSize.Large;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/4/4e/Firespray_31_Move.png";

                Firepower = 3;
                Agility = 2;
                MaxHull = 6;
                MaxShields = 4;

                AddUpgradeSlot(Upgrade.UpgradeType.Cannon);
                AddUpgradeSlot(Upgrade.UpgradeType.Bomb);
                AddUpgradeSlot(Upgrade.UpgradeType.Crew);
                AddUpgradeSlot(Upgrade.UpgradeType.Missiles);

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.XWingTable();

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
                BuiltInActions.Add(new ActionsList.EvadeAction());
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
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("3.F.R", ManeuverColor.Red);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
                Maneuvers.Add("5.F.S", ManeuverColor.None);
                Maneuvers.Add("5.F.R", ManeuverColor.None);
            }

        }
    }
}
