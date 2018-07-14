using Ship;
using Upgrade;
using ActionsList;
using Abilities;

namespace UpgradesList
{
    public class LightweightFrame : GenericUpgrade
    {
        public LightweightFrame() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Lightweight Frame";
            Cost = 2;

            UpgradeAbilities.Add(new LightweightFrameAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship.Agility < 3 && ship is TIE);
        }
    }
}

namespace Abilities
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