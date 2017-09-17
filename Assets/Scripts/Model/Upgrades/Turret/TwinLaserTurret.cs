using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class TwinLaserTurret : GenericSecondaryWeapon
    {
		public TwinLaserTurret() : base()
        {
            Type = UpgradeType.Turret;

            Name = "Twin Laser Turret";
            ShortName = "TLT";
            Cost = 6;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 3;

			IsTwinAttack = true; 
			CanShootOutsideArc = true;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

			SubscribeOnHit();
        }
			
		private void SubscribeOnHit()
		{
			Host.OnAttackHitAsAttacker += RegisterTwinLaserTurretEffect;
		}

		private void RegisterTwinLaserTurretEffect()
		{
			if (Combat.ChosenWeapon == this)
			{
				Triggers.RegisterTrigger(new Trigger()
					{
						Name = "Twin Laser Turret effect",
						TriggerType = TriggerTypes.OnAttackHit,
						TriggerOwner = Combat.Attacker.Owner.PlayerNo,
						EventHandler = TwinLaserTurretEffect
					});
			}
		}
		private void TwinLaserTurretEffect(object sender, System.EventArgs e)
		{
			Combat.DiceRollAttack.CancelAllResults();
			Combat.DiceRollAttack.RemoveAllFailures();
			DefenderSuffersDamage();
		}
		private void DefenderSuffersDamage()
		{
			Combat.Defender.AssignedDamageDiceroll.AddDice(DiceSide.Success);

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