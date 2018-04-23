using Upgrade;
using System;
using Ship;
using Abilities;
using UnityEngine;

namespace UpgradesList
{
    public class AgentKallus : GenericUpgrade
    {
        public AgentKallus() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Agent Kallus";
            Cost = 2;

            isUnique = true;

            AvatarOffset = new Vector2(43, 1);

            UpgradeAbilities.Add(new AgentKallusAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}

namespace Abilities
{
    public class AgentKallusAbility : GenericAbility
    {

        public GenericShip AgentKallusSelectedTarget;

        public override void ActivateAbility()
        {
            Phases.OnGameStart += RegisterAgentKallusAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnGameStart -= RegisterAgentKallusAbility;
        }

        private void RegisterAgentKallusAbility()
        {
            Triggers.RegisterTrigger(new Trigger() {
                Name = "Agent Kallus decision",
                TriggerType = TriggerTypes.OnGameStart,
                TriggerOwner = HostShip.Owner.PlayerNo,
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

            foreach (var enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Ships)
            {
                selectAgentKallusTargetDecisionSubPhase.AddDecision(
                    enemyShip.Value.ShipId + ": " + enemyShip.Value.PilotName,
                    delegate { SelectTarget(enemyShip.Value); }
                );
            }

            selectAgentKallusTargetDecisionSubPhase.InfoText = "Agent Kallus: Select enemy ship";

            GenericShip bestEnemyAce = GetEnemyPilotWithHighestSkill();
            selectAgentKallusTargetDecisionSubPhase.DefaultDecisionName = bestEnemyAce.ShipId + ": " + bestEnemyAce.PilotName;

            selectAgentKallusTargetDecisionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectAgentKallusTargetDecisionSubPhase.Start();
        }

        private void SelectTarget(GenericShip targetShip)
        {
            Messages.ShowInfo("Agent Kallus: " + targetShip.PilotName + " (" + targetShip.ShipId + ") is selected");

            AgentKallusSelectedTarget = targetShip;

            HostShip.AfterGenerateAvailableActionEffectsList += AddAgentKallusDiceModification;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void AddAgentKallusDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.AgentKallusDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host,
                AgentKallusSelectedTarget = AgentKallusSelectedTarget
            };
            host.AddAvailableActionEffect(newAction);
        }

        private GenericShip GetEnemyPilotWithHighestSkill()
        {
            GenericShip bestAce = null;
            int maxPilotSkill = 0;
            foreach (var enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Ships)
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

            if (Combat.CurrentDiceRoll.Focuses > 0)
            {
                result = 100;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (Combat.CurrentDiceRoll.Focuses > 0)
            {
                Combat.CurrentDiceRoll.Change(DieSide.Focus, DieSide.Success, 1);
            }
            else
            {
                Messages.ShowError("No Focus results to change");
            }
            callBack();
        }

    }

}


