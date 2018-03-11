using Ship;
using Upgrade;

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
            return (ship.Agility < 3);
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += LightweightFrameActionEffect;
            Host.OnShipIsDestroyed += StopAbility; // Can probably be removed
        }

        private void LightweightFrameActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.LightweightFrameEffect()
            {
                ImageUrl = ImageUrl,
                Host = host,
                Source = this
            };
            host.AddAvailableActionEffect(newAction);
        }

        private void ClearUsed()
        {
            isUsed = false;
        }

        private void StopAbility(GenericShip host, bool isFled)
        {
            // I don't think we need anything here, do we?
        }

    }
}

namespace ActionsList
{

    public class LightweightFrameEffect : GenericAction
    {

        public LightweightFrameEffect()
        {
            Name = EffectName = "LightweightFrame";
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
            Combat.DiceRollDefence.AddDice();
            Combat.DiceRollDefense.OrganizeDicePositions();

            callBack();
        }

    }

}