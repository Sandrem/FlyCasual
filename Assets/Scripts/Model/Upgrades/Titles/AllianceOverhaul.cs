using Ship;
using Ship.ARC170;
using Upgrade;
using Abilities;
using Arcs;

namespace UpgradesList
{
    public class AllianceOverhaul : GenericUpgradeSlotUpgrade
    {
        public AllianceOverhaul() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Alliance Overhaul";
            Cost = 0;

            UpgradeAbilities.Add(new AllianceOverhaulAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship is ARC170);
        }
    }
}

namespace Abilities
{
    public class AllianceOverhaulAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAddDiceForPrimaryArc;
            HostShip.OnGenerateDiceModifications += CheckFocusToCritAuxilaryArc;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckAddDiceForPrimaryArc;
            HostShip.OnGenerateDiceModifications -= CheckFocusToCritAuxilaryArc;
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
                HostShip.AddAvailableDiceModification(new ActionsList.AllianceOverhaulDiceModification());
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
            Name = DiceModificationName = "Alliance Overhaul";

            IsTurnsAllFocusIntoSuccess = true; // incorrect flag
            IsTurnsOneFocusIntoSuccess = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override int GetDiceModificationPriority()
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
