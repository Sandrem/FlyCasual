using Ship;
using Upgrade;
using System.Collections.Generic;
using Tokens;
using System.Linq;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class HotshotCoPilot : GenericUpgrade
    {
        public HotshotCoPilot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hotshot Co-pilot",
                UpgradeType.Crew,
                cost: 4,
                abilityType: typeof(Abilities.FirstEdition.HotshotCoPilotAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class HotshotCoPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += CheckAttackAbility;
            HostShip.OnAttackStartAsDefender += DefenceAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= CheckAttackAbility;
            HostShip.OnAttackStartAsDefender -= DefenceAbility;
        }

        private void CheckAttackAbility()
        {
            if (Combat.ChosenWeapon is PrimaryWeaponClass) AssignCondition(Combat.Defender);
        }

        private void DefenceAbility()
        {
            AssignCondition(Combat.Attacker);
        }

        private void AssignCondition(GenericShip ship)
        {
            Messages.ShowInfo("Hotshot Co-pilot effect is active");

            ship.Tokens.AssignCondition(typeof(Conditions.HotshotCoPilotCondition));

            ship.OnTryConfirmDiceResults += DisallowIfHasFocusToken;
            ship.OnAiGetDiceModificationPriority += PrioritizeSpendFocus;
            ship.OnTokenIsSpent += CheckRemoveCondition;
            ship.OnAttackFinish += RemoveCondition;
        }

        private void CheckRemoveCondition(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(FocusToken)) RemoveCondition(ship);
        }

        private void RemoveCondition(GenericShip ship)
        {
            Messages.ShowInfo("Hotshot Co-pilot effect is not active");

            ship.OnTryConfirmDiceResults -= DisallowIfHasFocusToken;
            ship.OnAiGetDiceModificationPriority -= PrioritizeSpendFocus;
            ship.OnTokenIsSpent -= CheckRemoveCondition;
            ship.OnAttackFinish -= RemoveCondition;

            ship.Tokens.RemoveCondition(typeof(Conditions.HotshotCoPilotCondition));
        }

        private void DisallowIfHasFocusToken(ref bool result)
        {
            GenericShip currentShip = null;

            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    currentShip = Combat.Attacker;
                    break;
                case CombatStep.Defence:
                    currentShip = Combat.Defender;
                    break;
                default:
                    break;
            }

            if (currentShip.GetDiceModificationsGenerated().Any(n => n.TokensSpend.Contains(typeof(FocusToken))))
            {
                Messages.ShowError("Cannot confirm results - must spend focus token!");
                result = false;
            }
        }

        private void PrioritizeSpendFocus(GenericAction diceModification, ref int priority)
        {
            if (diceModification.TokensSpend.Contains(typeof(FocusToken))) priority += 1000;
        }
    }
}

namespace Conditions
{
    public class HotshotCoPilotCondition : GenericToken
    {
        public HotshotCoPilotCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            Temporary = false;
            Tooltip = new UpgradesList.FirstEdition.HotshotCoPilot().ImageUrl;
        }
    }
}