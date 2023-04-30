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
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.HeroicAbility),
                restriction: new FactionRestriction(Faction.Resistance)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you defend or perform an attack, if you have only blank results and have two or more results, you may reroll any number of your dice.
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
            return (Combat.AttackStep == CombatStep.Defence || Combat.AttackStep == CombatStep.Attack)
                && (Combat.CurrentDiceRoll.Count == Combat.CurrentDiceRoll.Blanks)
                && (Combat.CurrentDiceRoll.Count >= 2);
        }

        public int GetDiceModificationAiPriority()
        {
            return 95;
        }
    }
}