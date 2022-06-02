using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class B6BladeWingPrototype : GenericUpgrade
    {
        public B6BladeWingPrototype() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "B6 Blade Wing Prototype",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Gunner),
                restrictions: new UpgradeCardRestrictions
                (
                    new FactionRestriction(Faction.Rebel),
                    new ShipRestriction(typeof(Ship.SecondEdition.ASF01BWing.ASF01BWing))
                )
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/af/a8/afa8ba7b-d5dd-42ac-a992-618bd3f71dbb/swz83_upgrade_b6bladewingprototype1.png";
        }        
    }
}