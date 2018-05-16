using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using ActionsList;

namespace UpgradesList
{
    public class MercenaryCopilot : GenericUpgrade
    {
        public MercenaryCopilot() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Mercenary Copilot";
            Cost = 2;

            AvatarOffset = new Vector2(46, 2);

            UpgradeAbilities.Add(new MercenaryCopilotAbility());
        }
    }
}

namespace Abilities
{
    public class MercenaryCopilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += MercenaryCopilotActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= MercenaryCopilotActionEffect;
        }

        private void MercenaryCopilotActionEffect(GenericShip host)
        {
            GenericAction newAction = new MercenaryCopilotAction()
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
    public class MercenaryCopilotAction : GenericAction
    {

        public MercenaryCopilotAction()
        {
            Name = EffectName = "Mercenary Copilot";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
            callBack();
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                BoardTools.ShipShotDistanceInformation shotInformation = new BoardTools.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
                if (shotInformation.Range == 3)
                {
                    result = true;
                }
            }
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Combat.DiceRollAttack.RegularSuccesses > 0) result = 20;
            }

            return result;
        }

    }
}