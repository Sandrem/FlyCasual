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
            ImageUrl = "https://i.imgur.com/PMqkGn3.png";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnMovementFinish += PlanDamageAfterNonStraightManeuvers;
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(new Tokens.LooseStabilizerSECritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("No damage after non-straight maneuvers");
            Host.Tokens.RemoveCondition(typeof(Tokens.LooseStabilizerSECritToken));
            Host.OnMovementFinish -= PlanDamageAfterNonStraightManeuvers;
            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
        }

        private void PlanDamageAfterNonStraightManeuvers(GenericShip ship)
        {
            if (Host.GetLastManeuverBearing() != Movement.ManeuverBearing.Straight)
            {
                Messages.ShowInfo("Loose Stabilizer: Suffer 1 damage on non-straight maneuver");

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

                Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, RepairLooseStabilizer);
            }
        }

        public void RepairLooseStabilizer()
        {
            DiscardEffect();
            Triggers.FinishTrigger();
        }    
    }
}