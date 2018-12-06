using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ChardaanRefit : GenericSpecialWeapon
    {
        public ChardaanRefit() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Chardaan Refit",
                UpgradeType.Missile,
                cost: -2,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.AWing.AWing))
            );
        }        
    }
}