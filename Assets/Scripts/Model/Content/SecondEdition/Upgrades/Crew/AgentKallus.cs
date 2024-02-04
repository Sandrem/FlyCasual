using Ship;
using Upgrade;
using UnityEngine;
using Tokens;
using System.Linq;
using Conditions;
using System;

namespace UpgradesList.SecondEdition
{
    public class AgentKallus : GenericUpgrade
    {
        public AgentKallus() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Agent Kallus",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.AgentKallusAbility),
                seImageNumber: 110
            );

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(409, 15),
                new Vector2(125, 125)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class AgentKallusAbility : FirstEdition.AgentKallusAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterAgentKallusAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterAgentKallusAbility;
        }

        protected override void SelectTarget(GenericShip targetShip)
        {
            Messages.ShowInfo("Agent Kallus is hunting " + targetShip.PilotInfo.PilotName + " (" + targetShip.ShipId + ")");

            // The difference with First Edition is that we keep track of the target with a condition token
            targetShip.Tokens.AssignCondition( new HuntedCondition(targetShip) { SourceUpgrade = HostUpgrade } );

            HostShip.OnGenerateDiceModifications += AddAgentKallusDiceModification;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        protected override void AddAgentKallusDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SecondEdition.AgentKallusDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = host
            };
            host.AddAvailableDiceModificationOwn(newAction);
        }
    }
}

namespace ActionsList.SecondEdition
{

    public class AgentKallusDiceModification : ActionsList.AgentKallusDiceModification
    {
        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            // The difference with First Edition is that we check for the Hunted condition to be present on the defender
            if (Combat.AttackStep == CombatStep.Attack && Combat.Attacker.ShipId == HostShip.ShipId && Combat.Defender.Tokens.HasToken<HuntedCondition>())
            {
                result = true;
            }

            return result;
        }
    }

}

namespace Conditions
{
    /// <summary>
    /// The Hunted condition passes to another friendly ship when the one carrying it is destroyed. 
    /// This is a big difference with First Edition Agent Kallus, where he would become useless once his chosen target was destroyed.
    /// </summary>
    public class HuntedCondition : GenericToken
    {
        public GenericUpgrade SourceUpgrade;

        public HuntedCondition(GenericShip host) : base(host)
        {
            Name = ImageName = "Hunted Condition";
            Temporary = false;

            Tooltip = "https://raw.githubusercontent.com/Sandrem/xwing-data2-test/master/images/conditions/hunted.png";
        }

        public override void WhenAssigned()
        {
            Host.OnShipIsDestroyed += RegisterTrigger;            
        }
                
        public override void WhenRemoved()
        {
            Host.OnShipIsDestroyed -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship, bool flag)
        {
            var otherFriendliesCount = Roster.GetPlayer(Host.Owner.PlayerNo).Ships.Values
                .Where(s => s != null && !s.IsDestroyed && s.ShipId != Host.ShipId)
                .Count();

            // Do nothing if there aren't any friendlies left
            if (otherFriendliesCount == 0)
            {
                return;
            }

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Hunted",
                TriggerType = TriggerTypes.OnShipIsDestroyed,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = AssignConditionToAnotherFriendly
            });
        }

        private void AssignConditionToAnotherFriendly(object sender, EventArgs e)
        {            
            var otherFriendlies = Roster.GetPlayer(Host.Owner.PlayerNo).Ships.Values
                .Where(s => s != null && !s.IsDestroyed && s.ShipId != Host.ShipId)
                .ToArray();

            // Do nothing if there aren't any friendlies left
            if (otherFriendlies.Length == 0)
            {
                return;
            }

            HuntedDecisionSubPhase selectAllyDecisionSubPhase = Phases.StartTemporarySubPhaseNew<HuntedDecisionSubPhase>(Name, Triggers.FinishTrigger);

            selectAllyDecisionSubPhase.DescriptionShort = "Agent Kallus";
            selectAllyDecisionSubPhase.DescriptionLong = "Assign the Haunted condition to 1 enemy ship";
            selectAllyDecisionSubPhase.ImageSource = SourceUpgrade;

            foreach (var friendlyShip in otherFriendlies)
            {
                var friendly = friendlyShip;
                selectAllyDecisionSubPhase.AddDecision(
                    friendlyShip.ShipId + ": " + friendlyShip.PilotInfo.PilotName,
                    delegate 
                    {
                        SelectTarget(friendly);
                    }
                );
            }

            selectAllyDecisionSubPhase.DescriptionShort = "Hunted: Select another friendly ship";

            GenericShip leastWorthAlly = otherFriendlies                
                .OrderBy(ally => ally.State.Initiative)
                .FirstOrDefault();
            selectAllyDecisionSubPhase.DefaultDecisionName = leastWorthAlly.ShipId + ": " + leastWorthAlly.PilotInfo.PilotName;
            selectAllyDecisionSubPhase.RequiredPlayer = Host.Owner.PlayerNo;
            selectAllyDecisionSubPhase.Start();            
        }

        private class HuntedDecisionSubPhase : SubPhases.DecisionSubPhase { }
                
        private void SelectTarget(GenericShip targetShip)
        {
            Messages.ShowInfo("Hunted: " + targetShip.PilotInfo.PilotName + " (" + targetShip.ShipId + ") is selected");

            targetShip.Tokens.AssignCondition(typeof(HuntedCondition));

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }
    }
}

