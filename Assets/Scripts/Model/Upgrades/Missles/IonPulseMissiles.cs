using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using UpgradesList;
using Abilities;

namespace UpgradesList
{
	public class IonPulseMissiles : GenericSecondaryWeapon
	{
		public IonPulseMissiles () : base()
		{
			Types.Add(UpgradeType.Missile);

			Name = "Ion Pulse Missiles";
			Cost = 3;

			MinRange = 2;
			MaxRange = 3;
			AttackValue = 3;

			RequiresTargetLockOnTargetToShoot = true;
			SpendsTargetLockOnTargetToShoot = false;
			IsDiscardedForShot = true;

			UpgradeAbilities.Add(new IonPulseMissilesAbility());
		}
	}
}

namespace Abilities
{
	public class IonPulseMissilesAbility : GenericAbility
	{
		public override void ActivateAbility() 
		{
			HostShip.OnShotHitAsAttacker += RegisterIonPulseMissleHit;
		}

		public override void DeactivateAbility()
		{
			//If the below line is uncommented, it appears that the ability
			//	gets deactivated before it fires.

			//Symptom is that when the missiles hit, no Ion Tokens are assigned
			//	and the actual damage rolled is applied, instead of a single damage.

			//I have tested this with various messages displayed to the user at 
			//	important points - when this happens, "RegisterIonPulseMissileHit()" is never called.

			//HostShip.OnShotHitAsAttacker -= RegisterIonPulseMissleHit;

			//Moved the deregister to after the single damage is applied, see below.
			//Order of events is:
			//	1. Cancel and Remove all hits from attack dice
			//	2. Assign first Ion Token
			//	3. Assign second Ion Token
			//	4. Apply single damage
			//	5. Deactivate ability with HostShip.OnShotHitAsAttacker -= RegisterIonPulseMissleHit;
		}

		private void RegisterIonPulseMissleHit()
		{
			//Note: the old syntax of "if (Combat.ChosenWeapon == this)" no longer works 
			//	since "this" is now the ability, not the weapon.
			//	"this.HostUpgrade" gets the weapon, which can then be compared to "Combat.ChosenWeapon"
			if (Combat.ChosenWeapon == this.HostUpgrade) {
				Triggers.RegisterTrigger (new Trigger () {
					Name = "Ion Pulse Missile Hit",
					TriggerType = TriggerTypes.OnShotHit,
					TriggerOwner = Combat.Attacker.Owner.PlayerNo,
					EventHandler = delegate {
						IonPulseMissilesHitEffect ();
					}
				}
				);
			}
		}

		private void IonPulseMissilesHitEffect()
		{
			Combat.DiceRollAttack.CancelAllResults();
			Combat.DiceRollAttack.RemoveAllFailures();

			Messages.ShowError ("Defender receives 2 Ion Tokens");

			Combat.Defender.Tokens.AssignToken(
				new Tokens.IonToken(Combat.Defender),
				delegate {
					GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
					Game.Wait(0, AddSecondIonToken);
				}
			);
		}

		private void AddSecondIonToken()
		{
			Combat.Defender.Tokens.AssignToken(
				new Tokens.IonToken(Combat.Defender),
				delegate {
					GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
					Game.Wait(0, DefenderSuffersDamage);
				}
			);
		}

		private void DefenderSuffersDamage()
		{
			Messages.ShowError ("Defender suffers 1 damage");

			Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Success);

			Triggers.RegisterTrigger(new Trigger()
				{
					Name = "Suffer damage",
					TriggerType = TriggerTypes.OnDamageIsDealt,
					TriggerOwner = Combat.Defender.Owner.PlayerNo,
					EventHandler = Combat.Defender.SufferDamage,
					EventArgs = new DamageSourceEventArgs()
					{
						Source = Combat.Attacker,
						DamageType = DamageTypes.ShipAttack
					},
					Skippable = true
				});

			Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Triggers.FinishTrigger);

			HostShip.OnShotHitAsAttacker -= RegisterIonPulseMissleHit;
		}
	}
}
