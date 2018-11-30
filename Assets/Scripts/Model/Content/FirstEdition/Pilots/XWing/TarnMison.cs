using System;
using System.Collections;
using System.Collections.Generic;
using SubPhases;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class TarnMison : XWing
        {
            public TarnMison() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tarn Mison",
                    3,
                    23,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TarnMisonAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class TarnMisonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsDefender += RegisterTarnMisonPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsDefender -= RegisterTarnMisonPilotAbility;
        }

        private void RegisterTarnMisonPilotAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskToUseTarnMisonAbility);
        }

        private void AskToUseTarnMisonAbility(object sender, EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, UseTarnMisonAbility);
        }

        private void UseTarnMisonAbility(object sender, EventArgs e)
        {
             ActionsHolder.AcquireTargetLock(HostShip, Combat.Attacker, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }
    }
}
