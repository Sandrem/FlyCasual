namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class SkakoanAce : Belbullab22Starfighter
    {
        public SkakoanAce()
        {
            PilotInfo = new PilotCardInfo(
                "Skakoan Ace",
                3,
                38,
                extraUpgradeIcon: Upgrade.UpgradeType.Talent
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/ceb7a3bc406ff17be5dee5de62b39195.png";
        }
    }
}