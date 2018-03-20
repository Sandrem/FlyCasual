using Ship;
using Upgrade;
using System.Collections.Generic;
using System.Linq;

namespace UpgradesList
{
    public class HeavyScykInterceptor : GenericUpgradeSlotUpgrade
    {
        private readonly List<UpgradeType> SlotTypes = new List<UpgradeType>
        {
            UpgradeType.Cannon,
            UpgradeType.Torpedo,
            UpgradeType.Missile
        };
        
        public HeavyScykInterceptor() : base()
        { 
            Types.Add(UpgradeType.Title);
            Name = "\"Heavy Scyk\" Interceptor";
            Cost = 2;            

            AddedSlots = SlotTypes.Select(CreateSlot).ToList();
            
            UpgradeAbilities.Add(new Abilities.HullUpgradeAbility());            
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

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Ship.M3AScyk.M3AScyk;
        }
        
    }
}
