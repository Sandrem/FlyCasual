using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using Ship;


namespace UpgradesList
{

    public class Juke : GenericUpgrade
    {

        public Juke() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Juke";
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);
            host.AfterGenerateAvailableOppositeActionEffectsList += JukeActionEffect;
            Console.Write("Juke attached");
        }

        private void JukeActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.JukeActionEffect()
            {
                ImageUrl = ImageUrl,
                Host = host
            };
            host.AddAvailableOppositeActionEffect(newAction);
        }
    }
}

namespace ActionsList
{
    public class JukeActionEffect : GenericAction
    {

        public JukeActionEffect()
        {
            Name = EffectName = "Juke";
            IsOpposite = true;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            result = 100;

            return result;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && 
                Combat.DiceRollDefence.RegularSuccesses > 0 && 
                Host.HasToken(typeof(Tokens.EvadeToken)))
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollDefence.ChangeOne(DieSide.Success, DieSide.Focus, false);
            callBack();
        }

    }

}