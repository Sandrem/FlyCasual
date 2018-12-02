using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.T70XWing
    {
        public class SnapWexley : T70XWing
        {
            public SnapWexley() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Snap\" Wexley",
                    6,
                    28,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.SnapWexleyAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    //After you execute a 2-, 3-, or 4-speed maneuver, if you are not touching a ship, you may perform a free boost action.
    public class SnapWexleyAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterSnapAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterSnapAbility;
        }

        private void RegisterSnapAbility(GenericShip hostShip)
        {
            if (Selection.ThisShip.IsBumped) return;

            int assignedSpeed = HostShip.AssignedManeuver.Speed;
            if (assignedSpeed < 2 || assignedSpeed > 4) return;

            if (BoardTools.Board.IsOffTheBoard(hostShip)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, PerformFreeBoost);
        }

        private void PerformFreeBoost(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(new List<GenericAction> { new BoostAction() }, Triggers.FinishTrigger);
        }
    }
}