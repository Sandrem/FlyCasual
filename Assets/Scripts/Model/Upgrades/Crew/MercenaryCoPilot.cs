using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using ActionsList;
using BoardTools;

namespace UpgradesList
{
    public class MercenaryCopilot : GenericUpgrade
    {
        public MercenaryCopilot() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Mercenary Copilot";
            Cost = 2;

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(46, 2));

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
            HostShip.OnGenerateDiceModifications += MercenaryCopilotActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= MercenaryCopilotActionEffect;
        }

        private void MercenaryCopilotActionEffect(GenericShip host)
        {
            GenericAction newAction = new MercenaryCopilotAction()
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
    public class MercenaryCopilotAction : GenericAction
    {

        public MercenaryCopilotAction()
        {
            Name = DiceModificationName = "Mercenary Copilot";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
            callBack();
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                ShotInfo shotInformation = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
                if (shotInformation.Range == 3)
                {
                    result = true;
                }
            }
            return result;
        }

        public override int GetDiceModificationPriority()
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