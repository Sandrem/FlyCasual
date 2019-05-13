using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class FeedbackArray : GenericUpgrade
    {
        public FeedbackArray() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Feedback Array",
                UpgradeType.Illicit,
                cost: 4,
                abilityType: typeof(Abilities.SecondEdition.FeedbackArrayAbility),
                seImageNumber: 60
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class FeedbackArrayAbility : GenericAbility
    {
        private List<GenericShip> ShipsR0;

        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskToUseOwnAbility);
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                CheckToUseAbility,
                ActivateOwnAbility,
                infoText: "Feedback Array: You may gain 1 ion token and 1 disarm token. If you do, each ship at range 0 suffers 1 damage."
            );
        }

        private void ActivateOwnAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(
                typeof(Tokens.IonToken),
                delegate
                {
                    HostShip.Tokens.AssignToken(
                        typeof(Tokens.WeaponsDisabledToken),
                        DealDamage
                    );
                }
            );
        }

        private bool CheckToUseAbility()
        {
            return !ActionsHolder.HasTarget(HostShip) && HasMoreEnemyShipsAtR0ThanFriendly() && HasMoreEnoughHp();
        }

        private bool HasMoreEnemyShipsAtR0ThanFriendly()
        {
            return HostShip.State.HullCurrent + HostShip.State.ShieldsCurrent > 1;
        }

        private bool HasMoreEnoughHp()
        {
            ShipsR0 = Board.GetShipsAtRange(HostShip, new Vector2(0, 0), Team.Type.Any);
            return (ShipsR0.Count(n => n.Owner.PlayerNo != HostShip.Owner.PlayerNo) > ShipsR0.Count(n => n.Owner.PlayerNo == HostShip.Owner.PlayerNo) - 1);
        }

        private void DealDamage()
        {
            Messages.ShowInfo("Feedback Array deals 1 damage to " + ShipsR0.Count + " ships");
            DealDamageToShips(ShipsR0, 1, false, Triggers.FinishTrigger);
        }
    }
}