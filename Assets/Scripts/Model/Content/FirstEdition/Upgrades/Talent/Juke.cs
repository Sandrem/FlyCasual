using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;
using Tokens;

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
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite -= JukeActionEffect;
        }

        private void JukeActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.JukeActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableDiceModification(newAction);
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
                Host.Tokens.HasToken(typeof(EvadeToken)))
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