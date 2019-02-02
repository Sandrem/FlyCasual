using Ship;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class SensorCluster : GenericUpgrade
    {
        public SensorCluster() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Sensor Cluster",
                UpgradeType.Tech,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.SensorClusterAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class SensorClusterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += SensorClusterActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= SensorClusterActionEffect;
        }

        private void SensorClusterActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SensorClusterActionEffect()
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
    public class SensorClusterActionEffect : GenericAction
    {

        public SensorClusterActionEffect()
        {
            Name = DiceModificationName = "Sensor Cluster";
            DiceModificationTiming = DiceModificationTimingType.Normal;
            TokensSpend.Add(typeof(FocusToken));
        }

        public override int GetDiceModificationPriority()
        {
            return (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes) ? 40 : 0;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence &&
                Combat.DiceRollDefence.Blanks > 0 &&
                HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            HostShip.Tokens.RemoveToken(typeof(FocusToken), delegate
            {
                Combat.DiceRollDefence.ChangeOne(DieSide.Blank, DieSide.Success, false);
                callBack();
            });

        }

    }

}
