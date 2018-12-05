using Ship;
using Upgrade;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class Calculation : GenericUpgrade
    {
        public Calculation() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Calculation",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.CalculationAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class CalculationAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += CalculationEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= CalculationEffect;
        }

        private void CalculationEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.CalculationEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = host
            };
            host.AddAvailableDiceModification(newAction);

        }
    }
}

namespace ActionsList
{
    public class CalculationEffect : GenericAction
    {

        public CalculationEffect()
        {
            Name = DiceModificationName = "Calculation";
        }

        public override int GetDiceModificationPriority()
        {
            int focuses = Combat.DiceRollAttack.Focuses;
            int success = Combat.DiceRollAttack.Successes;

            //No single focus result, don't use it
            if (focuses != 1) return 0;

            //We want that crit comes into
            if (success > Combat.Defender.State.Agility || Combat.Defender.State.ShieldsCurrent < success)
            {
                return 100;
            }
            //For a single focus result
            else
            {
                return 60;
            }
        }

        public override bool IsDiceModificationAvailable()
        {

            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                /*You can activate it if you rolled one focus at least,
                 * and you dispose of focus tokens
                 */
                result = HostShip.Tokens.HasToken(typeof(FocusToken)) &&
                         Combat.DiceRollAttack.Focuses > 0;
            }
            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollAttack.ChangeOne(DieSide.Focus, DieSide.Crit, false);
            HostShip.Tokens.SpendToken(typeof(FocusToken), callBack);
        }

    }

}