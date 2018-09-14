using Upgrade;
using Ship;
using Abilities;
using System;
using Tokens;
using UnityEngine;
using RuleSets;

namespace UpgradesList
{

    public class CrackShot : GenericUpgrade, ISecondEditionUpgrade
    {

        public CrackShot() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Crack Shot";
            Cost = 1;

            AvatarOffset = new Vector2(43, 1);

            UpgradeAbilities.Add(new CrackShotAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            MaxCharges = 1;
            UsesCharges = true;

            Cost = 1;

            UpgradeAbilities.RemoveAll(a => a is CrackShotAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.CrackShotAbility());

            SEImageNumber = 1;
        }
    }
}

namespace Abilities
{
    public class CrackShotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults += CrackShotDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults -= CrackShotDiceModification;
        }

        private void CrackShotDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.CrackShotDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host,
                Source = this.HostUpgrade
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{
    public class CrackShotDiceModification : GenericAction
    {

        public CrackShotDiceModification()
        {
            Name = DiceModificationName = "Crack Shot";
            DiceModificationTiming = DiceModificationTimingType.CompareResults;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.DiceRollDefence.Successes <= Combat.DiceRollAttack.Successes) result = 100;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.DiceRollDefence.Successes > 0 && Combat.ShotInfo.InArc)
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollDefence.ChangeOne(DieSide.Success, DieSide.Blank, false);
            Source.TryDiscard(callBack);
        }

    }

}



namespace Abilities.SecondEdition
{
    public class CrackShotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults += CrackShotDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults -= CrackShotDiceModification;
        }

        private void CrackShotDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SecondEdition.CrackShotDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host,
                Source = this.HostUpgrade
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}


namespace ActionsList.SecondEdition
{
    //While you perform a primary weapon attack, if the defender is in your bullseye firing arc, before the neutralize results step, you may spend one charge to cancel one evade result.
    public class CrackShotDiceModification : ActionsList.CrackShotDiceModification
    {

        public CrackShotDiceModification() : base()
        {
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.DiceRollDefence.Successes > 0 && Source.Charges > 0 && Combat.Attacker == Host && Combat.ChosenWeapon is PrimaryWeaponClass && Combat.ShotInfo.InArcByType(Arcs.ArcTypes.Bullseye))
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(Action callBack)
        {
            Combat.DiceRollDefence.ChangeOne(DieSide.Success, DieSide.Blank, false);
            Source.SpendCharge(callBack);
        }

    }

}