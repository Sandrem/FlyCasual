using Ship;
using Upgrade;
using SubPhases;
using BoardTools;
using SquadBuilderNS;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class ZeroDashZeroDashZero : GenericUpgrade
    {
        public ZeroDashZeroDashZero() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "0-0-0",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum, Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.ZeroDashZeroDashZeroAbility),
                seImageNumber: 127
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(367, 1),
                new Vector2(200, 200)
            );
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            if (squadList.SquadFaction == Faction.Scum) return true;

            if (squadList.SquadFaction == Faction.Imperial)
            {
                if (squadList.HasUpgrade("Darth Vader") || squadList.HasPilot("Darth Vader"))
                {
                    return true;
                }
                else
                {
                    Messages.ShowError("0-0-0 cannot be in an Imperial squad without Darth Vader");
                    return false;
                }
            }

            return false;
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
            if (Board.GetShipsAtRange(HostShip, new Vector2(0, 1), Team.Type.Enemy).Count > 0)
            {
                SelectTargetForAbility(
                    ShipForZeroIsSelected,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    HostUpgrade.UpgradeInfo.Name,
                    "Gain 1 calculate token unless the target ship chooses to gain 1 stress token.",
                    HostUpgrade
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
            int priority = ship.PilotInfo.Cost;

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
                "Do you want to get a Stress token?\nIf not, the opponent's ship will get a Calculate token.",
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
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " assigned a stress token to " + TargetShip.PilotInfo.PilotName);
            TargetShip.Tokens.AssignToken(typeof(Tokens.StressToken), FinishAbility);
        }

        private void AllowCalculate(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " granted " + HostShip.PilotInfo.PilotName + " a Calculate token");
            HostShip.Tokens.AssignToken(typeof(Tokens.CalculateToken), FinishAbility);
        }

        private void FinishAbility()
        {
            Selection.DeselectAllShips();
            Triggers.FinishTrigger();
        }
    }
}