using Upgrade;
using Ship;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class LightweightFrame : GenericUpgrade
    {
        public LightweightFrame() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Lightweight Frame",
                UpgradeType.Modification,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.LightweightFrameAbility)
            );
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship.ShipInfo.Agility < 3 && ship is TIE);
        }
    }
}

namespace Abilities.FirstEdition
{
    public class LightweightFrameAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += LightweightFrameActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= LightweightFrameActionEffect;
        }

        private void LightweightFrameActionEffect(GenericShip host)
        {
            GenericAction newAction = new LightweightFrameDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host,
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{

    public class LightweightFrameDiceModification : GenericAction
    {

        public LightweightFrameDiceModification()
        {
            Name = DiceModificationName = "Lightweight Frame";
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (Combat.DiceRollAttack.Count > Combat.DiceRollDefence.Count)
                {
                    result = true;
                }
            }

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            return 100; // I can't think of a reason we would ever NOT want to do this
        }

        public override void ActionEffect(System.Action callBack)
        {
            Messages.ShowInfo("Lightweight Frame - additional die rolled");
            DiceRoll.CurrentDiceRoll.RollInDice(callBack);
        }
    }

}