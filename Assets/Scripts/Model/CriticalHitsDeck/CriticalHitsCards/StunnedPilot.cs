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

            host.OnMovementFinishWithColliding += DoCollisionDamage;
            host.OnLandedOnObstacle += DoCollisionDamage;
            host.AssignToken(new Tokens.StunnedPilotCritToken());
        }

        private void DoCollisionDamage(Ship.GenericShip host)
        {
            Game.UI.ShowError("Stunned Pilot: Ship suffered damage");
            Game.UI.AddTestLogEntry("Stunned Pilot: Ship suffered damage");

            DiceRoll damageRoll = new DiceRoll("attack", 0);
            damageRoll.DiceList.Add(new Dice("attack", DiceSide.Success));

            DamageSourceEventArgs eventArgs = new DamageSourceEventArgs();
            eventArgs.Source = this;
            eventArgs.DamageType = DamageTypes.CriticalHitCard;

            host.SufferDamage(damageRoll, eventArgs);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            host.OnMovementFinishWithColliding -= DoCollisionDamage;
            host.OnLandedOnObstacle -= DoCollisionDamage;
            host.RemoveToken(typeof(Tokens.StunnedPilotCritToken));

            host.AfterAttackWindow -= DiscardEffect;
        }

    }

}

