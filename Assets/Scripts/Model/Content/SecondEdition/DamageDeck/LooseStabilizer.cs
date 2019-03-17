using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardSE
{

    public class LooseStabilizer : GenericDamageCard
    {
        public LooseStabilizer()
        {
            Name = "Loose Stabilizer";
            Type = CriticalCardType.Ship;
            ImageUrl = "https://i.imgur.com/bgNok6v.png";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnMovementFinish += PlanDamageAfterNonStraightManeuvers;
            Host.OnGenerateActions += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(typeof(Tokens.LooseStabilizerSECritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("The Loose Stabilizer has been locked down.  " + Host.PilotInfo.PilotName + " no longer takes damage after performing non-straight maneuvers.");
           
            Host.Tokens.RemoveCondition(typeof(Tokens.LooseStabilizerSECritToken));
            Host.OnMovementFinish -= PlanDamageAfterNonStraightManeuvers;
            Host.OnGenerateActions -= CallAddCancelCritAction;
        }

        private void PlanDamageAfterNonStraightManeuvers(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Loose Stabilizer",
                TriggerType = TriggerTypes.OnMovementFinish,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = SufferDamage
            });
        }

        private void SufferDamage(object sender, System.EventArgs e)
        {
            if (Host.GetLastManeuverBearing() != Movement.ManeuverBearing.Straight)
            {
                Messages.ShowInfo("A Loose Stabilizer causes " + Host.PilotInfo.PilotName + " to suffer 1 damage because they performed a non-straight maneuver.");

                DamageSourceEventArgs looseDamage = new DamageSourceEventArgs()
                {
                    Source = "Critical hit card",
                    DamageType = DamageTypes.CriticalHitCard
                };

                Host.Damage.TryResolveDamage(1, looseDamage, RepairLooseStabilizer);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        public void RepairLooseStabilizer()
        {
            DiscardEffect();
            Triggers.FinishTrigger();
        }    
    }
}