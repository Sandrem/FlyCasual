using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using UpgradesList;
using Abilities;
using Ship;
using Tokens;

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
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnShotHitAsAttacker -= RegisterIonPulseMissleHit;
        }

        private void RegisterIonPulseMissleHit()
		{
			if (Combat.ChosenWeapon == this.HostUpgrade)
            {
				Triggers.RegisterTrigger (
                    new Trigger () {
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

			Combat.Defender.Tokens.AssignToken(typeof(IonToken), AddSecondIonToken);
		}

		private void AddSecondIonToken()
		{
			Combat.Defender.Tokens.AssignToken(typeof(IonToken), DefenderSuffersDamage);
		}

		private void DefenderSuffersDamage()
		{
			Messages.ShowError ("Defender suffers 1 damage");

            DamageSourceEventArgs ionpulseDamage = new DamageSourceEventArgs()
            {
                Source = "Ion Pulse Missiles",
                DamageType = DamageTypes.ShipAttack
            };

            Combat.Defender.Damage.TryResolveDamage(1, ionpulseDamage, Triggers.FinishTrigger);

            HostShip.OnShotHitAsAttacker -= RegisterIonPulseMissleHit;
		}
	}
}
