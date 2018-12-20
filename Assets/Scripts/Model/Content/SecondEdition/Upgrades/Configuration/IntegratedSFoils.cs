using Upgrade;
using Ship;
using ActionsList;
using System;
using SubPhases;
using System.Collections.Generic;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class IntegratedSFoilsClosed : GenericDualUpgrade
    {
        public IntegratedSFoilsClosed() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Integrated S-Foils (Closed)",
                UpgradeType.Configuration,
                cost: 0,
                addAction: new ActionInfo(typeof(BarrelRollAction)),
                addActionLink: new LinkedActionInfo(typeof(FocusAction), typeof(BarrelRollAction)),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.T70XWing.T70XWing)),
                abilityType: typeof(Abilities.SecondEdition.IntegratedSFoilsClosedAbility)
            );

            IsSecondSide = true;
            AnotherSide = typeof(IntegratedSFoilsOpen);

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/e3b44142faf0f13e541bf674a6c06dbc.png";
        }
    }

    public class IntegratedSFoilsOpen : GenericDualUpgrade
    {
        public IntegratedSFoilsOpen() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Integrated S-Foils (Open)",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.T70XWing.T70XWing)),
                abilityType: typeof(Abilities.SecondEdition.IntegratedSFoilsOpenAbility)
            );

            AnotherSide = typeof(IntegratedSFoilsClosed);

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/49372b4efb974ff673a1b79441186fd5.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public abstract class IntegratedSFoilCommonAbility : GenericAbility
    {
        protected abstract bool AIWantsToFlip();

        protected abstract string FlipQuestion { get; }

        protected void TurnSFoilsToClosedPosition()
        {
            Phases.Events.OnGameStart -= TurnSFoilsToClosedPosition;
            HostShip.WingsClose();
        }

        protected void TurnSFoilsToAttackPosition(GenericShip ship)
        {
            HostShip.WingsOpen();
        }

        private void RegisterAskToUseFlip(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementActivation, AskToFlip);
        }

        protected void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(AIWantsToFlip, DoFlipSide, null, null, false, FlipQuestion);
        }

        protected void DoFlipSide(object sender, EventArgs e)
        {
            (HostUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }

        public override void ActivateAbility()
        {
            HostShip.OnMovementActivationStart += RegisterAskToUseFlip;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivationStart -= RegisterAskToUseFlip;
        }
    }

    public class IntegratedSFoilsClosedAbility : ServomotorSFoilCommonAbility
    {
        public override void ActivateAbility()
        {
            base.ActivateAbility();
            Phases.Events.OnGameStart += TurnSFoilsToClosedPosition;
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += ReduceNumberOfAttackDiceIfOutsideOfBullseye;
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            Phases.Events.OnGameStart -= TurnSFoilsToClosedPosition;
            TurnSFoilsToAttackPosition(HostShip);
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= ReduceNumberOfAttackDiceIfOutsideOfBullseye;
        }

        private void ReduceNumberOfAttackDiceIfOutsideOfBullseye(ref int value)
        {
            if (!HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Bullseye)) value--;
        }

        protected override string FlipQuestion
        {
            get
            {
                return string.Format("{0}: Open the S-Foils?", HostShip.PilotInfo.PilotName);
            }
        }

        protected override bool AIWantsToFlip()
        {
            /// TODO: Add more inteligence to this decision
            return true;
        }
    }

    public class IntegratedSFoilsOpenAbility : ServomotorSFoilCommonAbility
    {
        public override void ActivateAbility()
        {
            base.ActivateAbility();
            TurnSFoilsToAttackPosition(HostShip);
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            TurnSFoilsToClosedPosition();
        }

        protected override string FlipQuestion
        {
            get
            {
                return string.Format("{0}: Close the S-Foils?", HostShip.PilotInfo.PilotName);
            }
        }

        protected override bool AIWantsToFlip()
        {
            /// TODO: Add more inteligence to this decision
            return false;
        }
    }
}
