using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

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
                PilotSkill = 5;
                Cost = 15;

                IsUnique = true;

                PilotAbilities.Add(new PilotAbilitiesNamespace.NightBeastAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class NightBeastAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Host.OnMovementFinish += NightBeastPilotAbility;
        }

        private void NightBeastPilotAbility(GenericShip ship)
        {
            if (Host.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Green)
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

            Host.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}
