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
        public AiSelectShipPriority AiPriority { get; }
        public string Name { get; }
        public string Description { get; }
        public IImageHolder ImageSource { get; }
        public bool ShowSkipButton { get; }

        public SelectShipAction(string name, string description, IImageHolder imageSource, SelectShipFilter filter, AbilityPart action, AiSelectShipPriority aiPriority)
        {
            Name = name;
            Description = description;
            ImageSource = imageSource;
            Filter = filter;
            Action = action;
            AiPriority = aiPriority;
            ShowSkipButton = true;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;

            ability.SelectTargetForAbility(
                WhenSelected,
                FilterTargets,
                GetAiPriority,
                ability.HostShip.Owner.PlayerNo,
                Name,
                Description,
                ImageSource,
                ShowSkipButton
            );
        }

        private void WhenSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            this.Action.DoAction(Ability);
        }

        private int GetAiPriority(GenericShip ship)
        {
            switch (AiPriority)
            {
                case AiSelectShipPriority.Enemy:
                    if (Ability.TargetShip.Owner.PlayerNo != Ability.HostShip.Owner.PlayerNo)
                    {
                        return Ability.TargetShip.PilotInfo.Cost;
                    }
                    else
                    {
                        return 0;
                    }
                case AiSelectShipPriority.Friendly:
                    if (Ability.TargetShip.Owner.PlayerNo == Ability.HostShip.Owner.PlayerNo)
                    {
                        return Ability.TargetShip.PilotInfo.Cost;
                    }
                    else
                    {
                        return 0;
                    }
                default:
                    Debug.Log("Error: No AiPriorty handling for this ability");
                    return 0;
            }
        }

        private bool FilterTargets(GenericShip ship)
        {
            return Filter.FilterTargets(Ability, ship);
        }
    }

    public class SelectShipFilter
    {
        public int MinRange { get; }
        public int MaxRange { get; }
        public ArcType InArcType { get; }

        public SelectShipFilter(int minRange, int maxRange, ArcType inArcType)
        {
            MinRange = minRange;
            MaxRange = maxRange;
            InArcType = inArcType;
        }

        public bool FilterTargets(TriggeredAbility ability, GenericShip ship)
        {
            return ability.FilterTargetsByRangeInSpecificArc(ship, MinRange, MaxRange, InArcType);
        }
    }

    public enum AiSelectShipPriority
    {
        Enemy,
        Friendly
    }
}
