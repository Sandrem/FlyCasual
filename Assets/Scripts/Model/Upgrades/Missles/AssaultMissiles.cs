using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;

namespace UpgradesList
{
    public class AssaultMissiles : GenericSecondaryWeapon
    {
        public AssaultMissiles() : base()
        {
            Types.Add(UpgradeType.Missile);

            Name = "Assault Missiles";

            Cost = 5;
            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            RequiresTargetLockOnTargetToShoot = true;
            SpendsTargetLockOnTargetToShoot = true;
            IsDiscardedForShot = true;

            UpgradeAbilities.Add(new AssaultMissilesAbility());
        }
    }
}

namespace Abilities
{
    public class AssaultMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterAssaultMissleHit;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnShotHitAsAttacker -= RegisterAssaultMissleHit;
        }

		private void RegisterAssaultMissleHit()
		{
			if (Combat.ChosenWeapon == HostUpgrade)
			{
				Triggers.RegisterTrigger(new Trigger()
					{
						Name = "Assault Missile Hit",
						TriggerType = TriggerTypes.OnShotHit,
						TriggerOwner = Combat.Attacker.Owner.PlayerNo,
						EventHandler = delegate{
							AssaultMissilesHitEffect();
						}
					});
			}
		}

		private void AssaultMissilesHitEffect(){
			var ships = Roster.AllShips.Select (x => x.Value).ToList();

			foreach (GenericShip ship in ships) {

				// null refs?
				if (ship.Model == null || Combat.Defender == null || Combat.Defender.Model == null) {
					continue;
				}

				// Defending ship shouldn't suffer additional damage
				if (ship.Model == Combat.Defender.Model)
					continue;

				Board.ShipDistanceInfo shotInfo = new Board.ShipDistanceInfo(Combat.Defender, ship);

				if (shotInfo.Range == 1) {

					//Messages.ShowErrorToHuman(string.Format("{0} is within range 1 of {1}; assault missile deals 1 damage!", ship.PilotName, Combat.Defender.PilotName));

					var diceRoll = new DiceRoll (DiceKind.Attack, 0, DiceRollCheckType.Combat);
					diceRoll.AddDice (DieSide.Success);
					var hitDie = diceRoll.DiceList [0];
					ship.AssignedDamageDiceroll.DiceList.Add(hitDie);

					Triggers.RegisterTrigger(new Trigger() {
						Name = "Suffer Assault Missile Damage",
						TriggerType = TriggerTypes.OnDamageIsDealt,
						TriggerOwner = ship.Owner.PlayerNo,
						EventHandler = ship.SufferDamage,
                        Skippable = true,
						EventArgs = new DamageSourceEventArgs()
						{
							Source = "Assault Missile",
							DamageType = DamageTypes.CardAbility
						}
					});
				}
            }

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Triggers.FinishTrigger);
		}
	}
}