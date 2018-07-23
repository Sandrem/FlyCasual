using System;
using Ship;
using Upgrade;
using Abilities;
using Tokens;
using SubPhases;

namespace UpgradesList
{
	public class MultiSpectralCamouflage : GenericUpgrade
    {
		public MultiSpectralCamouflage() : base()
        {
            Types.Add(UpgradeType.Modification);
			Name = "Multi-spectral Camouflage";
			Cost = 1;

            UpgradeAbilities.Add(new MultiSpectralCamouflageAbility());
		}
    }
}

namespace Abilities
{
    public class MultiSpectralCamouflageAbility : GenericAbility
    {

        GenericShip TargetingShip;
        char TargetLockLetter;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += RegisterMultiSpectralCamouflageAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += RegisterMultiSpectralCamouflageAbility;
        }

        private void RegisterMultiSpectralCamouflageAbility(GenericShip ship, Type tokenType)
        {
            // Trigger only when a ship receives a BlueTargetLockToken
            if (tokenType != typeof(BlueTargetLockToken)) return;

            BlueTargetLockToken assignedBlueLock = (BlueTargetLockToken)ship.Tokens.GetToken(typeof(BlueTargetLockToken), '*');
            TargetLockLetter = assignedBlueLock.Letter;

            // Make sure the targeted ship is the HostShip
            if (HostShip.Tokens.GetToken(typeof(RedTargetLockToken), TargetLockLetter) == null) return;
         
            // Make sure Host Ship only has one red target lock
            int redTargetLockCount = 0;
            foreach (GenericToken token in HostShip.Tokens.GetAllTokens())
            {
                if (token is RedTargetLockToken)
                {
                    redTargetLockCount += 1;
                }
            }

            TargetingShip = ship;

            if (redTargetLockCount == 1)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskToUseMultiSpectralCamouflageAbility);
            }
        }

        private void AskToUseMultiSpectralCamouflageAbility(object sender, EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, StartDiceRollSubphase);
        }

        private void StartDiceRollSubphase(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Selection.ActiveShip = HostShip;
            Selection.ThisShip = HostShip;

            PerformDiceCheck(
                "Multi-Spectral Camouflage",
                DiceKind.Defence,
                1,
                FinishMultiSpectralCamouflage,
                Triggers.FinishTrigger
            );
        }

        private void CleanUpMultiSpectralCamouflage()
        {
            Selection.ActiveShip = TargetingShip;
            Selection.ThisShip = TargetingShip;
            AbilityDiceCheck.ConfirmCheck();
        }

        private void FinishMultiSpectralCamouflage()
        {
            if (DiceCheckRoll.RegularSuccesses > 0)
            {
                HostShip.Tokens.RemoveToken(
                    typeof(RedTargetLockToken),
                    delegate {
                        TargetingShip.Tokens.RemoveToken(
                            typeof(BlueTargetLockToken),
                            CleanUpMultiSpectralCamouflage,
                            TargetLockLetter
                        );
                    },
                    TargetLockLetter
                );
            }
            else
            {
                CleanUpMultiSpectralCamouflage();
            }
        }

    }
}