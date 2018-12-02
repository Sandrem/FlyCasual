using SubPhases;
using System;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YV666
    {
        public class Bossk : YV666
        {
            public Bossk() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Bossk",
                    7,
                    35,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.BosskPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    //When you perform an attack that hits, before dealing damage. you may cancel 1 critical result to add 2 success results.
    public class BosskPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterBosskPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterBosskPilotAbility;
        }

        protected virtual void RegisterBosskPilotAbility()
        {
            if (Combat.DiceRollAttack.CriticalSuccesses > 0)
            {
                Phases.CurrentSubPhase.Pause();
                RegisterAbilityTrigger(TriggerTypes.OnShotHit, PromptToChangeCritSuccess);
            }
        }

        private void PromptToChangeCritSuccess(object sender, EventArgs e)
        {
            DecisionSubPhase decisionSubPhase = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(DecisionSubPhase),
                Triggers.FinishTrigger);

            decisionSubPhase.InfoText = "Bossk: Would you like to cancel 1 critical result to add 2 success results?";

            decisionSubPhase.AddDecision("Yes", ConvertCriticalsToSuccesses);
            decisionSubPhase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); }
            );

            decisionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
            decisionSubPhase.ShowSkipButton = true;

            decisionSubPhase.DefaultDecisionName = "No";

            decisionSubPhase.Start();
        }

        private void ConvertCriticalsToSuccesses(object sender, EventArgs e)
        {
            Combat.DiceRollAttack.AddDice(DieSide.Success);
            Combat.DiceRollAttack.AddDice(DieSide.Success);

            Combat.DiceRollAttack.DiceList.Remove(
                Combat.DiceRollAttack.DiceList.First(die => die.Side == DieSide.Crit));

            Messages.ShowInfoToHuman("Bossk: Changed one critical result into two success results.");
            DecisionSubPhase.ConfirmDecision();
            Phases.CurrentSubPhase.Resume();
        }
    }
}