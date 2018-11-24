using System.Collections.Generic;
using Ship;

namespace Upgrade
{
    public abstract class GenericUpgradeSlotUpgrade : GenericUpgrade
    {
        public List<UpgradeSlot> AddedSlots = new List<UpgradeSlot>();
        public List<UpgradeType> ForbiddenSlots = new List<UpgradeType>();
        public Dictionary<UpgradeType, int> CostReductionByType = new Dictionary<UpgradeType, int>();

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            AddedSlots.ForEach(slot => {
                slot.GrantedBy = this;
                host.UpgradeBar.AddSlot(slot);
            });

            ForbiddenSlots.ForEach(type =>
            {
                host.UpgradeBar.ForbidSlots(type);
            });

            foreach (var item in CostReductionByType)
            {
                host.UpgradeBar.CostReduceByType(item.Key, item.Value);
            }
        }

        public override void PreDettachFromShip()
        {
            base.PreDettachFromShip();

            AddedSlots.ForEach(slot => Host.UpgradeBar.RemoveSlot(slot.Type, this));

            ForbiddenSlots.ForEach(type => Host.UpgradeBar.AllowSlots(type));

            foreach (var item in CostReductionByType)
            {
                Host.UpgradeBar.CostReduceByType(item.Key, -item.Value);
            }
        }
    }
}
