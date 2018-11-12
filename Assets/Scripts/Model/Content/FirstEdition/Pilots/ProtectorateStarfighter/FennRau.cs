namespace Ship
{
    namespace FirstEdition.ProtectorateStarfighter
    {
        public class FennRau : ProtectorateStarfighter
        {
            public FennRau() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Fenn Rau",
                    9,
                    28,
                    limited: 1,
                    abilityText: "When attacking or defending, if the enemy ship is at Range 1, you may roll 1 additional die.",
                    abilityType: typeof(Abilities.FirstEdition.FennRauScumAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class FennRauScumAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckFennRauAbility;
            HostShip.AfterGotNumberOfDefenceDice += CheckFennRauAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckFennRauAbility;
            HostShip.AfterGotNumberOfDefenceDice -= CheckFennRauAbility;
        }

        private void CheckFennRauAbility(ref int value)
        {
            if (Combat.ShotInfo.Range == 1)
            {
                Messages.ShowInfo("Fenn Rau: +1 die");
                value++;
            }
        }
    }
}
