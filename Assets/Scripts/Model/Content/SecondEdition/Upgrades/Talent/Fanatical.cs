using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class Fanatical : GenericUpgrade
    {
        public Fanatical() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Fanatical",
                UpgradeType.Talent,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.FanaticalAbility),
                restriction: new FactionRestriction(Faction.FirstOrder)//,
                //seImageNumber: 12
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6d/44/6d440bea-11dd-4e4c-b7ef-167a4b6d23e2/swz18_a1_fanatical.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack, if the defender is in your bullseye firing arc, you may reroll 1 attack die.
    public class FanaticalAbility : GenericAbility
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
            return (HostShip.State.ShieldsCurrent == 0
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