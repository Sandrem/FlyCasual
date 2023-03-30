using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class Fanatic : GenericUpgrade
    {
        public Fanatic() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Fanatic",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.FanaticAbility)
            );

            ImageUrl = "https://i.imgur.com/3AzqrIt.jpg";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FanaticAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Focus },
                DieSide.Success
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (HostShip.Damage.IsDamaged
                && Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon);
        }

        public int GetDiceModificationAiPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                if (attackFocuses > 0) result = 100;
            }

            return result;
        }
    }
}