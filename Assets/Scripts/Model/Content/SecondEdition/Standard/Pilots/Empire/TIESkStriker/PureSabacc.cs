using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class PureSabacc : TIESkStriker
        {
            public PureSabacc() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Pure Sabacc\"",
                    "Confident Gambler",
                    Faction.Imperial,
                    4,
                    4,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.PureSabaccAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 119
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
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
            if (HostShip.Damage.DamageCards.Count <= 1)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + "'s ability grants +1 attack die");
                value++;
            }
        }
    }
}