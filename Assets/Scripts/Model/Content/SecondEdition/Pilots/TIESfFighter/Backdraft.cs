using Arcs;
using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class Backdraft : TIESfFighter
        {
            public Backdraft() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Backdraft\"",
                    4,
                    41,
                    isLimited: true,
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityType: typeof(Abilities.SecondEdition.BackdraftAbility) //,
                        //seImageNumber: 120
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/ac/Swz18_backdraft_a3.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BackdraftAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckBackdraftAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckBackdraftAbility;
        }

        private void CheckBackdraftAbility(ref int count)
        {
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon && Combat.ChosenWeapon.WeaponInfo.ArcRestrictions.Contains(ArcType.SingleTurret))
            {
                if (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Rear))
                {
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + " gains +1 attack die.");
                    count++;
                }
            }
        }
    }
}
