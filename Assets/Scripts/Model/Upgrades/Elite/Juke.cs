using Upgrade;
using Ship;
using Abilities;
using System;
using Tokens;
using RuleSets;

namespace UpgradesList
{

    public class Juke : GenericUpgrade, ISecondEditionUpgrade
    {

        public Juke() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Juke";
            Cost = 2;

            UpgradeAbilities.Add(new JukeAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 4;

            SEImageNumber = 8;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (RuleSet.Instance is SecondEdition)
                return ship.ShipBaseSize == BaseSize.Small || ship.ShipBaseSize == BaseSize.Medium;
            else
                return ship.ShipBaseSize == BaseSize.Small;
        }
    }
}

namespace Abilities
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