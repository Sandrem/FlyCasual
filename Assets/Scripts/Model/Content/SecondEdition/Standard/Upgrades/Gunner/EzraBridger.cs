using Upgrade;
using Ship;
using Arcs;
using System.Linq;
using BoardTools;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class EzraBridgerGunner : GenericUpgrade
    {
        public EzraBridgerGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ezra Bridger",
                UpgradeType.Gunner,
                cost: 10,
                isLimited: true,
                addForce: 1,
                abilityType: typeof(Abilities.SecondEdition.EzraBridgerGunnerAbility),
                restriction: new FactionRestriction(Faction.Rebel),
                seImageNumber: 96
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(397, 13)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EzraBridgerGunnerAbility : GenericAbility
    {
        private bool IsSecondAttackActive;

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckAbility;

            AddDiceModification(
                "Ezra Bridger",
                IsDiceModificationAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                1
            );
        }

        private int GetAiPriority()
        {
            return 90;
        }

        private bool IsDiceModificationAvailable()
        {
            return HostShip.IsStressed && Combat.AttackStep == CombatStep.Attack && IsSecondAttackActive;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckAbility;

            RemoveDiceModification();
        }

        private void CheckAbility(GenericShip ship)
        {
            IsSecondAttackActive = false;

            if (Combat.ShotInfo.Weapon.WeaponType != WeaponTypes.PrimaryWeapon) return;

            bool availableArcsArePresent = HostShip.ArcsInfo.Arcs.Any(a => a.ArcType == ArcType.SingleTurret && !a.WasUsedForAttackThisRound);
            if (availableArcsArePresent)
            {
                HostShip.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
            }
            else
            {
                Messages.ShowError(HostUpgrade.UpgradeInfo.Name + " does not have any valid arcs to use");
            }
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSecondAttackTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, AskToUseGunnerAbility);
        }

        private void AskToUseGunnerAbility(object sender, EventArgs e)
        {
            if (HostShip.State.Force > 0)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    NeverUseByDefault,
                    UseGunnerAbility,
                    descriptionLong: "Do you want to spend 1 Force to perform a bonus Turret attack from a Turret you have not attacked from this round? (If you do and you are stressed, you may reroll 1 attack die)",
                    imageHolder: HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseGunnerAbility(object sender, System.EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();
            if (!HostShip.IsCannotAttackSecondTime)
            {
                IsSecondAttackActive = true;

                HostShip.IsCannotAttackSecondTime = true;

                HostShip.State.SpendForce(
                    1,
                    delegate
                    {
                        Combat.StartSelectAttackTarget(
                            HostShip,
                            FinishAdditionalAttack,
                            IsUnusedTurretArcShot,
                            HostUpgrade.UpgradeInfo.Name,
                            "You may perform a bonus turret arc attack using another turret arc",
                            HostUpgrade
                        );
                    }
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time", HostShip.PilotInfo.PilotName));
                HostShip.State.SpendForce(1, Triggers.FinishTrigger);
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

        private bool IsUnusedTurretArcShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, defender, weapon);
            if (!shotInfo.ShotAvailableFromArcs.Any(a => a.ArcType == ArcType.SingleTurret && !a.WasUsedForAttackThisRound))
            {
                if (!isSilent) Messages.ShowError("Your attack must use a turret arc you have not already attacked from this round");
                return false;
            }

            if (!weapon.WeaponInfo.ArcRestrictions.Contains(ArcType.SingleTurret))
            {
                if (!isSilent) Messages.ShowError("Your attack must use a turret arc");
                return false;
            }

            return true;
        }
    }
}