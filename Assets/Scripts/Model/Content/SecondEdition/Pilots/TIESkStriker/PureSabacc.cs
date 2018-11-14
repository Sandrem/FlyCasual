using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class PureSabacc : TIESkStriker
        {
            public PureSabacc() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Pure Sabacc\"",
                    4,
                    44,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.PureSabaccAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 119;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class PureSabaccAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckPureSabaccAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckPureSabaccAbility;
        }

        private void CheckPureSabaccAbility(ref int value)
        {
            if (HostShip.Damage.DamageCards.Count <= 1) value++;
        }
    }
}
