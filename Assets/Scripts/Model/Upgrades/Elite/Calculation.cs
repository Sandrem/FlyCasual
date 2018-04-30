using Upgrade;
using Ship;
using Abilities;
using Tokens;
using UnityEngine;

namespace UpgradesList
{
    public class Calculation : GenericUpgrade
    {
        public Calculation() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Calculation";
            Cost = 1;

            // AvatarOffset = new Vector2(39, 1);

            UpgradeAbilities.Add(new CalculationAbility());
        }
    }
}


namespace Abilities
{
    public class CalculationAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += CalculationEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= CalculationEffect;
        }

        private void CalculationEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.CalculationEffect()
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
    public class CalculationEffect : GenericAction
    {

        public CalculationEffect()
        {
            Name = EffectName = "Calculation";
        }

        public override int GetActionEffectPriority()
        {
            int focuses = Combat.DiceRollAttack.Focuses;
            int success = Combat.DiceRollAttack.Successes;

            //No single focus result, don't use it
            if (focuses != 1) return 0;

            //We want that crit comes into
            if ( success > Combat.Defender.Agility || Combat.Defender.Shields < success)
            {
                return 100;
            }
            //For a single focus result
            else
            {
                return 60;
            }
        }

        public override bool IsActionEffectAvailable()
        {

            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                /*You can activate it if you rolled one focus at least,
                 * and you dispose of focus tokens
                 */
                result = Host.Tokens.HasToken(typeof(FocusToken)) &&
                         Combat.DiceRollAttack.Focuses > 0;
            }
            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollAttack.ChangeOne(DieSide.Focus, DieSide.Crit, false);
            Host.Tokens.SpendToken(typeof(FocusToken), callBack);
        }

    }

}