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

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("After you execute a maneuver, if you are touching another ship or overlapping an obstacle token, suffer 1 damage");
            Game.UI.AddTestLogEntry("After you execute a maneuver, if you are touching another ship or overlapping an obstacle token, suffer 1 damage");

            host.OnMovementFinish += CheckCollisionDamage;
            host.AssignToken(new Tokens.StunnedPilotCritToken());
        }

        private void CheckCollisionDamage(Ship.GenericShip host)
        {
            if (host.IsBumped || host.IsLandedOnObstacle)
            {
                Game.UI.ShowError("Stunned Pilot: Ship suffered damage");
                Game.UI.AddTestLogEntry("Stunned Pilot: Ship suffered damage");

                DiceRoll damageRoll = new DiceRoll(DiceKind.Attack, 0);
                damageRoll.DiceList.Add(new Dice(DiceKind.Attack, DiceSide.Success));

                /*DamageSourceEventArgs eventArgs = new DamageSourceEventArgs();
                eventArgs.Source = this;
                eventArgs.DamageType = DamageTypes.CriticalHitCard;*/

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Suffer regular damage",
                    triggerType = TriggerTypes.OnRegularDamageIsDealt,
                    TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                    eventHandler = Selection.ActiveShip.SufferRegularDamage
                });

                SufferRegularDamage(SufferCriticalDamage);
            }
        }

        private static void SufferRegularDamage(System.Action callBack)
        {
            Triggers.ResolveTriggersByType(TriggerTypes.OnRegularDamageIsDealt, callBack);
        }

        private static void SufferCriticalDamage()
        {
            Triggers.ResolveTriggersByType(TriggerTypes.OnCriticalDamageIsDealt);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            host.OnMovementFinish -= CheckCollisionDamage;
            host.RemoveToken(typeof(Tokens.StunnedPilotCritToken));

            host.AfterAttackWindow -= DiscardEffect;
        }

    }

}

