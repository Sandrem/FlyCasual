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
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/damage-decks/core-tfa/stunned-pilot.png";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnMovementFinish += CheckCollisionDamage;
            Host.AssignToken(new Tokens.StunnedPilotCritToken(), Triggers.FinishTrigger);
        }

        private void CheckCollisionDamage(Ship.GenericShip host)
        {
            if (host.IsBumped || host.IsLandedOnObstacle)
            {
                Messages.ShowInfo("Stunned Pilot: Ship suffered damage");

                Selection.ThisShip.AssignedDamageDiceroll.AddDice(DiceSide.Success);

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

                Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, delegate { });
            }
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            host.OnMovementFinish -= CheckCollisionDamage;
            host.RemoveToken(typeof(Tokens.StunnedPilotCritToken));

            host.AfterAttackWindow -= DiscardEffect;
        }

    }

}

