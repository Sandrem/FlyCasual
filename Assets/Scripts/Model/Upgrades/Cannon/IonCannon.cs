using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

	public class IonCannon : GenericSecondaryWeapon
	{
		public IonCannon() : base()
		{
			Type = UpgradeType.Cannon;

			Name = "Ion Cannon";
			Cost = 3;

			MinRange = 1;
			MaxRange = 3;
			AttackValue = 3;
		}

		public override void AttachToShip(Ship.GenericShip host)
		{
			base.AttachToShip(host);

			SubscribeOnHit();
		}

		private void SubscribeOnHit()
		{
			Host.OnShotHitAsAttacker += RegisterIonCannonEffect;
		}

		private void RegisterIonCannonEffect()
		{
			if (Combat.ChosenWeapon == this)
			{
				Triggers.RegisterTrigger(new Trigger()
					{
						Name = "Ion Cannon effect",
						TriggerType = TriggerTypes.OnShotHit,
						TriggerOwner = Combat.Attacker.Owner.PlayerNo,
						EventHandler = IonCannonEffect
					});
			}
		}

		private void IonCannonEffect(object sender, System.EventArgs e)
		{
			Combat.DiceRollAttack.CancelAllResults();
			Combat.DiceRollAttack.RemoveAllFailures();

			Combat.Defender.Tokens.AssignToken(
				new Tokens.IonToken(Combat.Defender),
				delegate {
					GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
					Game.Wait(2, DefenderSuffersDamage);
				}
			);
		}

		private void DefenderSuffersDamage()
		{
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