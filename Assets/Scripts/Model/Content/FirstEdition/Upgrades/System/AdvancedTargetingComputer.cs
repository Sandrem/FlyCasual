using Ship;
using Upgrade;
using ActionsList;
using System;
using System.Collections.Generic;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class AdvancedTargetingComputer : GenericUpgrade
    {
        public AdvancedTargetingComputer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Adv. Targeting Computer",
                UpgradeType.Sensor,
                cost: 5,
                abilityType: typeof(Abilities.FirstEdition.AdvancedTargetingComputerAbility),
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.TIEAdvanced.TIEAdvanced))
            );

            ImageUrl = ImageUrls.GetImageUrl(this, "advanced-targeting-computer");
        }
    }
}

namespace Abilities.FirstEdition
{
    public class AdvancedTargetingComputerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AdvancedTargetingComputerDiceModification;
            HostShip.Ai.OnGetActionPriority += IncreaseAILockPriority;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AdvancedTargetingComputerDiceModification;
            HostShip.Ai.OnGetActionPriority -= IncreaseAILockPriority;
        }

        private void IncreaseAILockPriority(GenericAction action, ref int priority)
        {
            if (action is TargetLockAction && TargetLockAction.HasValidLockTargetsAndNoLockOnShipInRange(HostShip)) priority = 55;
        }

        private void AdvancedTargetingComputerDiceModification(GenericShip host)
        {
            GenericAction newAction = new AdvancedTargetingComputerActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = host
            };
            host.AddAvailableDiceModificationOwn(newAction);
        }
    }
}

namespace ActionsList
{
    public class AdvancedTargetingComputerActionEffect : GenericAction
    {
        private List<char> targetLockLetters = new List<char>();

        public AdvancedTargetingComputerActionEffect()
        {
            Name = DiceModificationName = "Advanced Targeting Computer";
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            result = 110;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack
                && ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender)
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                Combat.CurrentDiceRoll.AddDiceAndShow(DieSide.Crit);

                targetLockLetters = ActionsHolder.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);
                foreach (char targetLockLetter in targetLockLetters)
                {
                    Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), targetLockLetter).CanBeUsed = false;
                }

                Combat.Attacker.OnAttackFinish += SetTargetLockCanBeUsed;
            }
            else
            {
                Messages.ShowInfoToHuman("Cannot use ability: no Target Lock on defender");
            }

            callBack();
        }

        private void SetTargetLockCanBeUsed(GenericShip ship)
        {
            foreach (char targetLockLetter in targetLockLetters)
            {
                BlueTargetLockToken ownTargetLockToken = (BlueTargetLockToken)Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), targetLockLetter);
                if (ownTargetLockToken != null) ownTargetLockToken.CanBeUsed = true;
            }

            Combat.Attacker.OnAttackFinish -= SetTargetLockCanBeUsed;
        }

    }

}