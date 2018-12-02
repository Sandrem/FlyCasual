using Ship;
using Upgrade;
using System.Collections.Generic;
using Mods.ModsList;

namespace UpgradesList.FirstEdition
{
    public class PunishingOneSalvadgedAstromech : GenericUpgrade
    {
        public PunishingOneSalvadgedAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Punishing One (S.Astromech)",
                UpgradeType.Title,
                cost: 12,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.JumpMaster5000.JumpMaster5000)),
                addSlot: new UpgradeSlot(UpgradeType.SalvagedAstromech),
                abilityType: typeof(Abilities.FirstEdition.PunishingOneAbility)
            );

            FromMod = typeof(PunishingOneSalvadgedAstromechMod);
            ImageUrl = "https://i.imgur.com/9unI9tE.png";
        }        
    }
}