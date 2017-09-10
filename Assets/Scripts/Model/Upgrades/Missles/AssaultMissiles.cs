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
			Type = UpgradeType.Missiles;

			Name = "Assault Missiles";
			ShortName = "Assa. Missiles";
			ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/b/b0/Assault_Missiles.png";

			Cost = 5;
			MinRange = 2;
			MaxRange = 3;
			AttackValue = 4;

			RequiresTargetLockOnTargetToShoot = false; // true
			SpendsTargetLockOnTargetToShoot = false; // true
			IsDiscardedForShot = false; // true
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
							Triggers.FinishTrigger();
						}
					});
			}
		}

		private void AssaultMissilesHitEffect(){
			var ships = Roster.AllShips.Select (x => x.Value).ToList();


			foreach (Ship.GenericShip ship in ships) {
				if (ship.Model == Combat.Defender.Model)
					continue;

				Board.ShipDistanceInformation shotInfo = new Board.ShipDistanceInformation(Combat.Defender, ship);

				if (shotInfo.Range == 1) {

					Messages.ShowErrorToHuman(string.Format("{0} within range 1 of {1}; taking damage", ship.PilotName, Combat.Defender.PilotName));

					Selection.ActiveShip = ship;

					var diceRoll = new DiceRoll (DiceKind.Attack, 0, DiceRollCheckType.Combat);
					diceRoll.AddDice (DiceSide.Success);
					var hitDie = diceRoll.DiceList [0];
					Selection.ActiveShip.AssignedDamageDiceroll.DiceList.Add(hitDie);

					Triggers.RegisterTrigger(new Trigger() {
						Name = "Suffer Assault Missle Damage",
						TriggerType = TriggerTypes.OnDamageIsDealt,
						TriggerOwner = ship.Owner.PlayerNo,
						EventHandler = ship.SufferDamage,
						EventArgs = new DamageSourceEventArgs()
						{
							Source = "Assault Missle",
							DamageType = DamageTypes.ShipAttack
						}
					});


					/*
					*/
				}
			}
		}
	}
}