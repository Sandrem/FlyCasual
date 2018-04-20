using Ship;
using Upgrade;
using Abilities;
using ActionsList;
using System;
using System.Collections.Generic;
using SubPhases;
using UnityEngine;
using Conditions;
using Tokens;

namespace UpgradesList
{
    public class TargetingScrambler : GenericUpgrade
    {
        public TargetingScrambler() : base()
        {
            Types.Add(UpgradeType.System);
            Name = "Targeting Scrambler";
            Cost = 0;

            isUnique = true;

            UpgradeAbilities.Add(new TargetingScramblerAbility());
        }        
    }
}

namespace Abilities
{
    public class TargetingScramblerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnPlanningPhaseStart += RegisterAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.OnPlanningPhaseStart -= RegisterAbilityTrigger;
        }

        private void RegisterAbilityTrigger()
        {
            int enemiesInRange = Board.BoardManager.GetShipsAtRange(HostShip, new Vector2(1, 3), Team.Type.Enemy).Count;
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
                true,
                null,
                HostUpgrade.Name,
                "Choose another ship to assign \"Scrambled\" condition to it.\nYou will receive weapons disabled token.",
                HostUpgrade.ImageUrl
            );
        }

        private void AssignScrambledCondition()
        {
            Messages.ShowInfo(string.Format("\"Scrambled\" condition is assigned to {0}", TargetShip.PilotName));

            TargetShip.Tokens.AssignCondition(new ScrambledCondition(TargetShip));
            TargetShip.OnTryAddAvailableActionEffect += UseDiceModificationRestriction;
            Phases.OnCombatPhaseEnd_NoTriggers += RemoveScrambledCondition;

            HostShip.Tokens.AssignToken(
                new WeaponsDisabledToken(HostShip),
                SelectShipSubPhase.FinishSelection
            );
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

        private void UseDiceModificationRestriction(GenericAction action, ref bool canBeUsed)
        {
            if (Combat.Defender.ShipId == HostShip.ShipId && Combat.ShotInfo.Range == 1)
            {
                Messages.ShowInfoToHuman("Targeting Scrambler: All dice modifications are disabled");
                canBeUsed = false;
            }
        }

        private void RemoveScrambledCondition()
        {
            Phases.OnCombatPhaseEnd_NoTriggers -= RemoveScrambledCondition;

            TargetShip.Tokens.RemoveCondition(typeof(ScrambledCondition));
            TargetShip.OnTryAddAvailableActionEffect -= UseDiceModificationRestriction;
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