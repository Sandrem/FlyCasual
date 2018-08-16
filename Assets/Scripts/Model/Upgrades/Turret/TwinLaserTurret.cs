using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using Abilities;

namespace UpgradesList
{
    public class TwinLaserTurret : GenericSecondaryWeapon
    {
        public TwinLaserTurret() : base()
        {
            Types.Add(UpgradeType.Turret);

            Name = "Twin Laser Turret";
            Cost = 6;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 3;

            IsTwinAttack = true;
            CanShootOutsideArc = true;

            UpgradeAbilities.Add(new TwinLaserTurretAbility());
        }
    }
}

namespace Abilities
{
    public class TwinLaserTurretAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterTwinLaserTurretEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterTwinLaserTurretEffect;
        }

        private void RegisterTwinLaserTurretEffect()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        TriggerType = TriggerTypes.OnShotHit,
                        TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                        EventHandler = TwinLaserTurretEffect
                    }
                );
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
            DamageSourceEventArgs tltDamage = new DamageSourceEventArgs()
            {
                Source = HostShip,
                SourceDescription = "Twin Laser Turret",
                DamageType = DamageTypes.ShipAttack
            };

            Combat.Defender.Damage.TryResolveDamage(1, tltDamage, Triggers.FinishTrigger);
        }
    }
}