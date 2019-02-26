using System;
using System.Collections.Generic;
using Ship;
using Upgrade;
using BoardTools;
using SubPhases;
using Tokens;
using System.Linq;

namespace Ship.SecondEdition.SithInfiltrator
{
    public class CountDooku : SithInfiltrator
    {
        public CountDooku()
        {
            PilotInfo = new PilotCardInfo(
                "Count Dooku",
                3,
                64,
                true,
                abilityType: typeof(Abilities.SecondEdition.CountDookuPilotAbility),
                pilotTitle: "Darth Tyranus",
                force: 3,
                extraUpgradeIcon: UpgradeType.Force
            );

            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/3d/83/3d83b84f-e7d4-46b3-83ae-4d49245ae50d/swz30_count-dooku.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CountDookuPilotAbility : GenericAbility
    {
        public GenericShip PreviousAttacker { get; private set; }
        public GenericShip PreviousDefender { get; private set; }

        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += RegisterHitAbility;
            HostShip.OnAttackFinishAsDefender += RegisterDefenceAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= RegisterHitAbility;
            HostShip.OnAttackFinishAsDefender -= RegisterDefenceAbility;
        }

        private void RegisterHitAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, UseForceToPerformAction);
        }

        private void UseForceToPerformAction(object sender, EventArgs e)
        {
            if (HostShip.State.Force > 0)
            {
                AskToUseAbility(
                    NeverUseByDefault,
                    AgreeToPerformAction,
                    infoText: "Do you want to spend a force token to perform an action?"
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

            HostShip.State.Force--;
            HostShip.AskPerformFreeAction(HostShip.GetAvailableActions(), Triggers.FinishTrigger);
        }

        private void RegisterDefenceAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskToRemoveToken);
        }

        private void AskToRemoveToken(object sender, EventArgs e)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, Combat.Attacker, HostShip.PrimaryWeapons);

            if (HostShip.State.Force > 0
                && shotInfo.InArc
                && (HostShip.Tokens.HasTokenByColor(Tokens.TokenColors.Red) || HostShip.Tokens.HasTokenByColor(Tokens.TokenColors.Blue))
            )
            {
                PreviousAttacker = Combat.Attacker;
                PreviousDefender = Combat.Defender;

                Selection.DeselectAllShips();
                Selection.ChangeActiveShip(HostShip);

                SelectTokenSubphase subphase = Phases.StartTemporarySubPhaseNew<SelectTokenSubphase>("Select a token", Triggers.FinishTrigger);
                subphase.DecisionOwner = HostShip.Owner;
                subphase.ShowSkipButton = true;
                subphase.InfoText = "You can spend a force token to remove a red or a blue token from your ship";

                foreach (GenericToken token in HostShip.Tokens.GetAllTokens())
                {
                    if (token.TokenColor == TokenColors.Red || token.TokenColor == TokenColors.Blue)
                    {
                        subphase.AddDecision(token.Name, delegate { SpendForceToRemoveToken(token); });
                    }
                }

                subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

                subphase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SpendForceToRemoveToken(GenericToken token)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.State.Force--;
            HostShip.Tokens.RemoveToken(token, Phases.CurrentSubPhase.CallBack);
        }

        private void FinishAbility()
        {
            Selection.ChangeActiveShip(PreviousAttacker);
            Selection.ChangeAnotherShip(PreviousDefender);

            Triggers.FinishTrigger();
        }

        private class SelectTokenSubphase : DecisionSubPhase { };
    }
}
