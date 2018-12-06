﻿using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class Glitterstim : GenericUpgrade
    {
        public Glitterstim() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Glitterstim",
                UpgradeType.Illicit,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.GlitterstimAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class GlitterstimAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostShip.ShipId + ": " + Name,
                TriggerType = TriggerTypes.OnCombatPhaseStart,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = AskGlitterstim
            });
        }

        private void AskGlitterstim(object sender, System.EventArgs e)
        {
            GlitterstimDecisionSubPhase newSubPhase = (GlitterstimDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Glitterstim Dicision",
                typeof(GlitterstimDecisionSubPhase),
                Triggers.FinishTrigger
            );
            newSubPhase.GlitterstimAbility = this;
            newSubPhase.Start();
        }

        public void ActivateGlitterstim()
        {
            HostShip.Tokens.AssignCondition(typeof(Conditions.Glitterstim));

            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterTrigger;
            Phases.Events.OnEndPhaseStart_NoTriggers += DeactivateGlitterstim;

            HostShip.Tokens.AssignToken(typeof(StressToken), GlitterstimEffect);
        }

        private void GlitterstimEffect()
        {
            HostShip.OnGenerateDiceModifications += AddGlitterstimDiceModification;
        }

        private void AddGlitterstimDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.GlitterstimDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip
            };
            host.AddAvailableDiceModification(newAction);
        }

        public void DeactivateGlitterstim()
        {
            HostShip.OnGenerateDiceModifications -= AddGlitterstimDiceModification;

            Phases.Events.OnEndPhaseStart_NoTriggers -= DeactivateGlitterstim;
        }
    }
}

namespace SubPhases
{

    public class GlitterstimDecisionSubPhase : DecisionSubPhase
    {
        public Abilities.FirstEdition.GlitterstimAbility GlitterstimAbility;

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = GlitterstimAbility.HostShip.ShipId + ": Use ability of Glitterstim?";
            RequiredPlayer = GlitterstimAbility.HostShip.Owner.PlayerNo;

            AddDecision("Yes", UseGlitterstimAbility);
            AddDecision("No", DontUseGlitterstimAbility);

            DefaultDecisionName = (IsTimeToUseGlitterStim()) ? "Yes" : "No";

            UI.ShowSkipButton();

            callBack();
        }

        private bool IsTimeToUseGlitterStim()
        {
            bool result = false;

            GenericShip host = GlitterstimAbility.HostShip;

            if (host.Tokens.HasToken(typeof(Conditions.Glitterstim))) return false;

            if (host.Owner.UsesHotacAiRules)
            {
                if (!host.Tokens.HasToken(typeof(FocusToken)))
                {
                    int priority = 0;
                    if (ActionsHolder.HasTarget(host)) priority += 20;
                    priority += ActionsHolder.CountEnemiesTargeting(host) * 10;
                    if (host.State.HullCurrent < host.State.HullMax) priority += 5;
                    if (host.Tokens.HasToken(typeof(StressToken))) priority -= 10;

                    if (priority > 10)
                    {
                        Messages.ShowInfo("AI decides to use Glitterstim");
                        result = true;
                    }
                }
            }

            return result;
        }

        private void UseGlitterstimAbility(object sender, System.EventArgs e)
        {
            GlitterstimAbility.ActivateGlitterstim();
            GlitterstimAbility.HostUpgrade.TryDiscard(ConfirmDecision);
        }

        private void DontUseGlitterstimAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

    }

}

namespace ActionsList
{

    public class GlitterstimDiceModification : GenericAction
    {

        public GlitterstimDiceModification()
        {
            Name = DiceModificationName = "Glitterstim";

            IsTurnsAllFocusIntoSuccess = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            return true;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.CurrentDiceRoll.Focuses > 0)
            {
                if (Combat.AttackStep == CombatStep.Attack)
                {
                    result = 55;
                }
                else if (Combat.AttackStep == CombatStep.Defence)
                {
                    result = 80;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus, DieSide.Success);
            callBack();
        }

    }

}

namespace Conditions
{
    public class Glitterstim : GenericToken
    {
        public Glitterstim(GenericShip host) : base(host)
        {
            Name = "Glitterstim Condition";
            Temporary = true;
            Tooltip = new UpgradesList.FirstEdition.Glitterstim().ImageUrl;
        }
    }
}