using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

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
		}

		public override void AttachToShip(Ship.GenericShip host)
		{
			base.AttachToShip(host);

			SubscribeOnHit();
		}

		private void SubscribeOnHit()
		{
			Host.OnShotHitAsAttacker += RegisterAssaultMissleHit;
		}

		private void RegisterAssaultMissleHit()
		{
			if (Combat.ChosenWeapon == this)
			{
				Triggers.RegisterTrigger(new Trigger()
					{
						Name = "Ion Pulse Missile Hit",
						TriggerType = TriggerTypes.OnShotHit,
						TriggerOwner = Combat.Attacker.Owner.PlayerNo,
						EventHandler = delegate{
							IonPulseMissilesHitEffect();
						}
					});
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
					
				}
			);

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
		}


	}
}
