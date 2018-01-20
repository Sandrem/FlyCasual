﻿using Ship;
using Upgrade;

namespace UpgradesList
{
    public class SensorJammer : GenericUpgrade
    {
        public SensorJammer() : base()
        {
            Type = UpgradeType.System;
            Name = "Sensor Jammer";
            Cost = 4;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Large;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableOppositeActionEffectsList += SensorJammerActionEffect;
        }

        private void SensorJammerActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SensorJammerActionEffect()
            {
                ImageUrl = ImageUrl,
                Host = host
            };
            host.AddAvailableOppositeActionEffect(newAction);
        }
    }
}

namespace ActionsList
{
    public class SensorJammerActionEffect : GenericAction
    {

        public SensorJammerActionEffect()
        {
            Name = EffectName = "Sensor Jammer";
            IsOpposite = true;
        }
        
        public override int GetActionEffectPriority()
        {
            int result = 0;

            result = 100;
            
            return result;            
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack && Combat.DiceRollAttack.RegularSuccesses > 0)
            {
                result = true;
            }

            return result;
        }
        
        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollAttack.ChangeOne(DieSide.Success, DieSide.Focus, true);
            callBack();
        }

    }

}


