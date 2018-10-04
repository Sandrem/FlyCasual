using Ship;
using Upgrade;
using Ship.TIEAdvanced;
using Tokens;
using Abilities;
using System.Collections.Generic;

namespace UpgradesList
{
    public class AdvancedTargetingComputer : GenericUpgrade
    {
        public AdvancedTargetingComputer() : base()
        {
            Types.Add(UpgradeType.System);
            Name = "Adv. Targeting Computer";
            Cost = 5;

            ImageUrl = ImageUrls.GetImageUrl(this, "advanced-targeting-computer.png");

            UpgradeAbilities.Add(new AdvancedTargetingComputerAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEAdvanced;
        }
    }
}

namespace Abilities
{
    public class AdvancedTargetingComputerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AdvancedTargetingComputerDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AdvancedTargetingComputerDiceModification;
        }

        private void AdvancedTargetingComputerDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.AdvancedTargetingComputerActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableDiceModification(newAction);
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

            if (Combat.AttackStep == CombatStep.Attack && Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender) && Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass))
            {
                result = true;
            }

            return result;
        }
        
        public override void ActionEffect(System.Action callBack)
        {
            if (Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                Combat.CurrentDiceRoll.AddDice(DieSide.Crit).ShowWithoutRoll();
                Combat.CurrentDiceRoll.OrganizeDicePositions();

                targetLockLetters = Actions.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);
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


