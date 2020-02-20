using Actions;
using ActionsList;
using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ProudTradition : GenericDualUpgrade
    {
        public ProudTradition() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Proud Tradition",
                UpgradeType.Talent,
                cost: 2,
                restriction: new FactionRestriction(Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.ProudTraditionAbility)
            );

            SelectSideOnSetup = false;
            AnotherSide = typeof(FalseTradition);

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/09ec8bb3b37800437bbff7963db6aec6.png";
        }
    }
    public class FalseTradition : GenericDualUpgrade
    {
        public FalseTradition() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "False Tradition",
                UpgradeType.Talent,
                abilityType: typeof(Abilities.SecondEdition.FalseTraditionAbility)
            );

            AnotherSide = typeof(ProudTradition);
            IsSecondSide = true;

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/a060961e9ee792e605c75aaf6d65ad34.png";
        }
    }
}


namespace Abilities.SecondEdition
{
    //Setup: Equip this side faceup. While you have 2 or fewer stress tokens, you may perform [Focus] actions even while stressed. 
    //After you perform an attack, if you are stressed, the defender may spend 1 focus token or suffer 1 [Critical Hit] damage to flip this card.
    public class ProudTraditionAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += CheckAbilityRestrictions;
            HostShip.OnTokenIsRemoved += CheckAbilityRestrictions;

            HostShip.OnAttackFinishAsAttacker += RegisterFlipTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= CheckAbilityRestrictions;
            HostShip.OnTokenIsRemoved -= CheckAbilityRestrictions;

            HostShip.OnAttackFinishAsAttacker -= RegisterFlipTrigger;

            if (set) HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Remove(typeof(FocusAction));
        }

        private bool set = false;

        private void CheckAbilityRestrictions(GenericShip ship, Type type)
        {
            if (type == typeof(StressToken))
            {
                if (!set && HostShip.Tokens.CountTokensByType(typeof(StressToken)) <= 2)
                {
                    HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Add(typeof(FocusAction));
                    set = true;
                }
                else if (set && HostShip.Tokens.CountTokensByType(typeof(StressToken)) > 2)
                {
                    HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Remove(typeof(FocusAction));
                    set = false;
                }
            }
        }

        private void RegisterFlipTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskToFlip);
        }

        protected void AskToFlip(object sender, EventArgs e)
        {
            if (HostShip.IsStressed)
            {
                var decisionSubPhase = Phases.StartTemporarySubPhaseNew<DecisionSubPhase>(
                    Name,
                    Triggers.FinishTrigger
                );
                
                if (Combat.Defender.Tokens.HasToken<FocusToken>())
                {
                    decisionSubPhase.DescriptionShort = "Spend 1 focus token or suffer 1 Critical Hit to flip " + HostName + "?";
                    decisionSubPhase.AddDecision("Focus", SpendFocus);
                }
                else
                {
                    decisionSubPhase.DescriptionShort = "Suffer 1 Critical Hit to flip " + HostName + "?";
                }

                decisionSubPhase.AddDecision("Critical Hit", SufferDamage);

                decisionSubPhase.AddDecision("Skip", delegate { DecisionSubPhase.ConfirmDecision(); } );

                decisionSubPhase.DefaultDecisionName = GetAIDecision();
                decisionSubPhase.RequiredPlayer = Combat.Defender.Owner.PlayerNo;
                decisionSubPhase.ShowSkipButton = false;

                decisionSubPhase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
        
        private string GetAIDecision()
        {
            if (Combat.Defender.State.HullCurrent == 0) return "Critical Hit";
            if (Combat.Defender.Tokens.HasToken<FocusToken>()) return "Focus";
            if (Combat.Defender.State.ShieldsCurrent > 0 || Combat.Defender.State.HullCurrent > 2) return "Critical Hit";
            return "Skip";
        }

        private void SpendFocus(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            if (Combat.Defender.Tokens.HasToken<FocusToken>())
            {
                Messages.ShowInfo(Combat.Defender.PilotInfo.PilotName + " spends 1 Focus token to flip " + HostName);
                Combat.Defender.Tokens.SpendToken(typeof(FocusToken), Flip);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SufferDamage(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Messages.ShowInfo(Combat.Defender.PilotInfo.PilotName + " suffers 1 Critical Hit to flip " + HostName);
            DealDamageToShip(Combat.Defender, 1, true, Flip);
        }


        private void Flip()
        {
            (HostUpgrade as GenericDualUpgrade).Flip();
            Triggers.FinishTrigger();
        }
    }

    //Treat your [Focus] actions as red.
    public class FalseTraditionAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckActionComplexity += TreatFocusAsRed;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckActionComplexity -= TreatFocusAsRed;
        }

        private void TreatFocusAsRed(GenericAction action, ref ActionColor color)
        {
            if (action is FocusAction)
            {
                color = ActionColor.Red;
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Focus action is treated as red");
            }
        }
    }
}
