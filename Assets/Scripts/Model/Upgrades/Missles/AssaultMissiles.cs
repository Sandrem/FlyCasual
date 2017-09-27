using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
	public class AssaultMissiles : GenericSecondaryWeapon
	{
		public AssaultMissiles () : base()
		{
			Type = UpgradeType.Missile;

			Name = "Assault Missiles";

			Cost = 5;
			MinRange = 2;
			MaxRange = 3;
			AttackValue = 4;

			RequiresTargetLockOnTargetToShoot = true;
			SpendsTargetLockOnTargetToShoot = true;
			IsDiscardedForShot = true;
		}

		public override void AttachToShip(Ship.GenericShip host)
		{
			base.AttachToShip(host);

			SubscribeOnHit();
		}

		private void SubscribeOnHit()
		{
			Host.OnAttackHitAsAttacker += RegisterAssaultMissleHit;
		}

		private void RegisterAssaultMissleHit()
		{
			if (Combat.ChosenWeapon == this)
			{
				Triggers.RegisterTrigger(new Trigger()
					{
						Name = "Assault Missle Hit",
						TriggerType = TriggerTypes.OnAttackHit,
						TriggerOwner = Combat.Attacker.Owner.PlayerNo,
						EventHandler = delegate{
							AssaultMissilesHitEffect();
						}
					});
			}
		}

		private void AssaultMissilesHitEffect(){
			var ships = Roster.AllShips.Select (x => x.Value).ToList();

			foreach (Ship.GenericShip ship in ships) {

				// null refs?
				if (ship.Model == null || Combat.Defender == null || Combat.Defender.Model == null) {
					continue;
				}

				// Defending ship shouldn't suffer additional damage
				if (ship.Model == Combat.Defender.Model)
					continue;

				Board.ShipDistanceInformation shotInfo = new Board.ShipDistanceInformation(Combat.Defender, ship);

				if (shotInfo.Range == 1) {

					Messages.ShowErrorToHuman(string.Format("{0} is within range 1 of {1}; assault missile deals 1 damage!",
						ship.PilotName, Combat.Defender.PilotName));

					var diceRoll = new DiceRoll (DiceKind.Attack, 0, DiceRollCheckType.Combat);
					diceRoll.AddDice (DieSide.Success);
					var hitDie = diceRoll.DiceList [0];
					ship.AssignedDamageDiceroll.DiceList.Add(hitDie);

					Triggers.RegisterTrigger(new Trigger() {
						Name = "Suffer Assault Missle Damage",
						TriggerType = TriggerTypes.OnDamageIsDealt,
						TriggerOwner = ship.Owner.PlayerNo,
						EventHandler = ship.SufferDamage,
                        Skippable = true,
						EventArgs = new DamageSourceEventArgs()
						{
							Source = "Assault Missle",
							DamageType = DamageTypes.ShipAttack
						}
					});
				}
            }

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Triggers.FinishTrigger);
		}
	}
}