using System;
using System.Collections.Generic;
using Ship;
using Upgrade;

namespace Ship.SecondEdition.SithInfiltrator
{
    public class O66 : SithInfiltrator
    {
        public O66()
        {
            PilotInfo = new PilotCardInfo(
                "O-66",
                3,
                56,
                true,
                abilityType: typeof(Abilities.SecondEdition.O66PilotAbility)
            );

            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c4/a7/c4a7f563-fae2-4dd2-a0bb-fa0fd697d2a5/swz30_0-66.png";

            ShipInfo.ActionIcons.SwitchToDroidActions();
        }
    }
}

namespace Abilities.SecondEdition
{
    public class O66PilotAbility : GenericAbility
    {
        public GenericShip PreviousAttacker { get; private set; }
        public GenericShip PreviousDefender { get; private set; }

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, UseCalculateToPerformAction);
        }

        private void UseCalculateToPerformAction(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasToken(typeof(Tokens.CalculateToken)))
            {
                PreviousAttacker = Combat.Attacker;
                PreviousDefender = Combat.Defender;

                Selection.DeselectAllShips();
                Selection.ChangeActiveShip(HostShip);

                AskToUseAbility(
                    NeverUseByDefault,
                    AgreeToPerformAction,
                    infoText: "Do you want to spend a calculate token to perform an action?"
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AgreeToPerformAction(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.Tokens.SpendToken(typeof(Tokens.CalculateToken), PerformFreeAction);
        }

        private void PerformFreeAction()
        {
            HostShip.AskPerformFreeAction(HostShip.GetAvailableActions(), FinishAbility);
        }

        private void FinishAbility()
        {
            Selection.ChangeActiveShip(PreviousAttacker);
            Selection.ChangeAnotherShip(PreviousDefender);

            Triggers.FinishTrigger();
        }
    }
}