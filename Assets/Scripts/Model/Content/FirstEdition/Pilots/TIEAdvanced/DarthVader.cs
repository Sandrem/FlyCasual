using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEAdvanced
    {
        public class DarthVader : TIEAdvanced
        {
            public DarthVader() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Darth Vader",
                    9,
                    29,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.DarthVaderAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Blue";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class DarthVaderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionDecisionSubphaseEnd += DoSecondAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionDecisionSubphaseEnd -= DoSecondAction;
        }

        private void DoSecondAction(GenericShip ship)
        {
            if (!HostShip.Tokens.HasToken(typeof(Tokens.StressToken)) && Phases.CurrentSubPhase.GetType() == typeof(SubPhases.ActionDecisonSubPhase))
            {
                RegisterAbilityTrigger(TriggerTypes.OnFreeActionPlanned, PerformFreeAction);
            }
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();

            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}