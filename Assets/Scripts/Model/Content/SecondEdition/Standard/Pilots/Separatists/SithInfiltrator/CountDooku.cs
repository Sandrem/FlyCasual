using System;
using System.Collections.Generic;
using Ship;
using Upgrade;
using BoardTools;
using SubPhases;
using Tokens;
using System.Linq;
using Content;

namespace Ship.SecondEdition.SithInfiltrator
{
    public class CountDooku : SithInfiltrator
    {
        public CountDooku()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Count Dooku",
                "Darth Tyranus",
                Faction.Separatists,
                3,
                7,
                24,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.CountDookuPilotAbility),
                force: 3,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.TacticalRelay,
                    UpgradeType.Title
                },
                tags: new List<Tags>
                {
                    Tags.DarkSide,
                    Tags.Sith
                }
            );

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
                    HostShip.PilotInfo.PilotName,
                    NeverUseByDefault,
                    AgreeToPerformAction,
                    descriptionLong: "Do you want to spend 1 Force to perform an action?",
                    imageHolder: HostShip
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

            HostShip.State.SpendForce(
                1,
                delegate {
                    HostShip.AskPerformFreeAction(
                         HostShip.GetAvailableActions(),
                         Triggers.FinishTrigger,
                         HostShip.PilotInfo.PilotName,
                         "After you perform an attack that hits, you may spend 1 Force to perform an action",
                         HostShip
                     );
                }
            );
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

                subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
                subphase.DescriptionLong = "You can spend a force token to remove a red or a blue token from your ship";
                subphase.ImageSource = HostShip;

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

            HostShip.State.SpendForce(
                1,
                delegate { HostShip.Tokens.RemoveToken(token, Phases.CurrentSubPhase.CallBack); }
            );
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
