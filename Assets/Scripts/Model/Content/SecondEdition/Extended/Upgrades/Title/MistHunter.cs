using Actions;
using ActionsList;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class MistHunter : GenericUpgrade
    {
        public MistHunter() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Mist Hunter",
                UpgradeType.Title,
                cost: 0,       
                isLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Cannon),
                addAction: new ActionInfo(typeof(BarrelRollAction)),
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Scum),
                    new ShipRestriction(typeof(Ship.SecondEdition.G1AStarfighter.G1AStarfighter))
                ),
                seImageNumber: 151
            );
        }
    }
}