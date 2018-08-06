using Abilities.SecondEdition;
using ActionsList;
using RuleSets;
using Ship;
using System;
using System.Collections.Generic;

namespace Ship
{
    namespace UWing
    {
        public class HeffTobber : UWing, ISecondEditionPilot
        {
            public HeffTobber() : base()
            {
                PilotName = "Heff Tobber";
                PilotSkill = 2;
                Cost = 45;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new HeffTobberAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // not needed
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HeffTobberAbilitySE : GenericAbility
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
            Messages.ShowInfo("Heff Tobber can perform free action");

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