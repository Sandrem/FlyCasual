using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class Expertise : GenericUpgrade
    {

        public Expertise() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Expertise";
            Cost = 4;

            // AvatarOffset = new Vector2(10, 5);
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += AddExpertiseDiceModification;
        }

        private void AddExpertiseDiceModification(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.ExpertiseDiceModification();
            newAction.ImageUrl = ImageUrl;
            newAction.Host = Host;
            host.AddAvailableActionEffect(newAction);
        }

    }
}

namespace ActionsList
{

    public class ExpertiseDiceModification : GenericAction
    {

        public ExpertiseDiceModification()
        {
            Name = EffectName = "Expertise";

            IsTurnsAllFocusIntoSuccess = true;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (!Host.Tokens.HasToken(typeof(Tokens.StressToken)))
                {
                    int attackFocuses = Combat.DiceRollAttack.Focuses;
                    if (attackFocuses > 0) result = 55;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (!Host.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus, DieSide.Success);
            }
            else
            {
                Messages.ShowErrorToHuman("Cannot use Expertise while stressed");
            }
            callBack();
        }

    }

}
