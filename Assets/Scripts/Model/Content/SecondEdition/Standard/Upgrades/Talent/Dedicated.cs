using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;
using UpgradesList.SecondEdition;

namespace UpgradesList.SecondEdition
{
    public class Dedicated : GenericUpgrade
    {
        public Dedicated() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Dedicated",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.DedicatedAbility),
                restrictions: new UpgradeCardRestrictions
                (
                    new FactionRestriction(Faction.Republic),
                    new TagRestriction(Content.Tags.Clone)
                )
            );

            ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/dd/Swz32_dedicated.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    //While another friendly ship in your left or right arc at range 0-2 defends, if it is limited or has the Dedicated upgrade and you are not strained, you may gain 1 strain token.
    //If you do, the defender rerolls 1 of their blank results.
    public class DedicatedAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName + " (ID: " + HostShip.ShipId + ")",
                IsAvailable,
                AiPriority,
                DiceModificationType.Reroll,
                1,
                sidesCanBeSelected: new List<DieSide> { DieSide.Blank },
                isGlobal: true,
                payAbilityCost: PayAbilityCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        protected virtual bool IsAvailable()
        {
            var rangeLeft = HostShip.SectorsInfo.RangeToShipBySector(Combat.Defender, Arcs.ArcType.Left);
            var rangeRight = HostShip.SectorsInfo.RangeToShipBySector(Combat.Defender, Arcs.ArcType.Right);

            return
                Combat.AttackStep == CombatStep.Defence
                && Combat.Defender.Owner == HostShip.Owner
                && ((HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Left) && rangeLeft >= 0 && rangeLeft <= 2)
                    || (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Right) && rangeRight >= 0 && rangeRight <= 2))
                && (Combat.Defender.PilotInfo.IsLimited || Combat.Defender.UpgradeBar.GetInstalledUpgrades(UpgradeType.Talent).Any(u => u is Dedicated))
                && !HostShip.IsStrained
                && Combat.DiceRollDefence.DiceList.Any(d => d.Side is DieSide.Blank);
        }

        private int AiPriority()
        {
            int hitsIncoming = Combat.DiceRollAttack.Successes - Combat.DiceRollDefence.Successes;
            int hitsLeft = Combat.Defender.State.HullCurrent + Combat.Defender.State.ShieldsCurrent;

            if (hitsIncoming > 0 && hitsIncoming <= hitsLeft && Combat.DiceRollDefence.Blanks > 0)
            {
                return 87;
            }
            else
            {
                return 0;
            }
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), () => callback(true));
        }
    }
}