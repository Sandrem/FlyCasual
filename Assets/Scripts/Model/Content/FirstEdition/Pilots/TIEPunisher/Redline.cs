using Ship;
using SubPhases;
using Tokens;

namespace Ship
{
    namespace FirstEdition.TIEPunisher
    {
        public class Redline : TIEPunisher
        {
            public Redline() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Redline\"",
                    7,
                    27,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.RedlineAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    //You may maintain 2 target locks on the same ship. When you acquire a target lock, you may acquire a second lock on that same ship.
    public class RedlineAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.TwoTargetLocksOnSameTargetsAreAllowed.Add(HostShip);
            HostShip.OnTargetLockIsAcquired += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.TwoTargetLocksOnSameTargetsAreAllowed.Remove(HostShip);
            HostShip.OnTargetLockIsAcquired -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (!IsAbilityUsed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AcquireSecondTargetLock);
            }
        }

        private void AcquireSecondTargetLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, UseAbility);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            IsAbilityUsed = true;

            BlueTargetLockToken existingToken = HostShip.Tokens.GetToken<BlueTargetLockToken>('*');
            GenericShip TargetLockTarget = existingToken.OtherTokenOwner;

            ActionsHolder.AcquireTargetLock(HostShip, TargetLockTarget, FinishAbility, FinishAbility);
        }

        private void FinishAbility()
        {
            IsAbilityUsed = false;
            Triggers.FinishTrigger();
        }
    }
}