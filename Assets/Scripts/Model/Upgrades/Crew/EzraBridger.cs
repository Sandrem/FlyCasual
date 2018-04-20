using System;
using Upgrade;
using Ship;
using UnityEngine;

namespace UpgradesList
{
    public class EzraBridger : GenericUpgrade
    {
        public EzraBridger() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Ezra Bridger";
            Cost = 3;

            AvatarOffset = new Vector2(7, 2);

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += EzraBridgerActionEffect;
        }

        private void EzraBridgerActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.EzraBridgerAction()
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
    public class EzraBridgerAction : GenericAction
    {

        public EzraBridgerAction()
        {
            Name = EffectName = "Ezra Bridger";
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (Host.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Crit);
            }
            else
            {
                Messages.ShowErrorToHuman("Cannot use: not stressed");
            }
            callBack();
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                result = true;
            }

            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack && Host.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                if (Combat.DiceRollAttack.RegularSuccesses > 0) result = 100;
            }

            return result;
        }

    }
}
