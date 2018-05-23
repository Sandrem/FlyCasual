using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardSE
{

    public class DirectHit : GenericDamageCard
    {
        public DirectHit()
        {
            Name = "Direct Hit";
            Type = CriticalCardType.Ship;
            AiAvoids = true;                        
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.Tokens.AssignCondition(new Tokens.DirectHitCritToken(Host));
            SufferAdditionalDamage();
        }

        private void SufferAdditionalDamage()
        {
            Messages.ShowInfo("Direct Hit: Suffer 1 additional damage");

            Host.AssignedDamageDiceroll.AddDice(DieSide.Success);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer critical damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = Host.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = "Critical hit card",
                    DamageType = DamageTypes.CriticalHitCard
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, RepairDirectHit);
        }

        public void RepairDirectHit()
        {
            DiscardEffect();
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            Host.Tokens.RemoveCondition(typeof(Tokens.DirectHitCritToken));
        }
    }

}