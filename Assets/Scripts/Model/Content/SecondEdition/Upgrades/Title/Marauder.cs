using Arcs;
using Ship;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Marauder : GenericUpgrade
    {
        public Marauder() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Marauder",
                UpgradeType.Title,
                cost: 6,
                isLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Gunner),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.FiresprayClassPatrolCraft.FiresprayClassPatrolCraft)),
                abilityType: typeof(Abilities.SecondEdition.MarauderAbility),
                seImageNumber: 150
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class MarauderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                1
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack
                && Combat.ShotInfo.InArcByType(ArcType.Rear)
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                result = true;
            }
            return result;
        }

        public int GetDiceModificationAiPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack && (Combat.ChosenWeapon as Upgrade.GenericSpecialWeapon) != null)
            {
                if (Combat.DiceRollAttack.Blanks > 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0)
                {
                    result = 30;
                }
            }

            return result;
        }
    }
}