using Ship;
using Upgrade;
using Ship.TIEAdvanced;

namespace UpgradesList
{
    public class AdvancedTargetingComputer : GenericUpgrade
    {
        public AdvancedTargetingComputer() : base()
        {
            Type = UpgradeType.System;
            Name = "Advanced Targeting Computer";
            Cost = 5;
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
                Combat.CurentDiceRoll.AddDice(DieSide.Crit).ShowWithoutRoll();
                Combat.CurentDiceRoll.OrganizeDicePositions();

                targetLockLetter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
                Combat.Attacker.GetToken(typeof(Tokens.BlueTargetLockToken), targetLockLetter).CanBeUsed = false;

                Combat.Attacker.OnAttackPerformed += SetTargetLockCanBeUsed;
            }
            else
            {
                Messages.ShowInfoToHuman("Cannot use ability: no Target Lock on defender");
            }

            callBack();
        }

        private void SetTargetLockCanBeUsed()
        {
            Combat.Attacker.GetToken(typeof(Tokens.BlueTargetLockToken), targetLockLetter).CanBeUsed = true;

            Combat.Attacker.OnAttackPerformed -= SetTargetLockCanBeUsed;
        }

    }

}


