using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class Heroic : GenericUpgrade
    {
        public Heroic() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Heroic",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.HeroicAbility),
                restriction: new FactionRestriction(Faction.Resistance)//,
                                                                       //seImageNumber: 12
            );

            ImageUrl = "http://infinitearenas.com/xw2browse/images/upgrades/heroic.jpg";
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack, if the defender is in your bullseye firing arc, you may reroll 1 attack die.
    public class HeroicAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                20,
                new List<DieSide> { DieSide.Blank }
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            bool result = false;

            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    if (Combat.CurrentDiceRoll.Blanks == Combat.CurrentDiceRoll.Count) result = true;
                    break;
                case CombatStep.Defence:
                    if (Combat.CurrentDiceRoll.Blanks == Combat.CurrentDiceRoll.Count) result = true;
                    break;
                default:
                    break;
            }

            return result;
        }

        public int GetDiceModificationAiPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                if (Combat.CurrentDiceRoll.Blanks == Combat.CurrentDiceRoll.Count) result = 95;
            }

            return result;
        }
    }
}