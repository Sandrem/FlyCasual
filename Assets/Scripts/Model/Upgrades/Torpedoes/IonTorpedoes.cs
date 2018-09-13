using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using UpgradesList;
using Abilities;
using Ship;
using RuleSets;
using Tokens;

namespace UpgradesList
{
	public class IonTorpedoes : GenericSecondaryWeapon, ISecondEditionUpgrade
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

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 6;

            SpendsTargetLockOnTargetToShoot = false;

            IsDiscardedForShot = false;
            UsesCharges = true;
            MaxCharges = 2;

            UpgradeAbilities.RemoveAll(a => a is IonTorpedoesAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.IonDamageAbilitySE());
        }
    }
}

namespace Abilities
{
	public class IonTorpedoesAbility : GenericAbility
	{
		private List<GenericShip> shipsToAssignIon;

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
							AssignIonTokens ();
						}
					}
				);
			}
		}

		private void AssignIonTokens()
		{
			shipsToAssignIon = new List<GenericShip>();

			var ships = Roster.AllShips.Select (x => x.Value).ToList();

			foreach (GenericShip ship in ships) {
				BoardTools.DistanceInfo shotInfo = new BoardTools.DistanceInfo(Combat.Defender, ship);
				if (shotInfo.Range == 1) {
					shipsToAssignIon.Add (ship);
				}
			}

			AssignIonTokensRecursive();
		}

		private void AssignIonTokensRecursive()
		{
			if (shipsToAssignIon.Count > 0)
            {
				GenericShip shipToAssignIon = shipsToAssignIon [0];
				shipsToAssignIon.Remove (shipToAssignIon);
				Messages.ShowErrorToHuman(shipToAssignIon.PilotName + " assigned Ion Token");
				shipToAssignIon.Tokens.AssignToken (typeof(IonToken), AssignIonTokensRecursive);
			}
            else
            {
				Triggers.FinishTrigger();
			}
		}
	}
}

namespace Abilities.SecondEdition
{
    public class IonDamageAbilitySE : IonDamageAbility
    {

        protected override void IonTurretEffect(object sender, System.EventArgs e)
        {
            var ionTokens = Combat.DiceRollAttack.Successes - 1;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            if (ionTokens > 0)
            {
                Combat.Defender.Tokens.AssignTokens(
                    () => new Tokens.IonToken(Combat.Defender),
                    ionTokens,
                    delegate {
                        GameManagerScript.Wait(2, DefenderSuffersDamage);
                    }
                );
            }
            else
            {
                DefenderSuffersDamage();
            }
        }
    }

}

