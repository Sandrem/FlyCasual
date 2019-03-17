using BoardTools;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class Longshot : TIEFoFighter
        {
            public Longshot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Longshot\"",
                    3,
                    33,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LongshotAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 120
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/a6/a3/a6a34b16-16d8-43f7-b250-0e8dd9299a5f/swz26_a1_longshot.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LongshotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += LongshotPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= LongshotPilotAbility;
        }

        private void LongshotPilotAbility(ref int result)
        {
            ShotInfo shotInformation = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
            if (shotInformation.Range == 3)
            {
                Messages.ShowInfo("\"Longshot\" is attacking at range 3 and gains +1 attack die.");
                result++;
            }
        }
    }
}