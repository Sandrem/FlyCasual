using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace DamageDeckCardSE
{
    public class DisabledPowerRegulator : GenericDamageCard
    {
        public DisabledPowerRegulator()
        {
            Name = "Disabled Power Regulator";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnCombatActivation += SufferIon;
            Host.OnShipIsDestroyed += DiscardEffect;
            Host.OnMovementFinish += RemoveCritOnIonManeuver;
            Host.Tokens.AssignCondition(new Tokens.DisabledPowerRegulatorCritToken(Host));            
            Triggers.FinishTrigger();
        }

        private void RemoveCritOnIonManeuver(GenericShip ship)
        {
            if (ship.AssignedManeuver.IsIonManeuver)
            {
                DiscardEffect();
            }
        }

        private void SufferIon(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Get Ion assigned",
                TriggerType = TriggerTypes.OnTokenIsAssigned,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = GetIon
            });

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsAssigned, () => { });
        }

        public void GetIon(object sender, EventArgs e)
        {
            Host.Tokens.AssignToken(new IonToken(Host), Triggers.FinishTrigger);
        }

        public override void DiscardEffect()
        {
            Host.OnCombatActivation -= SufferIon;
            Host.OnShipIsDestroyed -= DiscardEffect;
            Host.OnMovementFinish -= RemoveCritOnIonManeuver;
            Host.Tokens.RemoveCondition(typeof(Tokens.DisabledPowerRegulatorCritToken));
        }

        private void DiscardEffect(GenericShip ship, bool flag)
        {
            DiscardEffect();
        }

    }

}

