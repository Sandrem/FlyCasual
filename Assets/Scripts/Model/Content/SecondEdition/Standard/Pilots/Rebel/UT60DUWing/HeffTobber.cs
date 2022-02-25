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
                PilotInfo = new PilotCardInfo25
                (
                    "Heff Tobber",
                    "Blue Eight",
                    Faction.Rebel,
                    2,
                    5,
                    9,
                    isLimited: true,
                    abilityType: typeof(HeffTobberAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
                    seImageNumber: 59,
                    skinName: "Blue"
                );
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
                Name = HostShip.PilotInfo.PilotName,
                TriggerType = TriggerTypes.OnMovementFinish,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = UseHeffTobberAbility
            });
        }

        private void UseHeffTobberAbility(object sender, EventArgs e)
        {
            GenericShip previousActiveShip = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);
            List<GenericAction> actions = HostShip.GetAvailableActions();

            HostShip.AskPerformFreeAction(
                actions,
                delegate {
                    Selection.ChangeActiveShip(previousActiveShip);
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "After an enemy executes a maneuver, if it is at range 0, you may perform an action",
                HostShip
            );
        }
    }
}
