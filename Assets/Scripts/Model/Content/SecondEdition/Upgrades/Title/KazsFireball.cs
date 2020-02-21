using Upgrade;
using BoardTools;
using System.Linq;
using ActionsList;
using System;

namespace UpgradesList.SecondEdition
{
    public class KazsFireball : GenericUpgrade
    {
        public KazsFireball() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Kaz's Fireball",
                UpgradeType.Title,
                cost: 2,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new ShipRestriction(typeof(Ship.SecondEdition.Fireball.Fireball)),
                    new FactionRestriction(Faction.Resistance)
                ),
                abilityType: typeof(Abilities.SecondEdition.KazsFireballAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/793a1b659936801101622a5fc0a71e73.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // Setup: When you resolve Explosion with Wings, you may search the damage deck
    // and choose a damage card with the Ship trait;
    // you are dealt that card instead. Then, shuffle the damage deck.

    // You can perform actions on damage cards even while ionized.

    // Important: First ability is coded directly in Fireball's ship ability

    public class KazsFireballAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCanPerformActionWhileIonized += CanPerformActionsFromDamageCardsWhileIonized;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCanPerformActionWhileIonized -= CanPerformActionsFromDamageCardsWhileIonized;
        }

        private void CanPerformActionsFromDamageCardsWhileIonized(GenericAction action, ref bool isAllowed)
        {
            if (action.IsCritCancelAction) isAllowed = true;
        }
    }
}