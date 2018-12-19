using BoardTools;
using Ship;
using SubPhases;
using System;
using Tokens;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class LieutenantRivas : TIEFoFighter
        {
            public LieutenantRivas() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant Rivas",
                    1,
                    30,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantRivasAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/7188ec2eb699261dbd47a15df6164f4c.png";
            }
        }
    }
}


namespace Abilities.SecondEdition
{
    public class LieutenantRivasAbility : GenericAbility
    {
        private GenericShip ShipWithAssignedToken;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, Type tokenType)
        {
            // To avoid infinite loop
            if (IsAbilityUsed) return;

            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) return;

            if (ActionsHolder.HasTargetLockOn(HostShip, ship)) return;

            TokenColors tokenColor = TokensManager.GetTokenColorByType(tokenType);
            if (tokenColor != TokenColors.Red && tokenColor != TokenColors.Orange) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (distInfo.Range == 1 || distInfo.Range == 2)
            {
                ShipWithAssignedToken = ship;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskAcquireTargetLock);
            }
        }

        private void AskAcquireTargetLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                AlwaysUseByDefault,
                AcquireTargetLock,
                infoText: HostShip.PilotInfo.PilotName + ": Acquire a Lock on " + ShipWithAssignedToken.PilotInfo.PilotName + "?"
            );
        }

        private void AcquireTargetLock(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            IsAbilityUsed = true;
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " acquired a Lock on " + ShipWithAssignedToken.PilotInfo.PilotName);
            ActionsHolder.AcquireTargetLock(HostShip, ShipWithAssignedToken, FinishAbility, FinishAbility);
        }

        private void FinishAbility()
        {
            IsAbilityUsed = false;
            Triggers.FinishTrigger();
        }
    }
}