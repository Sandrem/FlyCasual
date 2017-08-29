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
                OnMovementFinish += NightBeastPilotAbility;
            }

            private void NightBeastPilotAbility(GenericShip ship)
            {
                if (AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Green)
                {
                    Triggers.RegisterTrigger(
                        new Trigger() {
                            Name = "Night Beast: Free Focus action",
                            TriggerOwner = ship.Owner.PlayerNo,
                            TriggerType = TriggerTypes.OnShipMovementFinish,
                            EventHandler = PerformFreeFocusAction
                        }
                    );
                }
            }

            private void PerformFreeFocusAction(object sender, System.EventArgs e)
            {
                List<ActionsList.GenericAction> actions = new List<ActionsList.GenericAction>() { new ActionsList.FocusAction() };

                AskPerformFreeAction(actions, Triggers.FinishTrigger);
            }
        }
    }
}
