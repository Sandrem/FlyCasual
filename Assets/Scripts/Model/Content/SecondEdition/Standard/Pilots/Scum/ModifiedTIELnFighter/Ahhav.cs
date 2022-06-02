using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class Ahhav : ModifiedTIELnFighter
        {
            public Ahhav() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Ahhav",
                    "Vengeful Survivor",
                    Faction.Scum,
                    3,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.AhhavAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/d5/af/d5af765f-4c49-4209-98a8-e76f52bf9608/swz23_ahhav.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AhhavAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += RegisterAhhavAttackAbility;
            HostShip.AfterGotNumberOfDefenceDice += RegisterAhhavDefenceAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= RegisterAhhavAttackAbility;
            HostShip.AfterGotNumberOfDefenceDice -= RegisterAhhavDefenceAbility;
        }

        private void RegisterAhhavAttackAbility(ref int result)
        {
            if (Combat.Attacker == HostShip && Combat.Defender.ShipInfo.BaseSize > Combat.Attacker.ShipInfo.BaseSize)
            {
                Messages.ShowInfo("Ahhav is attacking a larger ship and gains +1 attack die");
                result++;
            }
        }

        private void RegisterAhhavDefenceAbility(ref int result)
        {
            if (Combat.Defender == HostShip && Combat.Attacker.ShipInfo.BaseSize > Combat.Defender.ShipInfo.BaseSize)
            {
                Messages.ShowInfo("Ahhav is defending against a larger ship and gains +1 defence die");
                result++;
            }
        }
    }
}