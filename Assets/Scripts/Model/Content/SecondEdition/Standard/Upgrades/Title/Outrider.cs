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
                cost: 0,
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
            //to see if host ship moved thru obstacles, use HostShip.ObstaclesHit instead of HostShip.IsHitObstacles
            //because HostShip.IsHitObstacles always returns false when HostShip.IsIgnoreObstacles = true (ex. Dash Rendar)
            if (HostShip.ObstaclesHit.Count > 0 || HostShip.IsLandedOnObstacle)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, CheckAbility);
            }
        }

        private void CheckAbility(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasTokenByColor(TokenColors.Orange) || HostShip.Tokens.HasTokenByColor(TokenColors.Red))
            {
                OutriderAbilityDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<OutriderAbilityDecisionSubPhase>(
                    "Outrider: Select token to remove",
                    Triggers.FinishTrigger
                );
                subphase.SourceUpgrade = HostUpgrade;
                subphase.Start();
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
        public GenericUpgrade SourceUpgrade;

        public override void PrepareCustomDecisions()
        {
            DescriptionShort = "Outrider";
            DescriptionLong = "Select a token to remove";
            ImageSource = SourceUpgrade;

            DecisionOwner = Selection.ThisShip.Owner;
            DefaultDecisionName = decisions.First().Name;
        }
    }
}
