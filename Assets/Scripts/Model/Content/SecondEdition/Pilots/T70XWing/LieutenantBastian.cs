using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class LieutenantBastian : T70XWing
        {
            public LieutenantBastian() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant Bastian",
                    2,
                    48,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantBastianAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/3f43d6b3c6e87bde6a681e9d4421dec8.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantBastianAbility : GenericAbility
    {
        private GenericShip ShipToAssignLock;

        public override void ActivateAbility()
        {
            GenericShip.OnDamageCardIsDealtGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDamageCardIsDealtGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (distInfo.Range == 1 || distInfo.Range == 2)
            {
                ShipToAssignLock = ship;
                RegisterAbilityTrigger(TriggerTypes.OnDamageCardIsDealt, AskAcquireTargetLock);
            }
        }

        private void AskAcquireTargetLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                ShouldAbilityBeUsed,
                AcquireTargetLock,
                infoText: HostShip.PilotInfo.PilotName + ": Acquire a Lock on " + ShipToAssignLock.PilotInfo.PilotName + "?"
            );
        }

        private bool ShouldAbilityBeUsed()
        {
            return (!HostShip.Tokens.HasToken<BlueTargetLockToken>(letter: '*'));
        }

        private void AcquireTargetLock(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " acquired a Lock on " + ShipToAssignLock.PilotInfo.PilotName);
            ActionsHolder.AcquireTargetLock(HostShip, ShipToAssignLock, Triggers.FinishTrigger, Triggers.FinishTrigger);
        }
    }
}