using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

	public class FlechetteCannon : GenericSecondaryWeapon
	{
		public FlechetteCannon() : base()
		{
            Types.Add(UpgradeType.Cannon);

			Name = "Flechette Cannon";
			Cost = 2;

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
			Host.OnShotHitAsAttacker += RegisterFlechetteCannonEffect;
		}

		private void RegisterFlechetteCannonEffect()
		{
			if (Combat.ChosenWeapon == this)
			{
				Triggers.RegisterTrigger(new Trigger()
					{
						Name = "Flechette Cannon effect",
						TriggerType = TriggerTypes.OnShotHit,
						TriggerOwner = Combat.Attacker.Owner.PlayerNo,
						EventHandler = FlechetteCannonEffect
                });
			}
		}

		private void FlechetteCannonEffect(object sender, System.EventArgs e)
		{
			Combat.DiceRollAttack.CancelAllResults();
			Combat.DiceRollAttack.RemoveAllFailures();

            DefenderSuffersDamage();
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

			Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CheckStress);
		}

        private void CheckStress()
        {
            if (!Combat.Defender.HasToken(typeof(Tokens.StressToken)))
            {
                Combat.Defender.AssignToken(
                    new Tokens.StressToken(),
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

	}

}