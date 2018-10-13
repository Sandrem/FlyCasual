using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardSE
{
    public class ConsoleFire : DamageDeckCardFE.ConsoleFire
    {
        public ConsoleFire()
        {
            ImageUrl = "https://i.imgur.com/x4a6fqE.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnCombatActivation += PlanRollForDamage;            
            Host.OnGenerateActions += CallAddCancelCritAction;
            Host.OnShipIsDestroyed += DiscardEffect;

            Host.Tokens.AssignCondition(typeof(Tokens.ConsoleFireSECritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.Tokens.RemoveCondition(typeof(Tokens.ConsoleFireSECritToken));

            Host.OnCombatActivation -= PlanRollForDamage;
            Host.OnGenerateActions -= CallAddCancelCritAction;
            Host.OnShipIsDestroyed -= DiscardEffect;
        }

        private void DiscardEffect(GenericShip ship, bool flag)
        {
            DiscardEffect();
        }

        protected override void PlanRollForDamage()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "#" + Host.ShipId + ": Console Fire Crit",
                TriggerOwner = Host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnCombatActivation,
                EventHandler = RollForDamage
            });
        }

        private void PlanRollForDamage(GenericShip ship)
        {
            PlanRollForDamage();
        }


    }

}

