using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;

namespace Ship
{
    namespace TIEFighter
    {
        public class NightBeast : TIEFighter
        {
            public NightBeast() : base()
            {
                PilotName = "\"Night Beast\"";
                PilotSkill = 5;
                Cost = 15;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.NightBeastAbility());
            }
        }
    }
}

namespace Abilities
{
    public class NightBeastAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += NightBeastPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= NightBeastPilotAbility;
        }

        private void NightBeastPilotAbility(GenericShip ship)
        {
            if (Board.BoardManager.IsOffTheBoard(ship)) return;

            if (HostShip.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Green)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
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

            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}
