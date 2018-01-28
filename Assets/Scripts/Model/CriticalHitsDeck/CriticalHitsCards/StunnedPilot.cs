using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class StunnedPilot : GenericCriticalHit
    {
        public StunnedPilot()
        {
            Name = "Stunned Pilot";
            Type = CriticalCardType.Pilot;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnMovementFinish += RegisterCheckCollisionDamage;
            Host.AssignToken(new Tokens.StunnedPilotCritToken(), Triggers.FinishTrigger);
        }

        private void RegisterCheckCollisionDamage(Ship.GenericShip host)
        {
            if (host.IsBumped || host.IsLandedOnObstacle)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Stunned Pilot crit",
                    TriggerType = TriggerTypes.OnShipMovementFinish,
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
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

        public override void DiscardEffect(Ship.GenericShip host)
        {
            host.OnMovementFinish -= RegisterCheckCollisionDamage;
            host.RemoveCondition(typeof(Tokens.StunnedPilotCritToken));

            host.AfterAttackWindow -= DiscardEffect;
        }

    }

}

