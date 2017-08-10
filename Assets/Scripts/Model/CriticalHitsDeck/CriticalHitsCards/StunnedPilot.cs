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
            ImageUrl = "http://i.imgur.com/J9knseg.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Messages.ShowInfo("After you execute a maneuver, if you are touching another ship or overlapping an obstacle token, suffer 1 damage");
            Game.UI.AddTestLogEntry("After you execute a maneuver, if you are touching another ship or overlapping an obstacle token, suffer 1 damage");

            Host.OnMovementFinish += CheckCollisionDamage;
            Host.AssignToken(new Tokens.StunnedPilotCritToken());

            Triggers.FinishTrigger();
        }

        private void CheckCollisionDamage(Ship.GenericShip host)
        {
            if (host.IsBumped || host.IsLandedOnObstacle)
            {
                Messages.ShowInfo("Stunned Pilot: Ship suffered damage");
                Game.UI.AddTestLogEntry("Stunned Pilot: Ship suffered damage");

                Selection.ThisShip.AssignedDamageDiceroll.DiceList.Add(new Dice(DiceKind.Attack, DiceSide.Success));

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Suffer damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    EventHandler = Selection.ThisShip.SufferDamage
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

