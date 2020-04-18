using Abilities.FirstEdition;
using Ship;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class NightBeast : TIEFighter
        {
            public NightBeast() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Night Beast",
                    4,
                    14,
                    isLimited: true,
                    abilityType: typeof(NightBeastAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class NightBeastAbility : GenericAbility
    {
        protected virtual string AbilityDescription => "After you execute a green maneuver, you may perform a Focus action";

        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += NightBeastPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= NightBeastPilotAbility;
        }

        protected void NightBeastPilotAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (HostShip.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = HostShip.PilotInfo.PilotName + ": Free Focus action",
                        TriggerOwner = ship.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnMovementFinish,
                        EventHandler = PerformFreeFocusAction
                    }
                );
            }
        }

        private void PerformFreeFocusAction(object sender, System.EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new ActionsList.FocusAction(),
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                AbilityDescription,
                HostShip
            );
        }
    }
}
