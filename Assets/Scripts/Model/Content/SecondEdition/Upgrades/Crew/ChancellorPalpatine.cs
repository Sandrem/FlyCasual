using Upgrade;
using Ship;
using ActionsList;
using System;
using SubPhases;
using Actions;
using BoardTools;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class ChancellorPalpatine : GenericDualUpgrade
    {
        public ChancellorPalpatine() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Chancellor Palpatine",
                UpgradeType.Crew,
                cost: 14,
                addForce: 1,
                addAction: new ActionInfo(typeof(CoordinateAction), ActionColor.Purple),
                restriction: new FactionRestriction(Faction.Republic, Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.ChancellorPalpatineAbility)
            );

            SelectSideOnSetup = false;
            AnotherSide = typeof(DarthSidious);

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/8218d7b903ea8c0c0f88375124a93a5f.png";
        }
    }

    public class DarthSidious : GenericDualUpgrade
    {
        public DarthSidious() : base()
        {
            IsHidden = true; // Hidden in Squad Builder only

            UpgradeInfo = new UpgradeCardInfo(
                "Darth Sidious",
                UpgradeType.Crew,
                cost: 14,
                addForce: 1,
                addAction: new ActionInfo(typeof(CoordinateAction), ActionColor.Purple),
                abilityType: typeof(Abilities.SecondEdition.DarthSidiousAbility)
            );

            AnotherSide = typeof(ChancellorPalpatine);
            IsSecondSide = true;

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/fe4d36bbd6f43ada43a5cf55354211c0.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChancellorPalpatineAbility : GenericAbility
    {
        //After you defend, if the attacker is at range 0-2, you may spend 1 force. If you do, the attacker gains 1 stress token.
        //During the End Phase, you may flip this card.
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += RegisterDefendAbility;
            Phases.Events.OnEndPhaseStart_Triggers += RegisterFlipAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= RegisterDefendAbility;
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterFlipAbility;
        }

        private void RegisterFlipAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskToFlip);
        }

        private void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, FlipCard, infoText: $"Flip {HostName}?");
        }

        private void FlipCard(object sender, EventArgs e)
        {
            (HostUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }

        private void RegisterDefendAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, UseAbility);
        }

        private void UseAbility(object sender, EventArgs e)
        {
            if (HostShip.State.Force > 0 && new DistanceInfo(HostShip, Combat.Attacker).Range <= 2)
            {
                AskToUseAbility(AlwaysUseByDefault, AssignStress);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AssignStress(object sender, EventArgs e)
        {
            HostShip.State.Force--;
            Messages.ShowInfo("Attacker gains 1 stress from Chancellor Palpatine");
            Combat.Attacker.Tokens.AssignToken(typeof(Tokens.StressToken), DecisionSubPhase.ConfirmDecision);
        }
    }

    public class DarthSidiousAbility : GenericAbility
    {
        //After you perform a purple coordinate action, the ship you coordinated gains 1 stress token. Then it gains 1 focus token or recovers 1 force.
        public override void ActivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected += CoordinateShipSelected;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected -= CoordinateShipSelected;
        }

        private void CoordinateShipSelected(GenericShip ship)
        {
            TargetShip = ship;
            ship.OnActionIsPerformed += RegisterAbility;            
        }

        private void RegisterAbility(GenericAction action)
        {
            TargetShip.OnActionIsPerformed -= RegisterAbility;
            RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, ShowDecision);
        }

        private void ShowDecision(object sender, EventArgs e)
        {
            var phase = Phases.StartTemporarySubPhaseNew<DarthSidiousDecisionSubPhase>("Darth Sidious", Triggers.FinishTrigger);
            phase.TargetShip = TargetShip;
            phase.Start();
        }

        protected class DarthSidiousDecisionSubPhase : DecisionSubPhase
        {
            public GenericShip TargetShip;

            public override void PrepareDecision(Action callBack)
            {
                InfoText = "Darth Sidious: Gain 1 focus token or recover 1 force?";

                DecisionViewType = DecisionViewTypes.TextButtons;

                AddDecision("Focus", GainFocus);
                AddDecision("Force", RecoverForce);

                DefaultDecisionName = GetDecisions().First().Name;
                ShowSkipButton = false;
                callBack();
            }

            private void GainFocus(object sender, EventArgs e)
            {
                TargetShip.Tokens.AssignToken(typeof(Tokens.FocusToken), ConfirmDecision);
            }

            private void RecoverForce(object sender, EventArgs e)
            {
                if (TargetShip.State.Force < TargetShip.State.MaxForce)
                    TargetShip.State.Force++;
                else
                    Messages.ShowErrorToHuman("Ship is not able to recover Force");

                ConfirmDecision();
            }
        }
    }
}
