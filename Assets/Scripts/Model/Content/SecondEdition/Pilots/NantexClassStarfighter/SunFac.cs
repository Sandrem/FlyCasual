using Upgrade;

namespace Ship
{
    namespace SecondEdition.NantexClassStarfighter
    {
        public class SunFac : NantexClassStarfighter
        {
            public SunFac() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sun Fac",
                    6,
                    52,
                    isLimited: true,
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityType: typeof(Abilities.SecondEdition.SunFacAbility)
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SunFacAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= CheckAbility;
        }

        private void CheckAbility(ref int count)
        {
            if (Combat.Defender.IsTractored)
            {
                Messages.ShowInfo("The defender is tractored. You roll an additional attack die.");
                count++;
            }
        }
    }
}