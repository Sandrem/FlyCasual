using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class MajorRhymer : TIESaBomber
        {
            public MajorRhymer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Major Rhymer",
                    "Scimitar Leader",
                    Faction.Imperial,
                    4,
                    5,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MajorRhymerAbility),
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    seImageNumber: 109,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MajorRhymerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnUpdateWeaponRange += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnUpdateWeaponRange -= CheckAbility;
        }

        private void CheckAbility(IShipWeapon weapon, ref int minRange, ref int maxRange, GenericShip target)
        {
            if (weapon is GenericSpecialWeapon)
            {
                var specialWeapon = weapon as GenericSpecialWeapon;
                if (specialWeapon.UpgradeInfo.HasType(UpgradeType.Missile) || specialWeapon.UpgradeInfo.HasType(UpgradeType.Torpedo))
                {
                    if (minRange > 0) minRange--;
                    if (maxRange < 3) maxRange++;
                }
            }
        }
    }
}