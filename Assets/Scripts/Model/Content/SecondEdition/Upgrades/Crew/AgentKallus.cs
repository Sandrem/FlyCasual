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
    public class AgentKallusAbility : FirstEdition.AgentKallusAbility
    {        
        protected override void SelectTarget(GenericShip targetShip)
        {
            Messages.ShowInfo("Agent Kallus is hunting " + targetShip.PilotInfo.PilotName + " (" + targetShip.ShipId + ")");

            // The difference with First Edition is that we keep track of the target with a condition token
            targetShip.Tokens.AssignCondition(typeof(Conditions.HuntedCondition));

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
            host.AddAvailableDiceModification(newAction);
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
        public HuntedCondition(GenericShip host) : base(host)
        {
            Name = "Hunted Condition";
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

            selectAllyDecisionSubPhase.InfoText = "Hunted: Select another friendly ship";

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
