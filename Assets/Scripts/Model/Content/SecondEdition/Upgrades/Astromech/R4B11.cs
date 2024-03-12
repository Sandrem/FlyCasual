using Upgrade;
using System;
using Tokens;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class R4B11 : GenericUpgrade
    {
        public R4B11() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "R4-B11",
                UpgradeType.Astromech,
                cost: 3, 
                abilityType: typeof(Abilities.SecondEdition.R4B11Ability),
                restriction: new FactionRestriction(Faction.Scum)
            );
            
            ImageUrl = "https://i.imgur.com/fyETLhg.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you perform an attack, you may remove 1 orange or red token from the defender
    // to reroll any number of defense dice

    public class R4B11Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                timing: DiceModificationTimingType.Opposite,
                count: int.MaxValue,
                payAbilityCost: PayAbilityCost
            );
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            if (IsDefenderHasBadTokens())
            {
                R4B11AbilityDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<R4B11AbilityDecisionSubPhase>(
                    "R4-B11: Select token to remove",
                    delegate { callback(true); }
                );

                subphase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
                subphase.DescriptionLong = "Select 1 orange or red token to remove from the defender";
                subphase.ImageSource = HostUpgrade;

                subphase.DecisionOwner = HostShip.Owner;
                subphase.Start();
            }
            else
            {
                Messages.ShowError("No orange or red token to spend");
                callback(false);
            }
        }

        private bool IsDefenderHasBadTokens()
        {
            return Combat.Defender.Tokens.HasTokenByColor(TokenColors.Orange)
                || Combat.Defender.Tokens.HasTokenByColor(TokenColors.Red);
        }

        private int GetAiPriority()
        {
            if (Combat.AttackStep == CombatStep.Attack)
            {
                return (Combat.DiceRollAttack.BlanksNotRerolled > 1) ? 10 : 0;
            }
            else if (Combat.AttackStep == CombatStep.Defence)
            {
                return (Combat.DiceRollAttack.Successes - Combat.DiceRollDefence.Successes > 0 && Combat.DiceRollDefence.Failures > 0) ? 10 : 0;
            }
            else
            {
                return 0;
            }
        }

        private bool IsAvailable()
        {
            return IsDefenderHasBadTokens();
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}

namespace SubPhases
{
    public class R4B11AbilityDecisionSubPhase : RemoveBadTokenFromDefenderDecisionSubPhase {}
}