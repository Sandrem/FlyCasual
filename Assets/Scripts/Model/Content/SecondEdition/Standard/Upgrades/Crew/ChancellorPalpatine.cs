using Upgrade;
using Ship;
using ActionsList;
using System;
using SubPhases;
using Actions;
using BoardTools;
using System.Linq;
using UnityEngine;

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
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.ChancellorPalpatineAbility)
            );

            SelectSideOnSetup = false;
            AnotherSide = typeof(DarthSidious);

            Avatar = new AvatarInfo(
                Faction.Republic,
                new Vector2(272, 8)
            );
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

            Avatar = new AvatarInfo(
                Faction.Separatists,
                new Vector2(304, 10),
                new Vector2(75, 75)
            );

            NameCanonical = "chancellorpalpatine-sideb";
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
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                FlipCard,
                descriptionLong: "Do you want to flip this upgrade?",
                imageHolder: HostUpgrade
            );
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
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    AssignStress,
                    descriptionLong: "Do you want to spend 1 Force to assign the attacker 1 Stress Token?",
                    imageHolder: HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AssignStress(object sender, EventArgs e)
        {
            Messages.ShowInfo("Attacker gains 1 stress from Chancellor Palpatine");
            HostShip.State.SpendForce(
                1, 
                delegate { Combat.Attacker.Tokens.AssignToken(typeof(Tokens.StressToken), DecisionSubPhase.ConfirmDecision); }
            );
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
            HostShip.OnActionIsPerformed += RegisterAbility;
        }

        private void RegisterAbility(GenericAction action)
        {
            HostShip.OnActionIsPerformed -= RegisterAbility;
            RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AssignStress);
        }

        private void AssignStress(object sender, EventArgs e)
        {
            TargetShip.Tokens.AssignToken(typeof(Tokens.StressToken), ShowDecision);
        }

        private void ShowDecision()
        {
            var phase = Phases.StartTemporarySubPhaseNew<DarthSidiousDecisionSubPhase>("Darth Sidious", Triggers.FinishTrigger);
            phase.TargetShip = TargetShip;
            phase.DescriptionShort = "Darth Sidious";
            phase.DescriptionLong = "Gain 1 focus token or recover 1 force?";
            phase.ImageSource = HostUpgrade;
            phase.Start();
        }

        protected class DarthSidiousDecisionSubPhase : DecisionSubPhase
        {
            public GenericShip TargetShip;

            public override void PrepareDecision(Action callBack)
            {
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
                    TargetShip.State.RestoreForce();
                else
                    Messages.ShowErrorToHuman("Ship is not able to recover Force");

                ConfirmDecision();
            }
        }
    }
}
