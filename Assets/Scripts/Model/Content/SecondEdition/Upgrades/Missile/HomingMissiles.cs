using Ship;
using System;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class HomingMissiles : GenericSpecialWeapon
    {
        public HomingMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Homing Missiles",
                UpgradeType.Missile,
                cost: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    charges: 2
                ),
                abilityType: typeof(Abilities.SecondEdition.HomingMissilesAbility),
                seImageNumber: 39
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class HomingMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += RegisterOwnTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= RegisterOwnTrigger;
        }

        private void RegisterOwnTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnShotStart, AskDefender);
        }

        private void AskDefender(object sender, EventArgs e)
        {
            AskToUseAbility(
                UseIfHaveEnoughHP,
                SufferOneDamage,
                infoText: "You may choose to suffer 1 damage. If you do, skip the Attack and Defense Dice steps and the attack is treated as hitting.",
                requiredPlayer: Combat.Defender.Owner.PlayerNo
            );
        }

        private bool UseIfHaveEnoughHP()
        {
            return Combat.Defender.State.HullCurrent + Combat.Defender.State.ShieldsCurrent > 1;
        }

        private void SufferOneDamage(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            Combat.Defender.Damage.TryResolveDamage(
                1,
                new DamageSourceEventArgs()
                {
                    DamageType = DamageTypes.CardAbility,
                    Source = HostUpgrade
                },
                SkipDice
            );
        }

        private void SkipDice()
        {
            Combat.SkipDiceRolls = true;
            Triggers.FinishTrigger();
        }
    }
}
