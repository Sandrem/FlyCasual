using Arcs;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class Backdraft : TIESfFighter
        {
            public Backdraft() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Backdraft\"",
                    "Fiery Fanatic",
                    Faction.FirstOrder,
                    4,
                    4,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BackdraftAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
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
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + " gains +1 attack die");
                    count++;
                }
            }
        }
    }
}
