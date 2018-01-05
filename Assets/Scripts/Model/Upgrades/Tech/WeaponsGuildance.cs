using Upgrade;
using Ship;
using ActionsList;

namespace UpgradesList
{
    public class WeaponsGuildance : GenericUpgrade
    {
        public WeaponsGuildance() : base()
        {
            Type = UpgradeType.Tech;
            Name = "Weapons Guidance";
            Cost = 2;            
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += WeaponsGuidanceActionEffect;
        }

        private void WeaponsGuidanceActionEffect(GenericShip host)
        {
            GenericAction newAction = new WeaponsGuildanceDiceModification()
            {
                ImageUrl = ImageUrl,
                Host = host
            };
            host.AddAvailableActionEffect(newAction);
        }

    }
}

namespace ActionsList
{
    public class WeaponsGuildanceDiceModification : GenericAction
    {

        public WeaponsGuildanceDiceModification()
        {
            Name = EffectName = "Weapons Guidance";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Host.RemoveToken(typeof(Tokens.FocusToken));
            Combat.CurrentDiceRoll.Change(DieSide.Blank, DieSide.Crit, 1);
            Combat.CurrentDiceRoll.OrganizeDicePositions();

            callBack();
        }

        public override bool IsActionEffectAvailable()
        {
            if (Combat.AttackStep == CombatStep.Attack 
                && Host.HasToken(typeof(Tokens.FocusToken))
                && Combat.CurrentDiceRoll.Blanks > 0)
            {
                return true;
            }

            return false;
        }

        public override int GetActionEffectPriority()
        {
            int result = 110;

            return result;
        }

    }
}