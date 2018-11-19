using Actions;
using ActionsList;
using System;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class Guri : StarViperClassAttackPlatform
        {
            public Guri() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Guri",
                    5,
                    62,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.GuriAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ShipInfo.ActionIcons.Actions.RemoveAll(a => a.ActionType == typeof(FocusAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(CalculateAction)));

                ShipInfo.ActionIcons.LinkedActions.RemoveAll(a => a.ActionType == typeof(BoostAction) && a.ActionLinkedType == typeof(FocusAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BoostAction), typeof(CalculateAction)));
                ShipInfo.ActionIcons.LinkedActions.RemoveAll(a => a.ActionType == typeof(BarrelRollAction) && a.ActionLinkedType == typeof(FocusAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), typeof(CalculateAction)));

                SEImageNumber = 178;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class GuriAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterGuriAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterGuriAbility;
        }

        private void RegisterGuriAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskGuriAbility);
        }

        private void AskGuriAbility(object sender, EventArgs e)
        {
            if (BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1, 1), Team.Type.Enemy).Count > 0)
            {
                if (!alwaysUseAbility)
                {
                    AskToUseAbility(AlwaysUseByDefault, UseAbility, null, null, true);
                }
                else
                {
                    AssignFocus(Triggers.FinishTrigger);
                }
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            AssignFocus(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void AssignFocus(Action callback)
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), callback);
        }

    }
}