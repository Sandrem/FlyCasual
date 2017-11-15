using Upgrade;
using System;
using Ship;

namespace UpgradesList
{
    public class AgentKallus : GenericUpgrade
    {
        public GenericShip AgentKallusSelectedTarget;

        public AgentKallus() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Agent Kallus";
            Cost = 2;

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Phases.OnGameStart += RegisterAgentKallusAbility;
        }

        private void RegisterAgentKallusAbility()
        {
            Triggers.RegisterTrigger(new Trigger() {
                Name = "Agent Kallus decision",
                TriggerType = TriggerTypes.OnGameStart,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = SelectAgentKallusTarget,
                Skippable = true
            });
        }

        private void SelectAgentKallusTarget(object Sender, System.EventArgs e)
        {
            AgentKallusDecisionSubPhase selectAgentKallusTargetDecisionSubPhase = (AgentKallusDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(AgentKallusDecisionSubPhase),
                Triggers.FinishTrigger
            );

            foreach (var enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(Host.Owner.PlayerNo)).Ships)
            {
                selectAgentKallusTargetDecisionSubPhase.AddDecision(
                    enemyShip.Value.ShipId + ": " + enemyShip.Value.PilotName,
                    delegate { SelectTarget(enemyShip.Value); }
                );
            }

            selectAgentKallusTargetDecisionSubPhase.InfoText = "Agent Kallus: Select enemy ship";

            GenericShip bestEnemyAce = GetEnemyPilotWithHighestSkill();
            selectAgentKallusTargetDecisionSubPhase.DefaultDecision = bestEnemyAce.Shields + ": " + bestEnemyAce.PilotName;

            selectAgentKallusTargetDecisionSubPhase.RequiredPlayer = Host.Owner.PlayerNo;

            selectAgentKallusTargetDecisionSubPhase.Start();
        }

        private void SelectTarget(GenericShip targetShip)
        {
            AgentKallusSelectedTarget = targetShip;

            Host.AfterGenerateAvailableActionEffectsList += AddAgentKallusDiceModification;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void AddAgentKallusDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.AgentKallusDiceModification()
            {
                ImageUrl = ImageUrl,
                Host = host,
                AgentKallusSelectedTarget = AgentKallusSelectedTarget
            };
            host.AddAvailableActionEffect(newAction);
        }

        private GenericShip GetEnemyPilotWithHighestSkill()
        {
            GenericShip bestAce = null;
            int maxPilotSkill = 0;
            foreach (var enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(Host.Owner.PlayerNo)).Ships)
            {
                if (enemyShip.Value.PilotSkill > maxPilotSkill)
                {
                    bestAce = enemyShip.Value;
                    maxPilotSkill = enemyShip.Value.PilotSkill;
                }
            }
            return bestAce;
        }

        private class AgentKallusDecisionSubPhase : SubPhases.DecisionSubPhase {}
    }
}

namespace ActionsList
{

    public class AgentKallusDiceModification : GenericAction
    {
        public GenericShip AgentKallusSelectedTarget;

        public AgentKallusDiceModification()
        {
            Name = EffectName = "Agent Kallus";

            IsTurnsOneFocusIntoSuccess = true;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    if (Combat.Defender.ShipId == AgentKallusSelectedTarget.ShipId) result = true;
                    break;
                case CombatStep.Defence:
                    if (Combat.Attacker.ShipId == AgentKallusSelectedTarget.ShipId) result = true;
                    break;
                default:
                    break;
            }

            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.CurentDiceRoll.Focuses > 0)
            {
                result = 100;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (Combat.CurentDiceRoll.Focuses > 0)
            {
                Combat.CurentDiceRoll.Change(DieSide.Focus, DieSide.Success, 1);
            }
            else
            {
                Messages.ShowError("No Focus results to change");
            }
        }

    }

}


