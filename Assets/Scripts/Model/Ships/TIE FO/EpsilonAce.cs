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
                ImageUrl   = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-fo%20Fighter/epsilon-ace.png";
                PilotSkill = TruePilotSkill;
                Cost       = 17;
                IsUnique   = true;

                PilotAbilities.Add(new PilotAbilitiesNamespace.EpsilonAceStartAbility());
                PilotAbilities.Add(new PilotAbilitiesNamespace.EpsilonAceEndAbility());
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
    public class EpsilonAceStartAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Phases.OnGameStart += RegisterEpsilonAceStartAbility;
        }
        private void RegisterEpsilonAceStartAbility ()
        {
            RegisterAbilityTrigger (TriggerTypes.OnGameStart, UseEpsilonAceStartAbility);
        }
        private void UseEpsilonAceStartAbility(object sender, System.EventArgs e)
        {
            //psModifier = (IModifyPilotSkill)this.Host;
            //Host.AddPilotSkillModifier (psModifier);
            Host.AddPilotSkillModifier ((IModifyPilotSkill)Host);
            Triggers.FinishTrigger ();
        }
    }

    public class EpsilonAceEndAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Host.OnDamageCardIsDealt += RegisterEpsilonAceAbility;
        }
        private void RegisterEpsilonAceAbility (GenericShip ship)
        {
            RegisterAbilityTrigger (TriggerTypes.OnDamageCardIsDealt, UseEpsilonAceAbility);
        }

        private void UseEpsilonAceAbility(object sender, System.EventArgs e)
        {
            if (Combat.CurrentCriticalHitCard != null) {
                Host.RemovePilotSkillModifier ((IModifyPilotSkill)Host);
            }
            Triggers.FinishTrigger ();
        }
    }
}