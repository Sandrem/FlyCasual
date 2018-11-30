using Upgrade;
using Ship;
using ActionsList;
using System;
using SubPhases;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class ServomotorSFoilsClosed : GenericDualUpgrade
    {
        public ServomotorSFoilsClosed() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Servomotor S-Foils (Closed)",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.T65XWing.T65XWing)),
                abilityType: typeof(Abilities.SecondEdition.ServomotorSFoilsClosedAbility),
                seImageNumber: 108
            );

            IsSecondSide = true;
            AnotherSide = typeof(ServomotorSFoilsAttack);
        }
    }

    public class ServomotorSFoilsAttack : GenericDualUpgrade
    {
        public ServomotorSFoilsAttack() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Servomotor S-Foils (Attack)",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.T65XWing.T65XWing)),
                abilityType: typeof(Abilities.SecondEdition.ServomotorSFoilsAttackAbility),
                seImageNumber: 108
            );

            AnotherSide = typeof(ServomotorSFoilsClosed);
        }
    }
}

namespace Abilities.SecondEdition
{
    public abstract class ServomotorSFoilCommonAbility : GenericAbility
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

    public class ServomotorSFoilsClosedAbility : ServomotorSFoilCommonAbility
    {
        public override void ActivateAbility()
        {
            base.ActivateAbility();
            Phases.Events.OnGameStart += TurnSFoilsToClosedPosition;
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += ReduceNumberOfAttackDice;
        }

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new BoostAction(), HostUpgrade);
            HostShip.ActionBar.AddActionLink(typeof(FocusAction), new BoostAction() { IsRed = true, Source = HostUpgrade });
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            Phases.Events.OnGameStart -= TurnSFoilsToClosedPosition;
            TurnSFoilsToAttackPosition(HostShip);
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= ReduceNumberOfAttackDice;
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(BoostAction), HostUpgrade);
            HostShip.ActionBar.RemoveActionLink(typeof(FocusAction), typeof(BoostAction), HostUpgrade);
        }

        private void ReduceNumberOfAttackDice(ref int value)
        {
            value--;
        }

        protected override string FlipQuestion
        {
            get
            {
                return string.Format("{0}: Open the S-Foils?", HostShip.PilotName);
            }
        }

        protected override bool AIWantsToFlip()
        {
            /// TODO: Add more inteligence to this decision
            return true;
        }
    }

    public class ServomotorSFoilsAttackAbility : ServomotorSFoilCommonAbility
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
                return string.Format("{0}: Close the S-Foils?", HostShip.PilotName);
            }
        }

        protected override bool AIWantsToFlip()
        {
            /// TODO: Add more inteligence to this decision
            return false;
        }
    }
}
