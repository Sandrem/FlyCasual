using Upgrade;
using Ship;
using ActionsList;
using System;
using SubPhases;
using System.Collections.Generic;
using Actions;
using System.Linq;
using Tokens;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class StabilizedSFoilsClosed : GenericDualUpgrade
    {
        public StabilizedSFoilsClosed() : base()
        {
            IsHidden = true;
            NameCanonical = "stabilizedsfoils-anotherside";

            UpgradeInfo = new UpgradeCardInfo(
                "Stabilized S-Foils (Closed)",
                UpgradeType.Configuration,
                cost: 2,
                addAction: new ActionInfo(typeof(ReloadAction), ActionColor.Red), 
                addActionLink: new LinkedActionInfo(typeof(BarrelRollAction), typeof(EvadeAction)),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.ASF01BWing.ASF01BWing)),
                abilityType: typeof(Abilities.SecondEdition.StabilizedSFoilsClosedAbility)
            );

            IsSecondSide = true;
            AnotherSide = typeof(StabilizedSFoilsOpen);

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/52/d5/52d52e74-c744-40b7-ad74-c98bcb04016c/swz66_stabilized-s-foils-closed.png";
        }
    }

    public class StabilizedSFoilsOpen : GenericDualUpgrade
    {
        public StabilizedSFoilsOpen() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Stabilized S-Foils (Open)",
                UpgradeType.Configuration,
                cost: 2,
                addActionLink: new LinkedActionInfo(typeof(BarrelRollAction), typeof(TargetLockAction)),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.ASF01BWing.ASF01BWing)),
                abilityType: typeof(Abilities.SecondEdition.StabilizedSFoilsOpenAbility)
            );

            AnotherSide = typeof(StabilizedSFoilsClosed);

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/64/9d/649d6f6e-8cba-404f-a00f-9a36a0076e34/swz66_stabilized-s-foils-open.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public abstract class StabilizedSFoilsCommonAbility : GenericAbility
    {
        protected abstract bool AIWantsToFlip();

        protected abstract string FlipQuestion { get; }

        protected void TurnSFoilsToClosedPosition()
        {
            Phases.Events.OnGameStart -= TurnSFoilsToClosedPosition;
            //HostShip.WingsClose();
        }

        protected void TurnSFoilsToAttackPosition(GenericShip ship)
        {
            //HostShip.WingsOpen();
        }

        private void RegisterAskToUseFlip(GenericShip ship)
        {
            if (!HostShip.Damage.HasFaceupCards)
                RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskToFlip);
        }

        protected void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AIWantsToFlip,
                DoFlipSide,
                descriptionLong: FlipQuestion,
                imageHolder: HostUpgrade
            );
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

    public class StabilizedSFoilsClosedAbility : StabilizedSFoilsCommonAbility
    {
        public override void ActivateAbility()
        {
            base.ActivateAbility();
            Phases.Events.OnGameStart += TurnSFoilsToClosedPosition;
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            Phases.Events.OnGameStart -= TurnSFoilsToClosedPosition;
            TurnSFoilsToAttackPosition(HostShip);
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

    public class StabilizedSFoilsOpenAbility : StabilizedSFoilsCommonAbility
    {
        // After you perform an attack, you may spend your lock on the defender to perform a bonus cannon attack
        // against that ship using a cannon upgrade you have not attacked with this turn.
        
        private IShipWeapon AlreadyUsedCannon;
        private GenericShip Defender;

        public override void ActivateAbility()
        {
            base.ActivateAbility();
            TurnSFoilsToAttackPosition(HostShip);
            HostShip.OnAttackFinishAsAttacker += CheckAbility;
            HostShip.Ai.OnGetWeaponPriority += ModifyWeaponPriority;
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            TurnSFoilsToClosedPosition();
            HostShip.OnAttackFinishAsAttacker -= CheckAbility;
            HostShip.Ai.OnGetWeaponPriority -= ModifyWeaponPriority;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (!ActionsHolder.HasTargetLockOn(HostShip, Combat.Defender)) return;

            if (HostShip.IsCannotAttackSecondTime) return;

            var availableCannons = HostShip.UpgradeBar
                .GetInstalledUpgrades(UpgradeType.Cannon)
                .Where(c => c != Combat.ShotInfo.Weapon);

            if (availableCannons.Any())
            {
                Defender = Combat.Defender;
                AlreadyUsedCannon = Combat.ShotInfo.Weapon.WeaponType == WeaponTypes.Cannon ? Combat.ShotInfo.Weapon : null;
                HostShip.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
            }
            else
            {
                Defender = null;
                AlreadyUsedCannon = null;
            }
        }

        private void ModifyWeaponPriority(GenericShip targetShip, IShipWeapon weapon, ref int priority)
        {
            //If this is first attack, and ship has lock and can also attack with another cannon priorize non-cannon weapon
            if
            (
                !HostShip.IsAttackPerformed
                && weapon.WeaponType != WeaponTypes.Cannon
                && ActionsHolder.HasTargetLockOn(HostShip, targetShip)
                && CanAttackTargetWithBothThisAndCannon(targetShip, weapon)
            )
            {
                priority += 2000;
            }
        }

        private bool CanAttackTargetWithBothThisAndCannon(GenericShip targetShip, IShipWeapon weapon)
        {
            //AI tries to check non-cannon weapon first
            ShotInfo shot = new ShotInfo(HostShip, targetShip, weapon);

            var hasCannonShot = HostShip.UpgradeBar
                .GetInstalledUpgrades(UpgradeType.Cannon)
                .Where(c => c != weapon)
                .Any(c =>
                {
                    var cannon = c as IShipWeapon;
                    if (cannon == null) return false;

                    return new ShotInfo(HostShip, targetShip, cannon).IsShotAvailable;
                });
            
            return shot.IsShotAvailable && hasCannonShot;
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSecondAttackTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, PerformBonusAttack);
        }

        private void PerformBonusAttack(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishAdditionalAttack,
                    IsAllowedAttack,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may spend your lock to perform a bonus cannon attack against the same ship",
                    HostUpgrade,
                    payAttackCost: SpendLock
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void SpendLock(Action callback)
        {
            List<char> tlLetter = ActionsHolder.GetTargetLocksLetterPairs(HostShip, Combat.Defender);
            if (tlLetter.Any())
            {
                HostShip.Tokens.SpendToken(typeof(BlueTargetLockToken), callback, tlLetter.First());
            }
            else
            {
                callback();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            HostShip.IsAttackPerformed = true;

            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }

        private bool IsAllowedAttack(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            if (defender != Defender)
            {
                if (!isSilent) Messages.ShowError("Your attack must be against the same ship");
                return false;
            }

            if (weapon == AlreadyUsedCannon)
            {
                if (!isSilent) Messages.ShowError("You must use a cannon you have not attacked with this round");
                return false;
            }

            if (weapon.WeaponType != WeaponTypes.Cannon)
            {
                if (!isSilent) Messages.ShowError("Your attack must use a cannon");
                return false;
            }

            return true;
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
