using Mods.ModsList;
using Ship;
using Ship.JumpMaster5000;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList
{
    public class PunishingOneSalvadgedAstromech : GenericUpgradeSlotUpgrade
    {
        public PunishingOneSalvadgedAstromech() : base()
        {
            Type = UpgradeType.Title;
            Name = "Punishing One (S.Astromech)";
            Cost = 12;

            ImageUrl = "https://i.imgur.com/sCtAQbe.png";

            isUnique = true;

            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.SalvagedAstromech)
            };

            FromMod = typeof(PunishingOneSalvadgedAstromechMod);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is JumpMaster5000;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            host.ChangeFirepowerBy(1);
        }
    }
}
