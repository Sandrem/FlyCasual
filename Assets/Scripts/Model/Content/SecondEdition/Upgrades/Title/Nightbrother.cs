using Ship;
using Upgrade;
using SubPhases;
using Tokens;
using Movement;
using System;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class Nightbrother : GenericUpgrade
    {
        public Nightbrother() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Nightbrother",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Title,
                    UpgradeType.Modification
                },
                cost: 0,
                charges: 2,
                regensCharges: true,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Rebel, Faction.Scum),
                    new ShipRestriction(typeof(Ship.SecondEdition.GauntletFighter.GauntletFighter))
                ),
                addSlot: new UpgradeSlot(UpgradeType.Crew),
                abilityType: typeof(Abilities.SecondEdition.Nightbrother)
            );
            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/nightbrother.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class Nightbrother : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterOwnAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterOwnAbilityTrigger;
        }

        private void RegisterOwnAbilityTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, ChooseToken);
        }

        private void ChooseToken(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 1 && HostShip.Tokens.HasToken(typeof(Tokens.StressToken))&& HostShip.AssignedManeuver.ColorComplexity != MovementComplexity.Easy)
            {
                var decisionSubPhase = Phases.StartTemporarySubPhaseNew<DecisionSubPhase>(
                    Name,
                    Triggers.FinishTrigger
                );

                decisionSubPhase.DescriptionShort = HostName + ": Spend 2 charges to gain 1 focus or evade token?";

                decisionSubPhase.AddDecision(
                    "Focus",
                    delegate {
                        GainToken("Focus");
                    }
                );
                decisionSubPhase.AddDecision(
                     "Evade",
                     delegate {
                         GainToken("Evade");
                     }
                 );

                decisionSubPhase.DefaultDecisionName = HostShip.Tokens.HasToken<FocusToken>() ? "Evade" : "Focus";
                decisionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
                decisionSubPhase.ShowSkipButton = true;

                decisionSubPhase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void GainToken(string tokenName)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            if (HostUpgrade.State.Charges > 0)
            {
                var tokenType = tokenName == "Focus" ? typeof(FocusToken) : typeof(EvadeToken);

                Messages.ShowInfo(HostShip.PilotName + " gains 1 " + tokenName + " token");
                HostUpgrade.State.SpendCharges(2);
                HostShip.Tokens.AssignToken(tokenType, Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

    }

}