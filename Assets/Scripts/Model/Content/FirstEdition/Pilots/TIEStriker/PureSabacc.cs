using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEStriker
    {
        public class PureSabacc : TIEStriker
        {
            public PureSabacc() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Pure Sabacc\"",
                    6,
                    22,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.PureSabaccAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
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