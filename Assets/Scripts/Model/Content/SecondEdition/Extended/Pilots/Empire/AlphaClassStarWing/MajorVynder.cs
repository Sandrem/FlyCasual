using Content;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class MajorVynder : AlphaClassStarWing
        {
            public MajorVynder() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Major Vynder",
                    "Pragmatic Survivor",
                    Faction.Imperial,
                    4,
                    6,
                    21,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MajorVynderAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    seImageNumber: 135,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
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
            if (HostShip.Tokens.HasToken(typeof(WeaponsDisabledToken)))
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " has Disarm token and rolls 1 additional defense die");
                diceNumber++;
            }
        }
    }
}
