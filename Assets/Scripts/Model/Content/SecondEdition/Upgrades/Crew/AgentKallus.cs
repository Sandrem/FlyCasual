using Ship;
using Upgrade;
using UnityEngine;
using Tokens;
using System.Linq;
using Conditions;

namespace UpgradesList.SecondEdition
{
    public class AgentKallus : GenericUpgrade
    {
        public AgentKallus() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Agent Kallus",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.AgentKallusAbility),
                seImageNumber: 110
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class AgentKallusAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += RegisterAgentKallusAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= RegisterAgentKallusAbility;
        }

        private void RegisterAgentKallusAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
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

            targetShip.Tokens.AssignCondition(typeof(Conditions.HuntedCondition));

            HostShip.OnGenerateDiceModifications += AddAgentKallusDiceModification;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void AddAgentKallusDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SecondEdition.AgentKallusDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = host
            };
            host.AddAvailableDiceModification(newAction);
        }

        private GenericShip GetEnemyPilotWithHighestSkill()
        {
            GenericShip bestAce = null;
            int maxPilotSkill = 0;
            foreach (var enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Ships)
            {
                if (enemyShip.Value.State.Initiative > maxPilotSkill)
                {
                    bestAce = enemyShip.Value;
                    maxPilotSkill = enemyShip.Value.State.Initiative;
                }
            }
            return bestAce;
        }

        private class AgentKallusDecisionSubPhase : SubPhases.DecisionSubPhase { }
    }
}

namespace ActionsList.SecondEdition
{

    public class AgentKallusDiceModification : GenericAction
    {
        public AgentKallusDiceModification()
        {
            Name = DiceModificationName = "Agent Kallus";

            IsTurnsOneFocusIntoSuccess = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack && Combat.Attacker == HostShip && Combat.Defender.Tokens.HasToken<HuntedCondition>())
            {
                result = true;
            }

            return result;
        }

        public override int GetDiceModificationPriority()
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

namespace Conditions
{
    public class HuntedCondition : GenericToken
    {
        public HuntedCondition(GenericShip host) : base(host)
        {
            Name = "Hunted Condition";
            Temporary = false;

            Tooltip = "https://raw.githubusercontent.com/Sandrem/xwing-data2-test/master/images/conditions/hunted.png";
        }

        public override void WhenAssigned()
        {
            Host.OnShipIsDestroyed += AssignToAnotherFriendly;            
        }
                
        public override void WhenRemoved()
        {
            Host.OnShipIsDestroyed -= AssignToAnotherFriendly;
        }

        private void AssignToAnotherFriendly(GenericShip ship, bool flag)
        {
            var otherFriendlies = Roster.GetPlayer(Host.Owner.PlayerNo).Ships.Values
                .Where(s => s != Host)
                .ToArray();

            if (otherFriendlies.Length == 0)
            {
                return;
            }
                
            HuntedDecisionSubPhase selectNewHuntedTargetDecisionSubPhase = Phases.StartTemporarySubPhaseNew<HuntedDecisionSubPhase>(Name, Triggers.FinishTrigger);

            foreach (var friendlyShip in otherFriendlies)
            {
                var friendly = friendlyShip;
                selectNewHuntedTargetDecisionSubPhase.AddDecision(
                    friendlyShip.ShipId + ": " + friendlyShip.PilotName,
                    delegate 
                    {
                        SelectTarget(friendly);
                        Triggers.FinishTrigger();
                    }
                );
            }

            selectNewHuntedTargetDecisionSubPhase.InfoText = "Hunted: Select another friendly ship";

            GenericShip lowliestFriendlyPilot = GetFriendlyPilotWithLowestSkill();
            selectNewHuntedTargetDecisionSubPhase.DefaultDecisionName = lowliestFriendlyPilot.ShipId + ": " + lowliestFriendlyPilot.PilotName;

            selectNewHuntedTargetDecisionSubPhase.RequiredPlayer = Host.Owner.PlayerNo;

            selectNewHuntedTargetDecisionSubPhase.Start();            
        }

        private class HuntedDecisionSubPhase : SubPhases.DecisionSubPhase { }
                
        private void SelectTarget(GenericShip targetShip)
        {
            Messages.ShowInfo("Hunted: " + targetShip.PilotName + " (" + targetShip.ShipId + ") is selected");

            targetShip.Tokens.AssignCondition(typeof(HuntedCondition));

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private GenericShip GetFriendlyPilotWithLowestSkill()
        {
            GenericShip lowliestPilot = null;
            int minPilotSkill = 100;
            foreach (var allyShip in Roster.GetPlayer(Host.Owner.PlayerNo).Ships)
            {
                if (allyShip.Value.State.Initiative < minPilotSkill)
                {
                    lowliestPilot = allyShip.Value;
                    minPilotSkill = allyShip.Value.State.Initiative;
                }
            }
            return lowliestPilot;
        }

    }
}
