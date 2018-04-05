using Ship;
using Upgrade;
using ActionsList;

namespace UpgradesList
{
    public class LightweightFrame : GenericUpgrade
    {
        public bool isUsed;

        public LightweightFrame() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Lightweight Frame";
            Cost = 2;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
			return (ship.Agility < 3 && ship is TIE);
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += LightweightFrameActionEffect;
            //Host.OnShipIsDestroyed += StopAbility; // Can probably be removed
        }

        private void LightweightFrameActionEffect(GenericShip host)
        {
			GenericAction newAction = new LightweightFrameDiceModification()
            {
                ImageUrl = ImageUrl,
                Host = host,
            };
            host.AddAvailableActionEffect(newAction);
        }
			
    }
}

namespace ActionsList
{

    public class LightweightFrameDiceModification : GenericAction
    {

        public LightweightFrameDiceModification()
        {
            Name = EffectName = "Lightweight Frame";
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (Combat.DiceRollAttack.Number > Combat.DiceRollDefence.Number)
                {
                    result = true;
                }
            }

            return result;
        }

        public override int GetActionEffectPriority()
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