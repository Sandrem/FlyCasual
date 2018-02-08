using Ship;
using Ship.ARC170;
using Upgrade;

namespace UpgradesList
{
    public class AllianceOverhaul : GenericUpgradeSlotUpgrade
    {
        public AllianceOverhaul() : base()
        {
            Type = UpgradeType.Title;
            Name = "Alliance Overhaul";
            Cost = 0;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship is ARC170);
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Host.AfterGotNumberOfAttackDice += CheckAddDiceForPrimaryArc;
            Host.AfterGenerateAvailableActionEffectsList += CheckFocusToCritAuxilaryArc;
        }

        private void CheckAddDiceForPrimaryArc(ref int diceCount)
        {
            if (Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass))
            {
                if (Combat.ShotInfo.InPrimaryArc) diceCount++;
            }
        }

        private void CheckFocusToCritAuxilaryArc(GenericShip ship)
        {
            if (!Combat.ShotInfo.InPrimaryArc)
            {
                Host.AddAvailableActionEffect(new ActionsList.AllianceOverhaulDiceModification());
            }
        }
    }
}

namespace ActionsList
{

    public class AllianceOverhaulDiceModification : GenericAction
    {

        public AllianceOverhaulDiceModification()
        {
            Name = EffectName = "Alliance Overhaul";

            IsTurnsAllFocusIntoSuccess = true; // incorrect flag
            IsTurnsOneFocusIntoSuccess = true;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.Focuses;
                if (attackFocuses > 0) result = 70;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Crit);
            callBack();
        }

    }

}
