using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;

namespace Ship
{
    namespace StarViper
    {
        public class Thweek : StarViper
        {
            public Thweek() : base()
            {
                PilotName = "Thweek";
                PilotSkill = 4;
                Cost = 28;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.ThweekAbility());
            }
        }
    }
}

namespace Abilities
{
    public class ThweekAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            IsAppliesConditionCard = true;
        }

        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += RegisterSelectThweekTarget;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= RegisterSelectThweekTarget;
        }

        private void RegisterSelectThweekTarget()
        {
            RegisterAbilityTrigger(TriggerTypes.OnSetupStart, SelectThweekTarget);
        }

        private void SelectThweekTarget(object Sender, System.EventArgs e)
        {
            ThweekTargetDecisionSubPhase selectTargetForThweekDecisionSubPhase = (ThweekTargetDecisionSubPhase) Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(ThweekTargetDecisionSubPhase),
                delegate {}
            );

            foreach (var enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Ships)
            {
                selectTargetForThweekDecisionSubPhase.AddDecision(
                    enemyShip.Value.ShipId + ": " + enemyShip.Value.PilotName,
                    delegate { SelectAbility(enemyShip.Value); }
                );
            }

            selectTargetForThweekDecisionSubPhase.InfoText = "Thweek: Select enemy ship";

            GenericShip bestEnemyAce = GetEnemyPilotWithHighestSkill();
            selectTargetForThweekDecisionSubPhase.DefaultDecisionName = bestEnemyAce.ShipId + ": " + bestEnemyAce.PilotName;

            selectTargetForThweekDecisionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectTargetForThweekDecisionSubPhase.Start();
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

        private void SelectAbility(GenericShip targetShip)
        {
            SubPhases.DecisionSubPhase.ConfirmDecision();

            ThweekAbilityDecisionSubPhase selectAbilityForThweekDecisionSubPhase = (ThweekAbilityDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(ThweekAbilityDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectAbilityForThweekDecisionSubPhase.AddDecision("Mimicked", delegate { Mimicked(targetShip); });
            selectAbilityForThweekDecisionSubPhase.AddTooltip("Mimicked", (new Conditions.Mimicked(targetShip)).Tooltip);
            selectAbilityForThweekDecisionSubPhase.AddDecision("Shadowed", delegate { Shadowed(targetShip); });
            selectAbilityForThweekDecisionSubPhase.AddTooltip("Shadowed", (new Conditions.Shadowed(targetShip)).Tooltip);

            selectAbilityForThweekDecisionSubPhase.InfoText = "Thweek: Select ability";
            selectAbilityForThweekDecisionSubPhase.DefaultDecisionName = "Shadowed";

            selectAbilityForThweekDecisionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectAbilityForThweekDecisionSubPhase.Start();
        }

        private void Mimicked(GenericShip targetShip)
        {
            bool abilityIsFound = false;
            foreach (var ability in targetShip.PilotAbilities)
            {
                if (!ability.IsAppliesConditionCard)
                {
                    Messages.ShowInfo("Ability of " + targetShip.PilotName + " is mimicked");

                    HostShip.PilotAbilities.Add( (GenericAbility) Activator.CreateInstance(ability.GetType()) );
                    HostShip.PilotAbilities[1].Initialize(HostShip);

                    abilityIsFound = true;

                    break;
                }
            }

            if (!abilityIsFound)
            { 
                Messages.ShowError(targetShip.PilotName + " doesn't have abilities to be mimicked");
            }
            targetShip.Tokens.AssignCondition(typeof(Conditions.Mimicked));
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void Shadowed(GenericShip targetShip)
        {
            Messages.ShowInfo("Pilot skill of " + targetShip.PilotName + " is shadowed");
            new ThweekPilotSkillModifier(HostShip, targetShip.PilotSkill);
            targetShip.Tokens.AssignCondition(typeof(Conditions.Shadowed));
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private class ThweekPilotSkillModifier : IModifyPilotSkill
        {
            private GenericShip host;
            private int newPilotSkill;

            public ThweekPilotSkillModifier(GenericShip host, int newPilotSkill)
            {
                this.host = host;
                this.newPilotSkill = newPilotSkill;

                host.AddPilotSkillModifier(this);
            }

            public void ModifyPilotSkill(ref int pilotSkill)
            {
                pilotSkill = newPilotSkill;
            }

            private void RemoveSwarmTacticsModifieer()
            {
                host.RemovePilotSkillModifier(this);
            }
        }

        private class ThweekTargetDecisionSubPhase : SubPhases.DecisionSubPhase
        {
            public override void PrepareDecision(Action callBack)
            {
                UI.ShowSkipButton();
                callBack();
            }

            public override void SkipButton()
            {
                UI.HideSkipButton();
                Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                Triggers.FinishTrigger();
            }
        }

        private class ThweekAbilityDecisionSubPhase : SubPhases.DecisionSubPhase
        {
            public override void PrepareDecision(Action callBack)
            {
                UI.ShowSkipButton();
                callBack();
            }

            public override void SkipButton()
            {
                UI.HideSkipButton();
                Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                Triggers.FinishTrigger();
            }
        }
    }
}

namespace Conditions
{
    public class Mimicked : Tokens.GenericToken
    {
        public Mimicked(GenericShip host) : base(host)
        {
            Name = "Thweek Condition";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/conditions/mimicked.png";
        }
    }

    public class Shadowed : Tokens.GenericToken
    {
        public Shadowed(GenericShip host) : base(host)
        {
            Name = "Thweek Condition";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/conditions/shadowed.png";
        }
    }
}
