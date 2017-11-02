using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFO
    {
        public class EpsilonAce : TIEFO,IModifyPilotSkill
        {
            private int TruePilotSkill = 4;
            private int PilotAbilitySkill = 12;
            public EpsilonAce () : base ()
            {
                PilotName = "Epsilon Ace";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-fo%20Fighter/epsilon-ace.png";
                IsUnique = true;
                PilotSkill = 4;
                Cost = 17;
            }

            public override void InitializePilot ()
            {
                base.InitializePilot ();

                // Set Pilot Skill for Pilot Ability
                PilotSkill = PilotAbilitySkill;

                OnDamageCardIsDealt += RegisterEpsilonAceAbility;
            }

            private void RegisterEpsilonAceAbility (GenericShip ship)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Epsilon Ace Ability",
                    TriggerOwner = this.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnDamageCardIsDealt,
                    EventHandler = UseEpsilonAceAbility
                });
            }

            private void UseEpsilonAceAbility(object sender, System.EventArgs e)
            {
                if (Combat.Defender == this && this.Hull != this.MaxHull){
                    // lose the pilot skill
                    this.AddPilotSkillModifier(this);
                    // remove
                    OnDamageCardIsDealt -= RegisterEpsilonAceAbility;
                    Triggers.FinishTrigger ();
                }
            }

            public void ModifyPilotSkill(ref int pilotSkill)
            {
                pilotSkill += this.TruePilotSkill;
            }
        }
    }
}