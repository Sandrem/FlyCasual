using System;
using Upgrade;
using Ship;
using UnityEngine;
using Abilities;
using ActionsList;
using Tokens;

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

            UpgradeAbilities.Add(new EzraBridgerAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class EzraBridgerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += EzraBridgerActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= EzraBridgerActionEffect;
        }

        private void EzraBridgerActionEffect(GenericShip host)
        {
            GenericAction newAction = new EzraBridgerAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
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

        public override void ActionEffect(Action callBack)
        {
            if (Host.Tokens.HasToken(typeof(StressToken)))
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

            if (Combat.AttackStep == CombatStep.Attack && Host.Tokens.HasToken(typeof(StressToken)))
            {
                if (Combat.DiceRollAttack.RegularSuccesses > 0) result = 100;
            }

            return result;
        }

    }
}
