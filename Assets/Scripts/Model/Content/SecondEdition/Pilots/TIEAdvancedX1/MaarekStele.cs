namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class MaarekStele : TIEAdvancedX1
        {
            public MaarekStele() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Maarek Stele",
                    5,
                    50,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.MaarekSteleAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 94;
            }
        }
    }
}
