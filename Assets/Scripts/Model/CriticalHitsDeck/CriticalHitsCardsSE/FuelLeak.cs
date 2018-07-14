﻿using System;
using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;

namespace DamageDeckCardSE
{

    public class FuelLeak : GenericDamageCard
    {
        public FuelLeak()
        {
            Name = "Fuel Leak";
            Type = CriticalCardType.Ship;
            ImageUrl = "https://i.imgur.com/MNNWJJe.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnGenerateActions += CallAddCancelCritAction;
            Host.OnSufferDamageConfirmed += CheckToSufferAdditionalDamageAndRepair;
            Host.Tokens.AssignCondition(typeof(Tokens.FuelLeakCritToken));
            Triggers.FinishTrigger();
        }
                

        private void CheckToSufferAdditionalDamageAndRepair(GenericShip ship, bool isCritical)
        {
            if (isCritical)
            {
                Messages.ShowInfo("Fuel Leak: Suffer 1 additional damage");

                Host.AssignedDamageDiceroll.AddDice(DieSide.Success);

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Suffer damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = Host.SufferDamage,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = "Critical hit card",
                        DamageType = DamageTypes.CriticalHitCard
                    }
                });

                Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, RepairFuelLeak);
            }
        }

        public void RepairFuelLeak()
        {
            DiscardEffect();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.OnGenerateActions -= CallAddCancelCritAction;
            Host.OnSufferDamageConfirmed -= CheckToSufferAdditionalDamageAndRepair;
            Host.Tokens.RemoveCondition(typeof(Tokens.FuelLeakCritToken));
        }
    }

}