namespace Abilities.FirstEdition
{
    public class AgentKallusAbility : GenericAbility
    {

        public GenericShip AgentKallusSelectedTarget;

        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += RegisterAgentKallusAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= RegisterAgentKallusAbility;
        }

        protected void RegisterAgentKallusAbility()
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

        protected void SelectAgentKallusTarget(object Sender, System.EventArgs e)
        {
            AgentKallusDecisionSubPhase selectAgentKallusTargetDecisionSubPhase = (AgentKallusDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(AgentKallusDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectAgentKallusTargetDecisionSubPhase.DescriptionShort = "Agent Kallus";
            selectAgentKallusTargetDecisionSubPhase.DescriptionLong = "Assign the Haunted condition to 1 enemy ship";
            selectAgentKallusTargetDecisionSubPhase.ImageSource = HostUpgrade;

            foreach (var enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Ships)
            {
                selectAgentKallusTargetDecisionSubPhase.AddDecision(
                    enemyShip.Value.ShipId + ": " + enemyShip.Value.PilotInfo.PilotName,
                    delegate { SelectTarget(enemyShip.Value); }
                );
            }

            GenericShip bestEnemyAce = GetEnemyPilotWithHighestSkill();
            selectAgentKallusTargetDecisionSubPhase.DefaultDecisionName = bestEnemyAce.ShipId + ": " + bestEnemyAce.PilotInfo.PilotName;

            selectAgentKallusTargetDecisionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectAgentKallusTargetDecisionSubPhase.Start();
        }

        protected virtual void SelectTarget(GenericShip targetShip)
        {
            Messages.ShowInfo("Agent Kallus is hunting " + targetShip.PilotInfo.PilotName + " (" + targetShip.ShipId + ")");

            AgentKallusSelectedTarget = targetShip;

            HostShip.OnGenerateDiceModifications += AddAgentKallusDiceModification;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        protected virtual void AddAgentKallusDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.AgentKallusDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                AgentKallusSelectedTarget = AgentKallusSelectedTarget
            };
            host.AddAvailableDiceModificationOwn(newAction);
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

        protected class AgentKallusDecisionSubPhase : SubPhases.DecisionSubPhase { }
    }
}

namespace ActionsList
{

    public class AgentKallusDiceModification : GenericAction
    {
        public GenericShip AgentKallusSelectedTarget;

        public AgentKallusDiceModification()
        {
            Name = DiceModificationName = "Agent Kallus";

            IsTurnsOneFocusIntoSuccess = true;
        }

        public override bool IsDiceModificationAvailable()
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
                Messages.ShowError("This die roll had no Focus results to change");
            }
            callBack();
        }

    }

}
