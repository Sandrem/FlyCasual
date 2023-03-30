using Ship;
using Upgrade;
using ActionsList;
using System;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class SensorJammer : GenericUpgrade
    {
        public SensorJammer() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Sensor Jammer",
                UpgradeType.Sensor,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.SensorJammerAbility)
            );

            ImageUrl = "https://i.imgur.com/hN4ZDsy.jpg";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SensorJammerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite += SensorJammerActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite -= SensorJammerActionEffect;
        }

        private void SensorJammerActionEffect(GenericShip host)
        {
            GenericAction newAction = new SensorJammerActionEffect()
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
    public class SensorJammerActionEffect : GenericAction
    {

        public SensorJammerActionEffect()
        {
            Name = DiceModificationName = "Sensor Jammer";
            DiceModificationTiming = DiceModificationTimingType.Opposite;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            result = 100;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack
                && Combat.DiceRollAttack.RegularSuccesses > 0
                && HasLockOfAnotherFriendlyShip(Combat.Attacker))
            {
                result = true;
            }

            return result;
        }

        private bool HasLockOfAnotherFriendlyShip(GenericShip attacker)
        {
            foreach (GenericToken token in attacker.Tokens.GetAllTokens())
            {
                if ((token is RedTargetLockToken)
                    && ((token as RedTargetLockToken).OtherTargetLockTokenOwner is GenericShip)
                    && ((token as RedTargetLockToken).OtherTargetLockTokenOwner as GenericShip).Owner == HostShip.Owner)
                {
                    return true;
                }
            }

            return false;
        }

        public override void ActionEffect(Action callBack)
        {
            Combat.DiceRollAttack.ChangeOne(DieSide.Success, DieSide.Focus, true);
            callBack();
        }

    }

}