using RuleSets;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace TIEPunisher
    {
        public class Redline : TIEPunisher, ISecondEditionPilot
        {
            public Redline() : base()
            {
                PilotName = "\"Redline\"";
                PilotSkill = 7;
                Cost = 27;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.RedlineAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
            }
        }
    }
}

namespace Abilities
{
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

            Actions.AcquireTargetLock(HostShip, TargetLockTarget, FinishAbility, FinishAbility);
        }

        private void FinishAbility()
        {
            IsAbilityUsed = false;
            Triggers.FinishTrigger();
        }
    }
}