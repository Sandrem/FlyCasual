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
                PilotSkill = 12;
                Cost       = 17;
                IsUnique   = true;
                PilotAbilities.Add(new PilotAbilitiesNamespace.EpsilonAce());
            }
            public void ModifyPilotSkill(ref int pilotSkill)
            {
                this.PilotSkill = this.TruePilotSkill;
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class EpsilonAce : GenericPilotAbility
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
            if (Combat.Defender == Host && Host.Hull != Host.MaxHull){
                // lose the pilot skill
                Host.AddPilotSkillModifier((IModifyPilotSkill)Host);
                Triggers.FinishTrigger ();
            }
        }
    }
}