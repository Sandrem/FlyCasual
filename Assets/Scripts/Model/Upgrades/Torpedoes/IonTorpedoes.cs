using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using UpgradesList;
using Abilities;
using Ship;

namespace UpgradesList
{
	public class IonTorpedoes : GenericSecondaryWeapon
	{
		public IonTorpedoes () : base()
		{
			Types.Add(UpgradeType.Torpedo);

			Name = "Ion Torpedoes";
			Cost = 5;

			MinRange = 2;
			MaxRange = 3;
			AttackValue = 4;

			RequiresTargetLockOnTargetToShoot = true;
			SpendsTargetLockOnTargetToShoot = true;
			IsDiscardedForShot = true;

			UpgradeAbilities.Add(new IonTorpedoesAbility());
		}
	}
}

namespace Abilities
{
	public class IonTorpedoesAbility : GenericAbility
	{
		public override void ActivateAbility() 
		{
			HostShip.OnShotHitAsAttacker += RegisterIonTorpedoesHit;
		}

		public override void DeactivateAbility()
		{
			// Ability is turned off only after full attack is finished
			HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
		}

		private void DeactivateAbilityPlanned(GenericShip ship)
		{
			HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
			HostShip.OnShotHitAsAttacker -= RegisterIonTorpedoesHit;
		}

		private void RegisterIonTorpedoesHit()
		{
			if (Combat.ChosenWeapon == this.HostUpgrade)
			{
				Triggers.RegisterTrigger (
					new Trigger () {
						Name = "Ion Torpedo Hit",
						TriggerType = TriggerTypes.OnShotHit,
						TriggerOwner = Combat.Attacker.Owner.PlayerNo,
						EventHandler = delegate {
							IonTorpedoHitEffect ();
						}
					}
				);
			}
		}

		private void IonTorpedoHitEffect()
		{
			Combat.DiceRollAttack.CancelAllResults();
			Combat.DiceRollAttack.RemoveAllFailures();

			Messages.ShowError ("Ion Torpedoes Hit");

			var ships = Roster.AllShips.Select (x => x.Value).ToList();

			foreach (GenericShip ship in ships) {

				// null refs?
				if (ship.Model == null || Combat.Defender == null || Combat.Defender.Model == null) {
					continue;
				}

				Board.ShipDistanceInformation shotInfo = new Board.ShipDistanceInformation(Combat.Defender, ship);

				if (shotInfo.Range == 1) {

					ship.Tokens.AssignToken(
						new Tokens.IonToken(ship),
						delegate {
							Messages.ShowError("Ion Tokens Assigned");
						}
					);
				}
			}

			Triggers.ResolveTriggers(TriggerTypes.OnShotHit, Triggers.FinishTrigger);
		}
	}
}

