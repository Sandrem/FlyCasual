using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Outrider : GenericUpgrade
    {
        public Outrider() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Outrider",
                UpgradeType.Title,
                cost: 14,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions
                (
                    new FactionRestriction(Faction.Rebel),
                    new ShipRestriction(typeof(Ship.SecondEdition.YT2400LightFreighter.YT2400LightFreighter))
                ),
                abilityType: typeof(Abilities.SecondEdition.OutriderAbility),
                seImageNumber: 105
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class OutriderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.AfterGotNumberOfDefenceDiceGlobal += CheckOutriderDiceChange;
            HostShip.OnMovementFinishSuccessfully += CheckOutriderTokensAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.AfterGotNumberOfDefenceDiceGlobal -= CheckOutriderDiceChange;
            HostShip.OnMovementFinishSuccessfully -= CheckOutriderTokensAbility;
        }

        private void CheckOutriderDiceChange(ref int count)
        {
            if (Combat.Attacker == HostShip && Combat.ShotInfo.IsObstructedByObstacle)
            {
                Messages.ShowInfo("Outrider: attack is obstructed,\n the defender rolls 1 fewer defense die");
                count--;
            }
        }

        private void CheckOutriderTokensAbility(GenericShip ship)
        {
            if (HostShip.IsHitObstacles || HostShip.IsLandedOnObstacle)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, CheckAbility);
            }
        }

        private void CheckAbility(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasTokenByColor(TokenColors.Orange) || HostShip.Tokens.HasTokenByColor(TokenColors.Red))
            {
                Phases.StartTemporarySubPhaseOld(
                    "Outrider: Select token to remove",
                    typeof(OutriderAbilityDecisionSubPhase),
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}

namespace SubPhases
{
    public class OutriderAbilityDecisionSubPhase : RemoveBadTokenDecisionSubPhase
    {
        public override void PrepareCustomDecisions()
        {
            InfoText = "Outrider: Select token to remove";
            DecisionOwner = Selection.ThisShip.Owner;
            DefaultDecisionName = decisions.First().Name;
        }
    }
}
