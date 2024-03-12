﻿using ActionsList;
using Ship;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Juke : GenericUpgrade
    {
        public Juke() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Juke",
                UpgradeType.Talent,
                cost: 7,
                abilityType: typeof(Abilities.SecondEdition.JukeAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small, BaseSize.Medium),
                seImageNumber: 8
            );
        }
    }
}

namespace Abilities.SecondEdition
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