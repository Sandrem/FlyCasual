using Ship;
using SubPhases;
using System;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.LambdaClassT4AShuttle
    {
        public class ColonelJendon : LambdaClassT4AShuttle
        {
            public ColonelJendon() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Colonel Jendon",
                    3,
                    46,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.ColonelJendonAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 143;
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
            AskToUseAbility(NeverUseByDefault, delegate (object s, EventArgs ev)
            {
                HostShip.SpendCharge();
                UseColonelJendonAbility(sender, e);
            });
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
