﻿using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class AllianceOverhaul : GenericUpgrade
    {
        public AllianceOverhaul() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Alliance Overhaul",
                UpgradeType.Title,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.ARC170.ARC170)),
                abilityType: typeof(Abilities.FirstEdition.AllianceOverhaulAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                if (Combat.ShotInfo.InPrimaryArc) diceCount++;
            }
        }

        private void CheckFocusToCritAuxilaryArc(GenericShip ship)
        {
            if (!Combat.ShotInfo.InPrimaryArc)
            {
                HostShip.AddAvailableDiceModificationOwn(new ActionsList.AllianceOverhaulDiceModification());
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