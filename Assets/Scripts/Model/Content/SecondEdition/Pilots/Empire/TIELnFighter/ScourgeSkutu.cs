using Abilities.SecondEdition;
using Arcs;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class ScourgeSkutu : TIELnFighter
        {
            public ScourgeSkutu()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Scourge\" Skutu",
                    "Seasoned Veteran",
                    Faction.Imperial,
                    5,
                    3,
                    3,
                    isLimited: true,
                    abilityType: typeof(ScourgeSkutuAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 82
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ScourgeSkutuAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
        }

        private void CheckConditions()
        {
            if (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye))
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            }
        }

        private void SendExtraDiceMessage()
        {
            Messages.ShowInfo("The defender is in your bullseye arc, " + HostShip.PilotInfo.PilotName + " rolls an additional attack die");
        }

        private void RollExtraDice(ref int count)
        {
            count++;
            SendExtraDiceMessage();
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }
    }
}