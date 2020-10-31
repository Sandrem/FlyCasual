using Upgrade;
using System.Linq;
using System.Collections.Generic;
using System;
using Ship;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class R6D8 : GenericUpgrade
    {
        public R6D8() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R6-D8",
                UpgradeType.Astromech,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.R6D8Ability)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e5/c2/e5c25a09-ca2a-4742-92d5-d39d0d33d99b/swz68_r6d8.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R6D8Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "R6-D8",
                CanBeUsed,
                GetAiPriority,
                DiceModificationType.Reroll,
                GetRerollCount
            );
        }

        private bool CanBeUsed()
        {
            return (Combat.AttackStep == CombatStep.Attack && GetRerollCount() > 0);
        }

        private int GetAiPriority()
        {
            return 90;
        }

        private int GetRerollCount()
        {
            int count = 0;

            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, friendlyShip);
                if (distInfo.Range <= 3)
                {
                    if (friendlyShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Bullseye)) count++;
                }
            }

            return count;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}