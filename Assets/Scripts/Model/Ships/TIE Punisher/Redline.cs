using ActionsList;
using RuleSets;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;

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
                Cost = 44;

                PilotAbilities.RemoveAll(a => a is Abilities.RedlineAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.RedlineAbility());

                SEImageNumber = 139;
            }
        }
    }
}

namespace Abilities
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

            Actions.AcquireTargetLock(HostShip, TargetLockTarget, FinishAbility, FinishAbility);
        }

        private void FinishAbility()
        {
            IsAbilityUsed = false;
            Triggers.FinishTrigger();
        }
    }

    namespace SecondEdition
    {
        //You can maintain 2 locks. After you perform an action, you may acquire a lock.
        public class RedlineAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                HostShip.TwoTargetLocksOnSameTargetsAreAllowed.Add(HostShip);
                HostShip.OnActionIsPerformed += RegisterAbility;
            }

            public override void DeactivateAbility()
            {
                HostShip.TwoTargetLocksOnSameTargetsAreAllowed.Remove(HostShip);
                HostShip.OnActionIsPerformed -= RegisterAbility;
            }

            private void RegisterAbility(GenericAction action)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AcquireSecondTargetLock);
            }

            private void AcquireSecondTargetLock(object sender, EventArgs e)
            {
                HostShip.ChooseTargetToAcquireTargetLock(
                    Triggers.FinishTrigger,
                    "You may acquire a lock",
                    HostShip.ImageUrl
                );
            }
        }
    }
}
