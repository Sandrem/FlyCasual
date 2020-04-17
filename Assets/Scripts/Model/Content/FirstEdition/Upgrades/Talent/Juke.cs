using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;
using Tokens;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class Juke : GenericUpgrade
    {
        public Juke() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Juke",
                UpgradeType.Talent,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.JukeAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class JukeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite += JukeActionEffect;
            HostShip.Ai.OnGetActionPriority += IncreaceEvadePriority;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite -= JukeActionEffect;
            HostShip.Ai.OnGetActionPriority -= IncreaceEvadePriority;
        }

        private void JukeActionEffect(GenericShip host)
        {
            GenericAction newAction = new JukeActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = host
            };
            host.AddAvailableDiceModificationOwn(newAction);
        }

        private void IncreaceEvadePriority(GenericAction action, ref int priority)
        {
            if (action is EvadeAction) priority = 80;
        }
    }
}

namespace ActionsList
{
    public class JukeActionEffect : GenericAction
    {

        public JukeActionEffect()
        {
            Name = DiceModificationName = "Juke";
            DiceModificationTiming = DiceModificationTimingType.Opposite;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            result = 100;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence &&
                Combat.DiceRollDefence.RegularSuccesses > 0 &&
                HostShip.Tokens.HasToken(typeof(EvadeToken)))
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