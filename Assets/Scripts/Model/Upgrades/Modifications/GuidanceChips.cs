using Ship;
using Upgrade;

namespace UpgradesList
{
    public class GuidanceChips : GenericUpgrade
    {
        public bool isUsed;

        public GuidanceChips() : base()
        {
            Type = UpgradeType.Modification;
            Name = "Guidance Chips";
            Cost = 0;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += LoneWolfActionEffect;
            Phases.OnRoundEnd += ClearUsed;
        }

        private void LoneWolfActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.GuidanceChipsEffect()
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

    }
}

namespace ActionsList
{

    public class GuidanceChipsEffect : GenericAction
    {

        public GuidanceChipsEffect()
        {
            Name = EffectName = "Guidance Chips";
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack && !(Source as UpgradesList.GuidanceChips).isUsed)
            {
                GenericSecondaryWeapon secondaryWeapon = (Combat.ChosenWeapon as GenericSecondaryWeapon);
                if (secondaryWeapon.Type == UpgradeType.Torpedo || secondaryWeapon.Type == UpgradeType.Missile)
                {
                    result = true;
                }
            }

            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.CurentDiceRoll.Blanks == 1) result = 100;
            else if (Combat.CurentDiceRoll.Blanks > 1) result = 55;
            else if (Combat.CurentDiceRoll.Focuses == 1) result = 55;
            else result = 30;

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DieSide newResult = (Host.Firepower >= 3) ? DieSide.Crit : DieSide.Success;

            DieSide oldResult = DieSide.Crit;
            if (Combat.CurentDiceRoll.Blanks > 0) oldResult = DieSide.Blank;
            else if (Combat.CurentDiceRoll.Focuses > 0) oldResult = DieSide.Focus;
            else if (Combat.CurentDiceRoll.RegularSuccesses > 0) oldResult = DieSide.Success;

            Combat.CurentDiceRoll.ChangeOne(oldResult, newResult);

            (Source as UpgradesList.GuidanceChips).isUsed = true;
        }

    }

}
