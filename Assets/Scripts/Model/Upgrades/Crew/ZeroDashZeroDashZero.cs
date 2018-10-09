using Upgrade;
using Ship;
using Abilities;
using RuleSets;
using Abilities.SecondEdition;
using Movement;
using SquadBuilderNS;
using UnityEngine;
using SubPhases;

namespace UpgradesList
{
    public class ZeroDashZeroDashZero : GenericUpgrade, ISecondEditionUpgrade
    {
        public ZeroDashZeroDashZero() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "0-0-0";
            Cost = 3;

            isUnique = true;

            UpgradeAbilities.Add(new ZeroDashZeroDashZeroAbility());

            UpgradeRuleType = typeof(SecondEdition);

            SEImageNumber = 127;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            //
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum || ship.faction == Faction.Imperial;
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = false;

            if (squadList.SquadFaction == Faction.Scum)
            {
                result = true;
            }

            if (squadList.SquadFaction == Faction.Imperial)
            {
                foreach (var shipHolder in squadList.GetShips())
                {
                    if (shipHolder.Instance.PilotName == "Darth Vader")
                    {
                        return true;
                    }

                    foreach (var upgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                    {
                        if (upgrade.Name == "Darth Vader")
                        {
                            return true;
                        }
                    }
                }

                if (result != true)
                {
                    Messages.ShowError("0-0-0 cannot be in Imperial squad without Darth Vader");
                }

            }

            return result;
        }

    }
}

namespace Abilities.SecondEdition
{
    public class ZeroDashZeroDashZeroAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, TryStartZeroAbility);
        }

        private void TryStartZeroAbility(object sender, System.EventArgs e)
        {
            if (BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(0, 1), Team.Type.Enemy).Count > 0)
            {
                SelectTargetForAbility(
                    ShipForZeroIsSelected,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    HostUpgrade.Name,
                    "Gain 1 calculate token unless target ship chooses to gain 1 stress token",
                    HostUpgrade.ImageUrl
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.Enemy) && FilterTargetsByRange(ship, 0, 1);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = ship.Cost;

            if (ship.IsStressed) priority += 100;

            return priority;
        }

        private void ShipForZeroIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            Selection.ChangeActiveShip(TargetShip);

            AskOpponent(
                AiUseByDefault,
                GetStress,
                AllowCalculate,
                "Do you want to get a Stress token?\nIf no - opponent's ship will get a Calculate token",
                showSkipButton: false
            );
        }

        private bool AiUseByDefault()
        {
            return !TargetShip.IsStressed;
        }

        private void GetStress(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Messages.ShowInfo(HostUpgrade.Name + ": A Stress token was assigned");
            TargetShip.Tokens.AssignToken(typeof(Tokens.StressToken), FinishAbility);
        }

        private void AllowCalculate(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Messages.ShowInfo(HostUpgrade.Name + ": A Calculate token was assigned");
            HostShip.Tokens.AssignToken(typeof(Tokens.CalculateToken), FinishAbility);
        }

        private void FinishAbility()
        {
            Selection.DeselectAllShips();
            Triggers.FinishTrigger();
        }
    }
}