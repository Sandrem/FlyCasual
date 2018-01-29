using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using SubPhases;

namespace UpgradesList
{
    public class Glitterstim : GenericUpgrade
    {
        public Glitterstim() : base()
        {
            Type = UpgradeType.Illicit;
            Name = "Glitterstim";
            Cost = 2;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Host.OnCombatPhaseStart += RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger() {
                Name = Host.ShipId + ": " + Name,
                TriggerType = TriggerTypes.OnCombatPhaseStart,
                TriggerOwner = Host.Owner.PlayerNo,
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
            newSubPhase.GlitterstimUpgrade = this;
            newSubPhase.Start();
        }

        public void ActivateAbility()
        {
            Conditions.Glitterstim newConditionToken = new Conditions.Glitterstim(Host) { Tooltip = ImageUrl };
            Host.AssignToken(newConditionToken, delegate { });

            Host.OnCombatPhaseStart -= RegisterTrigger;
            Phases.OnEndPhaseStart += DeactivateAbility;

            Host.AssignToken(new Tokens.StressToken(Host), GlitterstimEffect);
        }

        private void GlitterstimEffect()
        {
            Host.AfterGenerateAvailableActionEffectsList += AddGlitterstimDiceModification;
        }

        private void AddGlitterstimDiceModification(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.GlitterstimDiceModification()
            {
                ImageUrl = ImageUrl,
                Host = Host
            };
            host.AddAvailableActionEffect(newAction);
        }

        public void DeactivateAbility()
        {
            Host.AfterGenerateAvailableActionEffectsList -= AddGlitterstimDiceModification;

            Phases.OnEndPhaseStart -= DeactivateAbility;
        }
    }
}

namespace SubPhases
{

    public class GlitterstimDecisionSubPhase : DecisionSubPhase
    {
        public UpgradesList.Glitterstim GlitterstimUpgrade;

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = GlitterstimUpgrade.Host.ShipId + ": Use ability of Glitterstim?";
            RequiredPlayer = GlitterstimUpgrade.Host.Owner.PlayerNo;

            AddDecision("Yes", UseGlitterstimAbility);
            AddDecision("No", DontUseGlitterstimAbility);

            DefaultDecision = (IsTimeToUseGlitterSteam()) ? "Yes" : "No";

            UI.ShowSkipButton();

            callBack();
        }

        private bool IsTimeToUseGlitterSteam()
        {
            bool result = false;

            GenericShip host = GlitterstimUpgrade.Host;
            if (host.Owner.GetType() == typeof(Players.HotacAiPlayer))
            {
                if (!host.HasToken(typeof(Tokens.FocusToken)))
                {
                    int priority = 0;
                    if (Actions.HasTarget(host)) priority += 20;
                    priority += Actions.CountEnemiesTargeting(host) * 10;
                    if (host.Hull < host.MaxHull) priority += 5;
                    if (host.HasToken(typeof(Tokens.StressToken))) priority -= 10;

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
            GlitterstimUpgrade.ActivateAbility();
            GlitterstimUpgrade.TryDiscard(ConfirmDecision);
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
            Name = EffectName = "Glitterstim";

            IsTurnsAllFocusIntoSuccess = true;
        }

        public override bool IsActionEffectAvailable()
        {
            return true;
        }

        public override int GetActionEffectPriority()
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
    public class Glitterstim : Tokens.GenericToken
    {
        public Glitterstim(GenericShip host) : base(host)
        {
            Name = "Glitterstim Condition";
            Temporary = true;
        }
    }
}
