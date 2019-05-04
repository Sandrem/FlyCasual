using Upgrade;
using System.Linq;
using System.Collections.Generic;
using System;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class R2C4 : GenericUpgrade
    {
        public R2C4() : base()
        {
            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);

            UpgradeInfo = new UpgradeCardInfo(
                "R2-C4",
                UpgradeType.Astromech,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Republic),
                abilityType: typeof(Abilities.SecondEdition.R2C4Ability)
            );

            ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/8/89/Astromech_R2-C4.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R2C4Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "R2-C4",
                CanBeUsed,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Success,
                payAbilityCost: SpendTargetLockOnAttacker
            );
        }

        private bool CanBeUsed()
        {
            return
            (
                Combat.AttackStep == CombatStep.Attack
                && HostShip.Tokens.HasToken<EvadeToken>()
                && DiceRoll.CurrentDiceRoll.Focuses > 0
            );
        }

        private int GetAiPriority()
        {
            return 41;
        }

        private void SpendTargetLockOnAttacker(Action<bool> callback)
        {
            if (HostShip.Tokens.HasToken<EvadeToken>())
            {
                HostShip.Tokens.SpendToken(
                    typeof(EvadeToken),
                    delegate { callback(true); }
                );
            }
            else
            {
                Messages.ShowError("Error: The attacker has no Evade to spend.");
                callback(false);
            }
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}