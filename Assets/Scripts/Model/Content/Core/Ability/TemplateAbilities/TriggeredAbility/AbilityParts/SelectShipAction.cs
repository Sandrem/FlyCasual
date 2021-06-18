using Abilities.Parameters;
using Ship;
using SubPhases;

namespace Abilities
{
    public class SelectShipAction : AbilityPart
    {
        private GenericAbility Ability;
        public ConditionsBlock Conditions { get; }
        public AbilityPart Action { get; }
        public AiSelectShipPlan AiSelectShipPlan { get; }
        public AbilityDescription AbilityDescription { get; }
        public bool ShowSkipButton { get; }

        public SelectShipAction(AbilityDescription abilityDescription, ConditionsBlock conditions, AbilityPart action, AiSelectShipPlan aiSelectShipPlan)
        {
            AbilityDescription = abilityDescription;
            Conditions = conditions;
            Action = action;
            AiSelectShipPlan = aiSelectShipPlan;
            ShowSkipButton = true;
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;

            if (ability.TargetsForAbilityExist(FilterTargets))
            {
                ability.SelectTargetForAbility(
                    WhenSelected,
                    FilterTargets,
                    GetAiSelectShipPriority,
                    ability.HostShip.Owner.PlayerNo,
                    AbilityDescription.Name,
                    AbilityDescription.Description,
                    AbilityDescription.ImageSource,
                    ShowSkipButton
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void WhenSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            this.Action.TargetUpgrade = TargetUpgrade;
            this.Action.DoAction(Ability);
        }

        private int GetAiSelectShipPriority(GenericShip ship)
        {
            return AiSelectShipPlan.GetAiSelectShipPriority(ship, Ability.HostShip);
        }

        private bool FilterTargets(GenericShip ship)
        {
            ConditionArgs args = new ConditionArgs()
            {
                ShipAbilityHost = Ability.HostShip,
                ShipToCheck = ship
            };
            return Conditions.Passed(args);
        }
    }
}
