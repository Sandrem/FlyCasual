using Upgrade;
using Ship;
using SubPhases;
using Abilities;
using Tokens;

namespace UpgradesList
{
    public class Glitterstim : GenericUpgrade
    {
        public Glitterstim() : base()
        {
            Types.Add(UpgradeType.Illicit);
            Name = "Glitterstim";
            Cost = 2;

            UpgradeAbilities.Add(new GlitterstimAbility());
        }
    }
}

namespace Abilities
{
    public class GlitterstimAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers -= RegisterTrigger;
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
            Conditions.Glitterstim newConditionToken = new Conditions.Glitterstim(HostShip) { Tooltip = HostUpgrade.ImageUrl };
            HostShip.Tokens.AssignCondition(newConditionToken);

            Phases.OnCombatPhaseStart_Triggers -= RegisterTrigger;
            Phases.OnEndPhaseStart_NoTriggers += DeactivateGlitterstim;

            HostShip.Tokens.AssignToken(new StressToken(HostShip), GlitterstimEffect);
        }

        private void GlitterstimEffect()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += AddGlitterstimDiceModification;
        }

        private void AddGlitterstimDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.GlitterstimDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            };
            host.AddAvailableActionEffect(newAction);
        }

        public void DeactivateGlitterstim()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddGlitterstimDiceModification;

            Phases.OnEndPhaseStart_NoTriggers -= DeactivateGlitterstim;
        }
    }
}

namespace SubPhases
{

    public class GlitterstimDecisionSubPhase : DecisionSubPhase
    {
        public GlitterstimAbility GlitterstimAbility;

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

            if (host.Owner.GetType() == typeof(Players.HotacAiPlayer))
            {
                if (!host.Tokens.HasToken(typeof(FocusToken)))
                {
                    int priority = 0;
                    if (Actions.HasTarget(host)) priority += 20;
                    priority += Actions.CountEnemiesTargeting(host) * 10;
                    if (host.Hull < host.MaxHull) priority += 5;
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
    public class Glitterstim : GenericToken
    {
        public Glitterstim(GenericShip host) : base(host)
        {
            Name = "Glitterstim Condition";
            Temporary = true;
        }
    }
}
