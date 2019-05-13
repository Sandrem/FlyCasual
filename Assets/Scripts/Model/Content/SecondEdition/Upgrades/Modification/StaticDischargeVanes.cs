using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class StaticDischargeVanes : GenericUpgrade
    {
        public StaticDischargeVanes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Static Discharge Vanes",
                UpgradeType.Modification,
                cost: 8,
                abilityType: typeof(Abilities.SecondEdition.StaticDischargeVanesAbility),
                seImageNumber: 76
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class StaticDischargeVanesAbility : GenericAbility
    {
        private Type TokenType;

        public override void ActivateAbility()
        {
            HostShip.BeforeTokenIsAssigned += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeTokenIsAssigned -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, Type tokenType)
        {
            if
            (
                !ship.IsStressed
                && (tokenType == typeof(Tokens.IonToken) || tokenType == typeof(Tokens.JamToken))
                && Board.GetShipsAtRange(HostShip, new Vector2(0, 1), Team.Type.Any).Count > 1
            )
            {
                TokenType = tokenType;
                RegisterAbilityTrigger(TriggerTypes.OnBeforeTokenIsAssigned, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HasEnemyInRange,
                UseOwnAbility,
                infoText: "Do you want to use Static Discharge Vanes?"
            );
        }

        private void UseOwnAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            SelectTargetForAbility(
                TargetIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostUpgrade.UpgradeInfo.Name,
                description: "You may choose another ship at range 0–1 and gain 1 stress token. If you do, the chosen ship gains that ion or jam token instead.",
                imageSource: HostUpgrade
            );
        }

        private int GetAiPriority(GenericShip ship)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo)
            {
                return 0;
            }
            else
            {
                return ship.PilotInfo.Cost;
            }
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterTargetsByRange(ship, 0, 1) && ship.ShipId != HostShip.ShipId;
        }

        private void TargetIsSelected()
        {
            SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();

            Messages.ShowInfo("Static Discharge Vanes: Token is transfered to " + TargetShip.PilotInfo.PilotName);

            HostShip.Tokens.AssignToken(
                typeof(Tokens.StressToken),
                delegate
                {
                    TargetShip.Tokens.AssignToken(
                        TokenType,
                        delegate
                        {
                            HostShip.Tokens.TokenToAssign = null;
                            Triggers.FinishTrigger();
                        }
                    );
                }
            );
        }

        private bool HasEnemyInRange()
        {
            return Board.GetShipsAtRange(HostShip, new Vector2(0, 1), Team.Type.Enemy).Count > 0;
        }
    }
}