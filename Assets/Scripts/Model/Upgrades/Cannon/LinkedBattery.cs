using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using System;
using Abilities;

namespace UpgradesList
{

	public class LinkedBattery : GenericUpgrade
	{
		public LinkedBattery() : base()
		{
            Types.Add(UpgradeType.Cannon);

			Name = "Linked Battery";
            Cost = 2;

            isLimited = true;

            UpgradeAbilities.Add(new LinkedBatteryAbility());
		}

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;
        }
    }
}

namespace Abilities
{
    public class LinkedBatteryAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += CheckLinkedBatteryAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= CheckLinkedBatteryAbility;
        }

        public void CheckLinkedBatteryAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new ActionsList.LinkedBatteryAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            });
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
            return Combat.ChosenWeapon is PrimaryWeaponClass;
        }

        private bool IsCannon()
        {
            return Combat.ChosenWeapon is GenericSecondaryWeapon && (Combat.ChosenWeapon as GenericSecondaryWeapon).hasType(UpgradeType.Cannon);
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack && (IsPrimaryWeapon() || IsCannon()))
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