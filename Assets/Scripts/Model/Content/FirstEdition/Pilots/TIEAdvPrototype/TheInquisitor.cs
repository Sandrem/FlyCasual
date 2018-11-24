using BoardTools;
using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEAdvPrototype
    {
        public class TheInquisitor : TIEAdvPrototype
        {
            public TheInquisitor() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "The Inquisitor",
                    8,
                    25,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.TheInquisitorAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class TheInquisitorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            ShotInfo.OnRangeIsMeasured += SetRangeToOne;
        }

        public override void DeactivateAbility()
        {
            ShotInfo.OnRangeIsMeasured -= SetRangeToOne;
        }

        private void SetRangeToOne(GenericShip thisShip, GenericShip anotherShip, IShipWeapon chosenWeapon, ref int range)
        {
            if (thisShip.ShipId == HostShip.ShipId)
            {
                if ((range <= 3) && (chosenWeapon.GetType() == typeof(PrimaryWeaponClass)))
                {
                    range = 1;
                }
            }
        }
    }
}