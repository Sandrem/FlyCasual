using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class NightBeast : TIEFighter
        {
            public NightBeast() : base()
            {
                PilotName = "\"Night Beast\"";
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/b/ba/Night_Beast.png";
                IsUnique = true;
                PilotSkill = 5;
                Cost = 15;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                OnMovementFinishWithoutColliding += NightBeastPilotAbility;
            }

            private void NightBeastPilotAbility(Ship.GenericShip ship)
            {
                if (AssignedManeuver.ColorComplexity == ManeuverColor.Green) {
                    AskPerformFreeAction(new ActionsList.FocusAction());
                }
            }
        }
    }
}
