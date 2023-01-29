using ActionsList;
using Arcs;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class VenisaDoza : T70XWing
        {
            public VenisaDoza() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Venisa Doza",
                    "Jade Leader",
                    Faction.Resistance,
                    4,
                    5,
                    13,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.VenisaDozaAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/1329f27b-51aa-478c-aa83-8cf46bc2ad31/SWZ97_VenisaDozalegal+%281%29.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VenisaDozaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGameStart += UpdateArcRequirements;
            HostShip.OnUpdateWeaponRange += CheckRange;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGameStart -= UpdateArcRequirements;
            HostShip.OnUpdateWeaponRange -= CheckRange;
        }

        private void CheckRange(IShipWeapon weapon, ref int minRange, ref int maxRange, GenericShip target)
        {
            if (weapon is GenericSpecialWeapon)
            {
                var specialWeapon = weapon as GenericSpecialWeapon;
                if (specialWeapon.UpgradeInfo.HasType(UpgradeType.Missile)
                    || specialWeapon.UpgradeInfo.HasType(UpgradeType.Torpedo)
                    && Board.GetShipsInArcAtRange(HostShip, ArcType.Rear, new UnityEngine.Vector2(0, 4), Team.Type.Enemy).Contains(target))
                {
                    minRange = 1;
                    maxRange = 2;
                }
            }
        }

        private void UpdateArcRequirements()
        {
            foreach (GenericUpgrade weaponUpgrade in HostShip.UpgradeBar.GetSpecialWeaponsAll())
            {
                IShipWeapon specialWeapon = weaponUpgrade as IShipWeapon;
                if (specialWeapon.WeaponType == WeaponTypes.Torpedo || specialWeapon.WeaponType == WeaponTypes.Missile)
                {
                    if (specialWeapon.WeaponInfo.ArcRestrictions.Contains(ArcType.Front))
                    {
                        specialWeapon.WeaponInfo.ArcRestrictions.Add(ArcType.Rear);
                    }
                }
            }
        }
    }
}