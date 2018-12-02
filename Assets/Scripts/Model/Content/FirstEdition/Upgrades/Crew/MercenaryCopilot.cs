using Ship;
using Upgrade;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ActionsList;
using BoardTools;

namespace UpgradesList.FirstEdition
{
    public class MercenaryCopilot : GenericUpgrade
    {
        public MercenaryCopilot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Mercenary Copilot",
                UpgradeType.Crew,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.MercenaryCopilotAbility)
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(46, 2));
        }        
    }
}

namespace Abilities.FirstEdition
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