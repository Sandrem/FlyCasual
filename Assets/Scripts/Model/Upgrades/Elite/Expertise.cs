using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using ActionsList;

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

            UpgradeAbilities.Add(new ExpertiseAbility());
        }
    }
}

namespace Abilities
{
    public class ExpertiseAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += AddExpertiseDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddExpertiseDiceModification;
        }

        private void AddExpertiseDiceModification(GenericShip host)
        {
            GenericAction newAction = new ExpertiseDiceModification
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
