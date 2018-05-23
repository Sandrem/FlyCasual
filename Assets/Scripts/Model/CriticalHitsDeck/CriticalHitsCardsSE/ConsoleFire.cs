using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardSE
{
    public class ConsoleFire : DamageDeckCardFE.ConsoleFire
    {
        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnCombatActivation += PlanRollForDamage;            
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;
            Host.OnShipIsDestroyed += DiscardEffect;

            Host.Tokens.AssignCondition(new Tokens.ConsoleFireCritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            Host.Tokens.RemoveCondition(typeof(Tokens.ConsoleFireCritToken));

            Host.OnCombatActivation -= PlanRollForDamage;
            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
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

