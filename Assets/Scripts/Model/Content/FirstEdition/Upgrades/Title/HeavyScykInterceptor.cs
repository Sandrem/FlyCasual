using Ship;
using Upgrade;
using System.Collections.Generic;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class HeavyScykInterceptor : GenericUpgrade
    {
        private readonly List<UpgradeType> SlotTypes = new List<UpgradeType>
        {
            UpgradeType.Cannon,
            UpgradeType.Torpedo,
            UpgradeType.Missile
        };

        public HeavyScykInterceptor() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Heavy Scyk\" Interceptor",
                UpgradeType.Title,
                cost: 2,
                addSlots: SlotTypes.Select(CreateSlot).ToList(),
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.M3AInterceptor.M3AInterceptor)),
                abilityType: typeof(Abilities.FirstEdition.LightScykInterceptorAbility)
            );

            // TODO: +1 Hull
        }

        private UpgradeSlot CreateSlot(UpgradeType slotType)
        {
            var slot = new UpgradeSlot(slotType);
            slot.OnPreInstallUpgrade += delegate { UpgradeInstalled(slotType); };
            slot.OnRemovePreInstallUpgrade += delegate { UpgradeRemoved(slotType); };
            slot.GrantedBy = this;
            return slot;
        }

        private void UpgradeInstalled(UpgradeType slotType)
        {
            SlotTypes
                .Where(slot => slot != slotType)
                .ToList()
                .ForEach(slot => Host.UpgradeBar.RemoveSlot(slot, this));
        }

        private void UpgradeRemoved(UpgradeType slotType)
        {
            SlotTypes
                .Where(slot => slot != slotType)
                .ToList()
                .ForEach(slot => Host.UpgradeBar.AddSlot(CreateSlot(slot)));
        }
    }
}