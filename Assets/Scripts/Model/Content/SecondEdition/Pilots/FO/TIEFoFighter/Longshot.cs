using BoardTools;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class Longshot : TIEFoFighter
        {
            public Longshot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Longshot\"",
                    "Zeta Ace",
                    Faction.FirstOrder,
                    3,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LongshotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Tech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
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
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is attacking at range 3 and gains +1 attack die");
                result++;
            }
        }
    }
}