using Content;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class Scorch : TIEFoFighter
        {
            public Scorch() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Scorch\"",
                    "Zeta Leader",
                    Faction.FirstOrder,
                    4,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ScorchAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/80/bb/80bbabbb-3b30-448e-a896-ddcfc05082bd/swz26_a1_scorch.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ScorchAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterEpsilonLeaderAbility;
            HostShip.OnAttackFinishAsAttacker += RemoveZetaLeaderAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterEpsilonLeaderAbility;
            HostShip.OnAttackFinishAsAttacker -= RemoveZetaLeaderAbility;
        }

        private void RegisterEpsilonLeaderAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, ShowDecision);
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            // check if this ship is stressed
            if (!HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                // give user the option to use ability
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseAbility,
                    descriptionLong: "Do you want to receive 1 Stress token to roll 1 additional attack die?",
                    imageHolder: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            // don't need to check stressed as done already
            // add an attack dice
            IsAbilityUsed = true;
            //HostShip.ChangeFirepowerBy(+1);
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += ZetaLeaderAddAttackDice;
            HostShip.Tokens.AssignToken(typeof(StressToken), SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void RemoveZetaLeaderAbility(GenericShip genericShip)
        {
            // At the end of combat phase, need to remove attack value increase
            if (IsAbilityUsed)
            {
                //HostShip.ChangeFirepowerBy(-1);
                HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= ZetaLeaderAddAttackDice;
                IsAbilityUsed = false;
            }
        }
        private void ZetaLeaderAddAttackDice(ref int value)
        {
            value++;
        }
    }
}
