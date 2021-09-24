using Abilities.Parameters;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class XX23SThreadTracers : GenericSpecialWeapon
    {
        public XX23SThreadTracers() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "XX-23 S-Thread Tracers",
                UpgradeType.Missile,
                cost: 4,
                limited: 2,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3,
                    requiresTokens: new List<Type>()
                    {
                        typeof(FocusToken),
                        typeof(CalculateToken),
                        typeof(BlueTargetLockToken),
                    },
                    charges: 2
                ),
                abilityType: typeof(Abilities.SecondEdition.XX23SThreadTracersAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/96/d7/96d7fd7e-cca2-403d-a291-5fcb973404c6/swz81_upgrade_s-tread-tracers.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class XX23SThreadTracersAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AfterYourAttackHits(HostUpgrade as IShipWeapon);

        public override AbilityPart Action => new EachShipCanDoAction(
            CanAcquireLock,
            onFinish: CancelAllResultsAndFinish,
            onSkip: CancelAllResults,
            new ConditionsBlock(
                new RangeToDefenderCondition(1, 3),
                new TeamCondition(ShipTypes.Friendly)
            ),
            new AbilityDescription(
                HostUpgrade.UpgradeInfo.Name,
                "Do you want to acquire a lock on defender?",
                HostUpgrade
            )
        );

        private void CanAcquireLock(GenericShip ship, Action callback)
        {
            ActionsHolder.AcquireTargetLock(
                ship,
                Combat.Defender,
                callback,
                callback
            );
        }

        private void CancelAllResultsAndFinish()
        {
            CancelAllResults();
            Triggers.FinishTrigger();
        }

        private void CancelAllResults()
        {
            Debug.Log("Cancel");

            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();
        }
    }
}