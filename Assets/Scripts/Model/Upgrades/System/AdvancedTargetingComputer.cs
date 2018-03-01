using Ship;
using Upgrade;
using Ship.TIEAdvanced;
using Tokens;

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
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEAdvanced;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += AdvancedTargetingComputerDiceModification;
        }

        private void AdvancedTargetingComputerDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.AdvancedTargetingComputerActionEffect()
            {
                ImageUrl = ImageUrl,
                Host = host
            };
            host.AddAvailableActionEffect(newAction);
        }
    }
}

namespace ActionsList
{
    public class AdvancedTargetingComputerActionEffect : GenericAction
    {
        private char targetLockLetter;

        public AdvancedTargetingComputerActionEffect()
        {
            Name = EffectName = "Advanced Targeting Computer";
        }
        
        public override int GetActionEffectPriority()
        {
            int result = 0;

            result = 110;
            
            return result;            
        }

        public override bool IsActionEffectAvailable()
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

                targetLockLetter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
                Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), targetLockLetter).CanBeUsed = false;

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
            BlueTargetLockToken ownTargetLockToken = (BlueTargetLockToken)Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), targetLockLetter);
            if (ownTargetLockToken != null) ownTargetLockToken.CanBeUsed = true;

            Combat.Attacker.OnAttackFinish -= SetTargetLockCanBeUsed;
        }

    }

}


