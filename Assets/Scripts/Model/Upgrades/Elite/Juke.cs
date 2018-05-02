using Upgrade;
using Ship;
using Abilities;
using System;
using Tokens;

namespace UpgradesList
{

    public class Juke : GenericUpgrade
    {

        public Juke() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Juke";
            Cost = 2;

            UpgradeAbilities.Add(new JukeAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
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
            HostShip.AfterGenerateAvailableOppositeActionEffectsList += JukeActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableOppositeActionEffectsList -= JukeActionEffect;
        }

        private void JukeActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.JukeActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
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
            DiceModificationTiming = DiceModificationTimingType.Opposite;
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