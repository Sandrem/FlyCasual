using Upgrade;
using Ship;
using System.Linq;
using System;

namespace UpgradesList.SecondEdition
{
    public class C110P : GenericDualUpgrade
    {
        public C110P() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "C1-10P",
                UpgradeType.Astromech,
                cost: 7,
                isLimited: true,
                charges: 2,
                restriction: new FactionRestriction(Faction.Republic),
                abilityType: typeof(Abilities.SecondEdition.C110PAbility)
            );

            SelectSideOnSetup = false;
            AnotherSide = typeof(C110PErratic);

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/10/a8/10a8d369-5f71-4f3c-80c1-0b3dbed5d2ff/swz48_cards-c1-10p.png";
        }
    }

    public class C110PErratic : GenericDualUpgrade
    {
        public C110PErratic() : base()
        {
            IsHidden = true; // Hidden in Squad Builder only

            UpgradeInfo = new UpgradeCardInfo(
                "C1-10P (Erratic)",
                UpgradeType.Astromech,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.C110PErraticAbility)
            );

            AnotherSide = typeof(C110P);

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/16/fb/16fb5483-81db-4172-857b-08cdcb254a3a/swz48_cards-c1-10p_erratic.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class C110PAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += TryRegisterAbility;
            Phases.Events.OnEndPhaseStart_NoTriggers += CheckFlip;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= TryRegisterAbility;
            Phases.Events.OnEndPhaseStart_NoTriggers -= CheckFlip;
        }

        private void CheckFlip()
        {
            if (HostUpgrade.State.Charges == 0)
            {
                Messages.ShowInfo("C1-10P: Upgrade is flipped to another side");
                (HostUpgrade as GenericDualUpgrade).Flip();
            }
        }

        private void TryRegisterAbility(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                "C1-10P",
                NeverUseByDefault,
                ExecuteAbility,
                descriptionLong: "Do you want to spend Charge to perform a red Evade action (even while stressed)?",
                imageHolder: HostUpgrade
            );
        }

        private void ExecuteAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();

            // TODO: Add Chopper sounds

            HostShip.AskPerformFreeAction(
                new ActionsList.EvadeAction()
                {
                    Color = Actions.ActionColor.Red,
                    CanBePerformedWhileStressed = true
                },
                Triggers.FinishTrigger,
                descriptionShort: "C1-10P",
                descriptionLong: "You must perform a red Evade action (even while stressed)",
                imageHolder: HostUpgrade,
                isForced: true
            );
        }
    }

    public class C110PErraticAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToSelectShip);
        }

        private void AskToSelectShip(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                ShipIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: "C1-10P (Erratic)",
                description: "You must to assign Jam token to ship at range 0-1",
                imageSource: HostUpgrade,
                showSkipButton: false
            );
        }

        private void ShipIsSelected()
        {
            SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();

            // TODO: Add Chopper angry sounds

            TargetShip.Tokens.AssignToken(
                new Tokens.JamToken(TargetShip, HostShip.Owner),
                Triggers.FinishTrigger
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterTargetsByRange(ship, 0, 1);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int teamModifier = (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) ? 1 : 10;
            int tokensModifier = (teamModifier == 1) ? 0 : ship.Tokens.GetAllTokens().Count(n => n.TokenColor == Tokens.TokenColors.Green) * 10;
            int shipCostPriority = (teamModifier == 1) ? 200 - ship.PilotInfo.Cost : 200 + ship.PilotInfo.Cost;
            return (shipCostPriority + tokensModifier) * teamModifier;
        }
    }
}