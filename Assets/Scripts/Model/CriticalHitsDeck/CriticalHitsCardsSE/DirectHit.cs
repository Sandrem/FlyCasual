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
            ImageUrl = "https://i.imgur.com/aOzGkNK.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
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
            // Do nothing;
        }
    }

}