using System;
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
            Host.OnDamageWasSuccessfullyDealt += CheckToSufferAdditionalDamageAndRepair;
            Host.Tokens.AssignCondition(typeof(Tokens.FuelLeakCritToken));
            Triggers.FinishTrigger();
        }
                

        private void CheckToSufferAdditionalDamageAndRepair(GenericShip ship, bool isCritical)
        {
            if (isCritical)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Fuel Leak",
                    TriggerType = TriggerTypes.OnDamageWasSuccessfullyDealt,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = SufferAdditonalDamage,
                });
            }
        }

        private void SufferAdditonalDamage(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Fuel Leak: Suffer 1 additional damage");

            DamageSourceEventArgs fuelleakDamage = new DamageSourceEventArgs()
            {
                Source = "Critical hit card",
                DamageType = DamageTypes.CriticalHitCard
            };

            Host.Damage.TryResolveDamage(1, fuelleakDamage, RepairFuelLeak);
        }

        private void RepairFuelLeak()
        {
            DiscardEffect();
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.OnGenerateActions -= CallAddCancelCritAction;
            Host.OnDamageWasSuccessfullyDealt -= CheckToSufferAdditionalDamageAndRepair;
            Host.Tokens.RemoveCondition(typeof(Tokens.FuelLeakCritToken));
        }
    }

}