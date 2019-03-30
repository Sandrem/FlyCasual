using Upgrade;
using System.Linq;
using System.Collections.Generic;
using System;

namespace UpgradesList.SecondEdition
{
    public class R2HA : GenericUpgrade
    {
        public R2HA() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2-HA",
                UpgradeType.Astromech,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.R2HAAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/b3e6d35ca6b6fd297312248ddf4e69a7.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R2HAAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name + "'s Ability",
                CanBeUsed,
                GetAiPriority,
                DiceModificationType.Reroll,
                int.MaxValue,
                payAbilityCost: SpendTargetLockOnAttacker
            );
        }

        private bool CanBeUsed()
        {
            return (Combat.AttackStep == CombatStep.Defence && ActionsHolder.HasTargetLockOn(HostShip, Combat.Attacker));
        }

        private int GetAiPriority()
        {
            return 85;
        }

        private void SpendTargetLockOnAttacker(Action<bool> callback)
        {
            if (ActionsHolder.HasTargetLockOn(HostShip, Combat.Attacker))
            {
                SpendTargetLock(delegate { callback(true); });
            }
            else
            {
                Messages.ShowError("Error: The attacker has no Target Lock to spend.");
                callback(false);
            }
        }

        private void SpendTargetLock(Action callBack)
        {
            List<char> letters = ActionsHolder.GetTargetLocksLetterPairs(Combat.Defender, Combat.Attacker);
            HostShip.Tokens.SpendToken(typeof(Tokens.BlueTargetLockToken), callBack, letters.First());
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}