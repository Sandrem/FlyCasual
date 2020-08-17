using Actions;
using ActionsList;
using Arcs;
using Ship;
using SubPhases;
using System;
using UnityEngine;

namespace Abilities
{
    public class AskToPerformAction : AbilityPart
    {
        private TriggeredAbility Ability;

        public Type ActionType { get; }
        public ActionColor ActionColor { get; }
        public bool CanBePerformedWhileStressed { get; }
        public AbilityPart AfterAction { get; }

        public AskToPerformAction(
            Type actionType,
            ActionColor actionColor = ActionColor.White,
            bool canBePerformedWhileStressed = false,
            AbilityPart afterAction = null
        )
        {
            ActionType = actionType;
            ActionColor = actionColor;
            CanBePerformedWhileStressed = canBePerformedWhileStressed;
            AfterAction = afterAction;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Debug.Log("Ask action");

            Ability = ability;

            GenericAction action = (GenericAction)Activator.CreateInstance(ActionType);
            action.Color = ActionColor;
            if (CanBePerformedWhileStressed) action.CanBePerformedWhileStressed = true;

            Ability.HostShip.AskPerformFreeAction
            (
                action,
                delegate { AfterAction.DoAction(ability); },
                "Description short",
                "Description long",
                (IImageHolder) Ability.HostReal,
                false
            );
        }
    }
}
