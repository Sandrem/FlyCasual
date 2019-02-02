using Ship;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEFoFighter
    {
        public class ZetaLeader : TIEFoFighter
        {
            public ZetaLeader() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Zeta Leader\"",
                    7,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ZetaLeaderAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class ZetaLeaderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterEpsilonLeaderAbility;
            HostShip.OnAttackFinishAsAttacker += RemoveEpsilonLeaderAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterEpsilonLeaderAbility;
            HostShip.OnAttackFinishAsAttacker -= RemoveEpsilonLeaderAbility;
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
                AskToUseAbility(AlwaysUseByDefault, UseAbility);
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

        private void RemoveEpsilonLeaderAbility(GenericShip genericShip)
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