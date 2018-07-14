using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using SubPhases;
using System;
using Tokens;

namespace Ship
{
    namespace XWing
    {
        public class TarnMison : XWing
        {
            public TarnMison() : base()
            {
                PilotName = "Tarn Mison";
                PilotSkill = 3;
                Cost = 23;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.TarnMisonAbility());
            }
        }
    }
}

namespace Abilities
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
            Actions.AcquireTargetLock(HostShip, Combat.Attacker, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);            
        }        
    }
}
