using ActionsList;
using Conditions;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class TargetingScrambler : GenericUpgrade
    {
        public TargetingScrambler() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Targeting Scrambler",
                UpgradeType.System,
                cost: 0,
                abilityType: typeof(Abilities.FirstEdition.TargetingScramblerAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class TargetingScramblerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnPlanningPhaseStart += RegisterAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnPlanningPhaseStart -= RegisterAbilityTrigger;
        }

        private void RegisterAbilityTrigger()
        {
            int enemiesInRange = BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1, 3), Team.Type.Enemy).Count;
            if (enemiesInRange > 0) RegisterAbilityTrigger(TriggerTypes.OnPlanningSubPhaseStart, AskToUseTargetingScrambler);
        }

        private void AskToUseTargetingScrambler(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, SelecScrambledTarget);
        }

        private void SelecScrambledTarget(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            SelectTargetForAbility(
                AssignScrambledCondition,
                FilterAbilityTargets,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                "Choose another ship to assign the \"Scrambled\" condition to,\nyou will receive a weapons disabled token",
                HostUpgrade
            );
        }

        private void AssignScrambledCondition()
        {
            Messages.ShowInfo(string.Format("The \"Scrambled\" condition is assigned to {0}", TargetShip.PilotInfo.PilotName));

            TargetShip.Tokens.AssignCondition(typeof(ScrambledCondition));
            TargetShip.OnTryAddAvailableDiceModification += UseDiceModificationRestriction;
            Phases.Events.OnCombatPhaseEnd_NoTriggers += RemoveScrambledCondition;

            HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), SelectShipSubPhase.FinishSelection);
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 3);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            //Is not used by AI so far
            return 50;
        }

        private void UseDiceModificationRestriction(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (Combat.Defender.ShipId == HostShip.ShipId && Combat.ShotInfo.Range == 1)
            {
                Messages.ShowInfoToHuman("Targeting Scrambler: All dice modifications are disabled");
                canBeUsed = false;
            }
        }

        private void RemoveScrambledCondition()
        {
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= RemoveScrambledCondition;

            TargetShip.Tokens.RemoveCondition(typeof(ScrambledCondition));
            TargetShip.OnTryAddAvailableDiceModification -= UseDiceModificationRestriction;
        }
    }
}

namespace Conditions
{
    public class ScrambledCondition : GenericToken
    {
        public ScrambledCondition(GenericShip host) : base(host)
        {
            Name = "Scrambled Condition";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/conditions/scrambled.png";
        }
    }
}