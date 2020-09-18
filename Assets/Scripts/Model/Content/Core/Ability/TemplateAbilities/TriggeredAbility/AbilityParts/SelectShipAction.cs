using Abilities.Parameters;
using Arcs;
using Ship;
using SubPhases;
using UnityEngine;

namespace Abilities
{
    public class SelectShipAction : AbilityPart
    {
        private TriggeredAbility Ability;
        public SelectShipFilter Filter { get; }
        public AbilityPart Action { get; }
        public AiSelectShipPlan AiSelectShipPlan { get; }
        public AbilityDescription AbilityDescription { get; }
        public bool ShowSkipButton { get; }

        public SelectShipAction(AbilityDescription abilityDescription, SelectShipFilter filter, AbilityPart action, AiSelectShipPlan aiSelectShipPlan)
        {
            AbilityDescription = abilityDescription;
            Filter = filter;
            Action = action;
            AiSelectShipPlan = aiSelectShipPlan;
            ShowSkipButton = true;
        }

        public override void DoAction(TriggeredAbility ability)
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
            this.Action.DoAction(Ability);
        }

        private int GetAiSelectShipPriority(GenericShip ship)
        {
            return AiSelectShipPlan.GetAiSelectShipPriority(ship, Ability.HostShip);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return Filter.FilterTargets(Ability, ship);
        }
    }
}
