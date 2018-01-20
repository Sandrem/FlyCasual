﻿using System;
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
			Type = UpgradeType.Torpedo;

			Name = "Advanced Proton Torpedoes";
			Cost = 6;

			MinRange = 1;
			MaxRange = 1;
			AttackValue = 5;

			RequiresTargetLockOnTargetToShoot = true;
			SpendsTargetLockOnTargetToShoot = true;
			IsDiscardedForShot = true;
		}

		public override void AttachToShip(Ship.GenericShip host)
		{
			base.AttachToShip(host);

			AddDiceModification();
		}

		private void AddDiceModification()
		{
			ActionsList.AdvancedProtonTorpedoesAction action = new ActionsList.AdvancedProtonTorpedoesAction()
			{
				Host = Host,
				ImageUrl = ImageUrl,
				Source = this
			};
			action.AddDiceModification();

			Host.AddAvailableAction(action);
		}

	}

}

namespace ActionsList
{ 

	public class AdvancedProtonTorpedoesAction : GenericAction
	{

		public AdvancedProtonTorpedoesAction()
		{
			Name = EffectName = "Advanced Proton Torpedoes";

			IsTurnsOneFocusIntoSuccess = true;
		}

		public void AddDiceModification()
		{
			Host.AfterGenerateAvailableActionEffectsList += AdvancedProtonTorpedoesAddDiceModification;
		}

		private void AdvancedProtonTorpedoesAddDiceModification(Ship.GenericShip ship)
		{
			ship.AddAvailableActionEffect(this);
		}

		public override bool IsActionEffectAvailable()
		{
			bool result = true;

			if (Combat.AttackStep != CombatStep.Attack) result = false;

			if (Combat.ChosenWeapon != Source) result = false;

			return result;
		}

		public override int GetActionEffectPriority()
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