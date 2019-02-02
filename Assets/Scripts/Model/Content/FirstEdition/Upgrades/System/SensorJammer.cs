using Ship;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class SensorJammer : GenericUpgrade
    {
        public SensorJammer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Sensor Jammer",
                UpgradeType.System,
                cost: 4,
                abilityType: typeof(Abilities.FirstEdition.SensorJammerAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            ActionsList.GenericAction newAction = new ActionsList.SensorJammerActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = host
            };
            host.AddAvailableDiceModification(newAction);
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

            if (Combat.AttackStep == CombatStep.Attack && Combat.DiceRollAttack.RegularSuccesses > 0)
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollAttack.ChangeOne(DieSide.Success, DieSide.Focus, true);
            callBack();
        }

    }

}