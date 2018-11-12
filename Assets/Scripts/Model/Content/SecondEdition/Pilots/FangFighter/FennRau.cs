namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class FennRau : FangFighter
        {
            public FennRau() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Fenn Rau",
                    6,
                    68,
                    limited: 1,
                    abilityText: "When you defend or perform an attack, if the attack range is 1, you may roll 1 additional die.",
                    abilityType: typeof(Abilities.FirstEdition.FennRauScumAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 155;
            }
        }
    }
}