using Abilities;
using ActionsList;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class AdvancedProtonTorpedoes : GenericSecondaryWeapon
    {
        public AdvancedProtonTorpedoes() : base()
        {
            Types.Add(UpgradeType.Torpedo);

            Name = "Advanced Proton Torpedoes";
            Cost = 6;

            MinRange = 1;
            MaxRange = 1;
            AttackValue = 5;

            RequiresTargetLockOnTargetToShoot = true;
            SpendsTargetLockOnTargetToShoot = true;
            IsDiscardedForShot = true;

            UpgradeAbilities.Add(new AdvancedProtonTorpedoesAbility());
        }
    }
}

namespace Abilities
{
    public class AdvancedProtonTorpedoesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddDiceModification;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnGenerateDiceModifications -= AddDiceModification;
        }

        private void AddDiceModification(GenericShip host)
		{
			AdvancedProtonTorpedoesAction action = new AdvancedProtonTorpedoesAction()
			{
				Host = host,
				ImageUrl = HostUpgrade.ImageUrl,
				Source = HostUpgrade
			};
			host.AddAvailableDiceModification(action);
		}
	}
}

namespace ActionsList
{ 
	public class AdvancedProtonTorpedoesAction : GenericAction
	{
		public AdvancedProtonTorpedoesAction()
		{
			Name = DiceModificationName = "Advanced Proton Torpedoes";

			IsTurnsOneFocusIntoSuccess = true;
		}

		private void AdvancedProtonTorpedoesAddDiceModification(Ship.GenericShip ship)
		{
			ship.AddAvailableDiceModification(this);
		}

		public override bool IsDiceModificationAvailable()
		{
			bool result = true;

			if (Combat.AttackStep != CombatStep.Attack) result = false;

			if (Combat.ChosenWeapon != Source) result = false;

			return result;
		}

		public override int GetDiceModificationPriority()
		{
			int result = 0;

			if (Combat.AttackStep == CombatStep.Attack)
			{
				int blanks = Combat.DiceRollAttack.Blanks;
				if (blanks > 0) result = 100;
			}

			return result;
		}

		public override void ActionEffect(System.Action callBack)
		{
			Combat.CurrentDiceRoll.Change(DieSide.Blank, DieSide.Focus, 3);
			callBack();
		}
	}
}