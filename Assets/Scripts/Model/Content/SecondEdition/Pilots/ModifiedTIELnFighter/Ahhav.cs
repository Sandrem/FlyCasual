using BoardTools;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class Ahhav : ModifiedTIELnFighter
        {
            public Ahhav() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ahhav",
                    3,
                    30,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.AhhavAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         // seImageNumber: 92
                );

                // Ability

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
            HostShip.AfterGotNumberOfAttackDice += RegisterAhhavAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= RegisterAhhavAbility;
        }

        private void RegisterAhhavAbility(ref int result)
        {
            if (Combat.Defender.ShipInfo.BaseSize > Combat.Attacker.ShipInfo.BaseSize)
            {
                Messages.ShowInfo("Ahhav: +1 attack die");
                result++;
            }
        }
    }
}