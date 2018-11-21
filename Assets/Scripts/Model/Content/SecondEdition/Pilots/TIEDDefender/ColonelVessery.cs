namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class ColonelVessery : TIEDDefender
        {
            public ColonelVessery() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Colonel Vessery",
                    4,
                    88,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.ColonelVesseryAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 123;
            }
        }
    }
}