using BoardTools;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class MaulerMithel : TIELnFighter
        {
            public MaulerMithel() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Mauler\" Mithel",
                    "Black Two",
                    Faction.Imperial,
                    5,
                    3,
                    4,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MaulerMithelAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 80
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MaulerMithelAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += MaulerMithelPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= MaulerMithelPilotAbility;
        }

        private void MaulerMithelPilotAbility(ref int result)
        {
            ShotInfo shotInformation = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
            if (shotInformation.Range == 1)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is within range 1 of his target, he rolls +1 attack die");
                result++;
            }
        }
    }
}