using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardFE
{

    public class StunnedPilot : GenericDamageCard
    {
        public StunnedPilot()
        {
            Name = "Stunned Pilot";
            Type = CriticalCardType.Pilot;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnMovementFinish += RegisterCheckCollisionDamage;
            Host.Tokens.AssignCondition(new Tokens.StunnedPilotCritToken(Host));
            Triggers.FinishTrigger();
        }

        private void RegisterCheckCollisionDamage(GenericShip ship)
        {
            if (Host.IsBumped || Host.IsLandedOnObstacle)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Stunned Pilot crit",
                    TriggerType = TriggerTypes.OnMovementFinish,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = CheckCollisionDamage
                });
            }
        }

        private void CheckCollisionDamage(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Stunned Pilot: Ship suffered damage");

            Selection.ThisShip.AssignedDamageDiceroll.AddDice(DieSide.Success);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = Selection.ThisShip.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = "Critical hit card",
                    DamageType = DamageTypes.CriticalHitCard
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Triggers.FinishTrigger);
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.OnMovementFinish -= RegisterCheckCollisionDamage;
            Host.Tokens.RemoveCondition(typeof(Tokens.StunnedPilotCritToken));

            Host.AfterAttackWindow -= DiscardEffect;
        }

    }

}

