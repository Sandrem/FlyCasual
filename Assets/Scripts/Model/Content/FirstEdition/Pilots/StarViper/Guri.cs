using System;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.StarViper
    {
        public class Guri : StarViper
        {
            public Guri() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Guri",
                    5,
                    30,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.GuriAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
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

        protected virtual Vector2 AbilityRange
        {
            get { return new Vector2(1, 1); }
        }

        private void AskGuriAbility(object sender, EventArgs e)
        {
            if (BoardTools.Board.GetShipsAtRange(HostShip, AbilityRange, Team.Type.Enemy).Count > 0)
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
