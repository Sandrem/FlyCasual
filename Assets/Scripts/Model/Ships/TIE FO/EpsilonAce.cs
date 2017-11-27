using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace Ship
{
    namespace TIEFO
    {
        public class EpsilonAce : TIEFO,IModifyPilotSkill
        {
            private int TruePilotSkill = 4;

            public EpsilonAce () : base ()
            {
                PilotName  = "Epsilon Ace";
                PilotSkill = TruePilotSkill;
                Cost       = 17;
                IsUnique   = true;

                PilotAbilities.Add(new PilotAbilitiesNamespace.EpsilonAceAbility());
            }
            public void ModifyPilotSkill(ref int pilotSkill)
            {
                pilotSkill = 12;
            }
        }
    }
}


namespace PilotAbilitiesNamespace
{
    public class EpsilonAceAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Phases.OnGameStart       += RegisterEpsilonAceStartAbility;
            Host.OnDamageCardIsDealt += RegisterEpsilonAceEndAbility;
        }
        private void RegisterEpsilonAceStartAbility ()
        {
            RegisterAbilityTrigger (TriggerTypes.OnGameStart, UseEpsilonAceStartAbility);
        }
        private void RegisterEpsilonAceEndAbility (GenericShip ship)
        {
            RegisterAbilityTrigger (TriggerTypes.OnDamageCardIsDealt, UseEpsilonAceEndAbility);
        }
        private void UseEpsilonAceStartAbility(object sender, System.EventArgs e)
        {
            Host.AddPilotSkillModifier ((IModifyPilotSkill)Host);
            Triggers.FinishTrigger ();
        }
        private void UseEpsilonAceEndAbility(object sender, System.EventArgs e)
        {
            if (Combat.CurrentCriticalHitCard != null) {
                Host.RemovePilotSkillModifier ((IModifyPilotSkill)Host);
            }
            Triggers.FinishTrigger ();
        }
    }
}