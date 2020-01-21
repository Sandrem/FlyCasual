using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LambdaClassT4AShuttle
    {
        public class ColonelJendon : LambdaClassT4AShuttle
        {
            public ColonelJendon() : base()
            {
                PilotInfo = new PilotCardInfo(
                    pilotName: "Colonel Jendon",
                    initiative: 3,
                    cost: 48,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ColonelJendonAbility),
                    charges: 2,
                    seImageNumber: 143
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ColonelJendonAbility : GenericAbility
    {
        Dictionary<GenericShip, int[]> savedTargetLockRestrictions;

        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseStart += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, AskUseColonelJendonAbility);
            }
        }

        private void AskUseColonelJendonAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                delegate (object s, EventArgs ev)
                {
                    HostShip.SpendCharge();
                    UseColonelJendonAbility(sender, e);
                },
                descriptionLong: "Do you want to spend 1 Charge? (If you do, while friendly ships acquire locks this round, they must acquire locks beyond range 3 instead of at range 0-3)",
                imageHolder: HostShip
            );
        }

        private void UseColonelJendonAbility(object sender, EventArgs e)
        {
            Phases.Events.OnEndPhaseStart_NoTriggers += RestoreTargetLockRestrictions;

            savedTargetLockRestrictions = new Dictionary<GenericShip, int[]>();
            foreach (var kv in HostShip.Owner.Ships)
            {
                GenericShip curship = kv.Value;

                int[] targetlockRange = new int[2];
                targetlockRange[0] = curship.TargetLockMinRange;
                targetlockRange[1] = curship.TargetLockMaxRange;

                savedTargetLockRestrictions.Add(curship, targetlockRange);
                curship.SetTargetLockRange(4, int.MaxValue);
            }

            DecisionSubPhase.ConfirmDecision();
        }

        private void RestoreTargetLockRestrictions()
        {
            Phases.Events.OnEndPhaseStart_NoTriggers -= RestoreTargetLockRestrictions;

            foreach (var kv in HostShip.Owner.Ships)
            {
                GenericShip curship = kv.Value;
                int min = savedTargetLockRestrictions[curship][0];
                int max = savedTargetLockRestrictions[curship][1];
                curship.SetTargetLockRange(min, max);
            }
        }
    }
}
