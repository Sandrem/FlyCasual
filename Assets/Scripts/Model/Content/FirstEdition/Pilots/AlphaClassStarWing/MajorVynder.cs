using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AlphaClassStarWing
    {
        public class MajorVynder : AlphaClassStarWing
        {
            public MajorVynder() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Major Vynder",
                    7,
                    26,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.MajorVynderAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class MajorVynderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice += IncreaseDefenceDiceNumber;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice -= IncreaseDefenceDiceNumber;
        }

        private void IncreaseDefenceDiceNumber(ref int diceNumber)
        {
            if (HostShip.Tokens.HasToken(typeof(WeaponsDisabledToken))) diceNumber++;
        }
    }
}
