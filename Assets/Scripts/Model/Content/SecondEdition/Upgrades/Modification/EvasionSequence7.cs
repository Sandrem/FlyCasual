using Upgrade;
using ActionsList;
using Obstacles;
using BoardTools;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class EvasionSequence7 : GenericUpgrade
    {
        public EvasionSequence7() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Evasion Sequence 7",
                UpgradeType.Modification,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.EvasionSequence7Ability)
            );

            ImageUrl = "https://i.imgur.com/a4AvvnZ.jpg";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EvasionSequence7Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckActionComplexity += CheckDecreaseComplexity;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckActionComplexity -= CheckDecreaseComplexity;
        }

        private void CheckDecreaseComplexity(GenericAction action, ref ActionColor color)
        {
            if (action is EvadeAction && color == ActionColor.Red)
            {
                if (IsNearObstacle()) color = ActionColor.White;
            }
        }

        private bool IsNearObstacle()
        {
            foreach (GenericObstacle obstacle in ObstaclesManager.GetPlacedObstacles())
            {
                ShipObstacleDistance shipObstacleDist = new ShipObstacleDistance(HostShip, obstacle);
                if (shipObstacleDist.Range == 1)
                {
                    Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": The Evade action is treated as a white action");
                    return true;
                }
            }

            return false;
        }
    }
}