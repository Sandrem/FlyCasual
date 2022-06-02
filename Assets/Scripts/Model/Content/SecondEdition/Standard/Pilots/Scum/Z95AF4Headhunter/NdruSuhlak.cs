using Ship;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class NdruSuhlak : Z95AF4Headhunter
        {
            public NdruSuhlak() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "N'dru Suhlak",
                    "Hunt Saboteur",
                    Faction.Scum,
                    4,
                    3,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.NdruSuhlakAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Illicit
                    },
                    seImageNumber: 169,
                    skinName: "N'dru Suhlak"
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack, if there are no other friendly ships at range 0-2, roll 1 additional attack die.

    public class NdruSuhlakAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckNdruSuhlakAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckNdruSuhlakAbility;
        }

        private void CheckNdruSuhlakAbility(ref int value)
        {
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon)
                return;

            if (BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(0, 2), Team.Type.Friendly).Count == 1)
            {
                Messages.ShowInfo(HostName + ": +1 attack die");
                value++;
            }
        }
    }
}
