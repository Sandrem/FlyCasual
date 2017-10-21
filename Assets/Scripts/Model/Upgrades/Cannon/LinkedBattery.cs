using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;

namespace UpgradesList
{

	public class LinkedBattery : GenericSecondaryWeapon
	{
		public LinkedBattery() : base()
		{
			Type = UpgradeType.Cannon;

			Name = "Linked Battery";
            Cost = 2;

            isLimited = true;
		}

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;
        }

        public override void AttachToShip(GenericShip host)
		{
			base.AttachToShip(host);

            Host.AfterGenerateAvailableActionEffectsList += LinkedBatteryAbility;
        }

        public void LinkedBatteryAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new ActionsList.LinkedBatteryAction());
        }
    }
}

namespace ActionsList
{
    public class LinkedBatteryAction : GenericAction
    {
        public LinkedBatteryAction()
        {
            Name = EffectName = "Linked Battery";
            IsReroll = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack) {
                if (IsPrimaryWeapon() || IsCannon())
                {
                    result = true;
                }
            }
            return result;
        }

        private bool IsPrimaryWeapon()
        {
            return (Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass));
        }

        private bool IsCannon()
        {
            return ((Combat.ChosenWeapon as GenericSecondaryWeapon) != null) && ((Combat.ChosenWeapon as GenericSecondaryWeapon).Type == UpgradeType.Cannon);
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack && (Combat.ChosenWeapon as GenericSecondaryWeapon) != null)
            {
                if (Combat.DiceRollAttack.Blanks > 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0)
                {
                    result = 30;
                }
            }

            return result;
        }

    }
}