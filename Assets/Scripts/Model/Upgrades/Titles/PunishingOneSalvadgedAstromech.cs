using Mods.ModsList;
using Ship;
using Ship.JumpMaster5000;
using System.Collections.Generic;
using Upgrade;
using Abilities;

namespace UpgradesList
{
    public class PunishingOneSalvadgedAstromech : GenericUpgradeSlotUpgrade
    {
        public PunishingOneSalvadgedAstromech() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Punishing One (S.Astromech)";
            Cost = 12;

            ImageUrl = "https://i.imgur.com/9unI9tE.png";

            isUnique = true;

            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.SalvagedAstromech)
            };

            FromMod = typeof(PunishingOneSalvadgedAstromechMod);

            UpgradeAbilities.Add(new PunishingOneSalvadgedAstromechAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is JumpMaster5000;
        }
    }
}

namespace Abilities
{
    public class PunishingOneSalvadgedAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.ChangeFirepowerBy(1);
        }

        public override void DeactivateAbility()
        {
            HostShip.ChangeFirepowerBy(-1);
        }
    }
}
