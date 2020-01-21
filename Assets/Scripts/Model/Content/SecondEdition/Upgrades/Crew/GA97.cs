using Ship;
using Upgrade;
using System.Linq;
using ActionsList;
using System;
using System.Collections.Generic;
using Actions;
using SubPhases;
using Tokens;
using BoardTools;
using Conditions;

namespace UpgradesList.SecondEdition
{
    public class GA97 : GenericUpgrade
    {
        public GA97() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "GA-97",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                charges: 5,
                regensCharges: true,
                addAction: new ActionInfo(typeof(CalculateAction)),
                abilityType: typeof(Abilities.SecondEdition.GA97Ability)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/3af44c6d32812dc07238b40842d67b47.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class GA97Ability : GenericAbility
    {
        public int ChargesSpent { get; private set; }
        public ItsTheResistanceCondition AssignedCondition { get; private set; }

        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += TryRegisterOwnTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= TryRegisterOwnTrigger;
            Phases.Events.OnRoundStart -= CheckCharges;
            HostShip.OnShipIsRemoved -= DeployImmediately;
        }

        private void TryRegisterOwnTrigger()
        {
            if (HasAnotherFriendlyShip())
            {
                RegisterAbilityTrigger(TriggerTypes.OnSetupStart, AskToUseOwnAbility);
            }
            else
            {
                Messages.ShowErrorToHuman("GA-97: No another friendly ships");
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                AskHowChargesToSpend,
                descriptionLong: "Do you want to spend 3-5 charges to assign the \"It's The Resistance\" condition to another friendly ship?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private bool HasAnotherFriendlyShip()
        {
            return HostShip.Owner.Ships.Count > 1;
        }

        private void AskHowChargesToSpend(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            GA97DecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<GA97DecisionSubphase>(
                "GA-97 Decision",
                StartSelectShip
            );

            subphase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
            subphase.DescriptionLong = "Spend 3-5 charges";
            subphase.ImageSource = HostUpgrade;

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;
            subphase.ShowSkipButton = false;

            for (int i = 3; i < 6; i++)
            {
                int chargesCount = i;
                subphase.AddDecision(
                    chargesCount.ToString(),
                    delegate { SpendCharges(chargesCount); }
                );
            }

            subphase.Start();
        }

        private void SpendCharges(int chargesCount)
        {
            Messages.ShowInfo("GA-97: " + chargesCount.ToString() + " charges are spent");
            HostUpgrade.State.SpendCharges(chargesCount);
            ChargesSpent = chargesCount;

            DecisionSubPhase.ConfirmDecision();
        }

        private void StartSelectShip()
        {
            SelectTargetForAbility(
                ShipIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                "Assign the \"It's The Resistance\" condition to another friendly ship",
                HostUpgrade,
                showSkipButton: false
            );
        }

        private void ShipIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            AssignedCondition = new ItsTheResistanceCondition(TargetShip);
            TargetShip.Tokens.AssignCondition(AssignedCondition);

            Phases.Events.OnRoundStart += CheckCharges;
            HostShip.OnShipIsRemoved += DeployImmediately;

            Triggers.FinishTrigger();
        }

        private void DeployImmediately(GenericShip ship)
        {
            Messages.ShowInfo("It's The Resistance: Deployment");
            Phases.Events.OnRoundStart -= CheckCharges;
            HostShip.OnShipIsRemoved -= DeployImmediately;

            AssignedCondition.ImmediateDeployment = true;
            AssignedCondition.PreviousCurrentShip = Selection.ThisShip;
            RegisterAbilityTrigger(TriggerTypes.OnShipIsRemoved, AssignedCondition.SetupShip);
        }

        private void CheckCharges()
        {
            if (HostUpgrade.State.Charges == HostUpgrade.UpgradeInfo.Charges)
            {
                Messages.ShowInfo("It's The Resistance: Deployment");
                Phases.Events.OnRoundStart -= CheckCharges;
                HostShip.OnShipIsRemoved -= DeployImmediately;

                RegisterAbilityTrigger(TriggerTypes.OnRoundStart, AssignedCondition.SetupShip);
            }
        }

        private bool FilterTargets(GenericShip ship)
        {
            return ship.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && ship.ShipId != HostShip.ShipId;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        private class GA97DecisionSubphase : DecisionSubPhase { }
    }
}

namespace Conditions
{
    public class ItsTheResistanceCondition : GenericToken
    {
        public bool ImmediateDeployment { get; set; }
        public GenericShip PreviousCurrentShip { get; set; }
        public ItsTheResistanceCondition(GenericShip host) : base(host)
        {
            Name = ImageName = "It's The Resistance Condition";
            Temporary = false;

            Tooltip = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/7/73/Swz45_its-the-resistance.png";
        }

        public override void WhenAssigned()
        {
            Messages.ShowInfo(Host.PilotInfo.PilotName + "has been moved to the Reserve");
            Roster.MoveToReserve(Host);
        }

        public void SetupShip(object sender, EventArgs e)
        {
            Roster.ReturnFromReserve(Host);

            var subphase = Phases.StartTemporarySubPhaseNew<SetupShipMidgameSubPhase>(
                "Setup",
                 FinishDeployment
            );

            subphase.ShipToSetup = Host;
            subphase.SetupSide = Direction.All;
            subphase.DescriptionShort = Name;
            subphase.DescriptionLong = "Place yourself within range 1 of any table edge and beyond range 3 of any enemy ship.";
            subphase.SetupFilter = SetupFilter;

            subphase.Start();
        }

        private void FinishDeployment()
        {
            Messages.ShowInfo(Host.PilotInfo.PilotName + " has been placed");

            Host.Tokens.RemoveCondition(this);

            if (PreviousCurrentShip != null)
            {
                Selection.ChangeActiveShip(PreviousCurrentShip);
            }

            if (ImmediateDeployment)
            {
                Host.IsManeuverPerformed = true;

                Host.Tokens.AssignToken(
                    typeof(WeaponsDisabledToken),
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool SetupFilter()
        {
            bool result = true;

            foreach (GenericShip enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(Host.Owner.PlayerNo)).Ships.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(Host, enemyShip);
                if (distInfo.Range < 4)
                {
                    Messages.ShowErrorToHuman("Boba Fett: The range to the closest enemy is " + distInfo.Range + ", it must be beyond range 3");
                    return false;
                }
            }

            return result;
        }
    }
}