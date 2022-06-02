using System.Collections.Generic;
using Abilities.SecondEdition;
using Upgrade;
using System.Linq;
using Content;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class GideonHask : TIELnFighter
        {
            public GideonHask() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Gideon Hask",
                    "Inferno Two",
                    Faction.Imperial,
                    4,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(GideonHaskTieLnAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 84,
                    skinName: "Inferno"
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GideonHaskTieLnAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
        }

        protected virtual void CheckConditions()
        {
            if (Combat.Defender.Damage.DamageCards.Any())
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            }
        }

        protected virtual void SendExtraDiceMessage()
        {
            Messages.ShowInfo("The defender has a damage card, " + HostShip.PilotInfo.PilotName + " rolls an additional attack die");
        }

        protected void RollExtraDice(ref int count)
        {
            count++;
            SendExtraDiceMessage();
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }
    }
}

