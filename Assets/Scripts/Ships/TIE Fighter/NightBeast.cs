using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class NightBeast : TIEFighter
        {
            public NightBeast(Players.GenericPlayer owner, int shipId, Vector3 position) : base(owner, shipId, position)
            {
                PilotName = "\"Night Beast\"";
                isUnique = true;
                PilotSkill = 5;

                OnMovementFinishWithoutColliding += NightBeastPilotAbility;
            }

            private void NightBeastPilotAbility(Ship.GenericShip ship)
            {
                if (AssignedManeuver.ColorComplexity == ManeuverColor.Green) {
                    AskPerformFreeAction(new Actions.FocusAction());
                }
            }
        }
    }
}
