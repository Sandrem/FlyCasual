using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class XWing : GenericShip
        {

            public XWing(Players.PlayerNo playerNo, int shipId, Vector3 position) : base(playerNo, shipId, position)
            {
                Type = "X-Wing";

                Firepower = 3;
                Agility = 2;
                MaxHull = 3;
                MaxShields = 2;

                BuiltInActions.Add(new ActionsList.FocusAction());
                BuiltInActions.Add(new ActionsList.TargetLockAction());
                BuiltInActions.Add(new ActionsList.BarrelRollAction());

                AddUpgradeSlot(Upgrade.UpgradeSlot.Astromech);
                AddUpgradeSlot(Upgrade.UpgradeSlot.Torpedoes);

                AssignTemporaryManeuvers();
                InitializeShip();
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
                Maneuvers.Add("3.F.R", ManeuverColor.None);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
                Maneuvers.Add("5.F.S", ManeuverColor.None);
                Maneuvers.Add("5.F.R", ManeuverColor.None);
            }

        }
    }
}
