using Abilities.SecondEdition;
using ActionsList;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class HeffTobber : UT60DUWing
        {
            public HeffTobber() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Heff Tobber",
                    2,
                    45,
                    isLimited: true,
                    abilityType: typeof(HeffTobberAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 59
                );

                ModelInfo.SkinName = "Blue";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HeffTobberAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementBumped += RegisterHeffTobberAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementBumped -= RegisterHeffTobberAbility;
        }

        private void RegisterHeffTobberAbility(GenericShip ship)
        {
            if (ship.Owner.Id == HostShip.Owner.Id)
                return;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Heff Tobber's ability",
                TriggerType = TriggerTypes.OnMovementFinish,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = UseHeffTobberAbility
            });
        }

        private void UseHeffTobberAbility(object sender, EventArgs e)
        {
            Messages.ShowInfo("Heff Tobber can perform a free action");

            GenericShip previousActiveShip = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);
            List<GenericAction> actions = HostShip.GetAvailableActions();

            HostShip.AskPerformFreeAction(
                actions,
                delegate {
                    Selection.ChangeActiveShip(previousActiveShip);
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}
