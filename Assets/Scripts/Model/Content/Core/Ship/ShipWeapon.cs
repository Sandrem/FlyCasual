using Arcs;
using BoardTools;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    public interface IShipWeapon
    {
        GenericShip HostShip { get; set; }
        string Name { get; }

        SpecialWeaponInfo WeaponInfo { get; set; }

        bool IsShotAvailable(GenericShip targetShip);
        void PayAttackCost(Action callBack);
    }

    public class PrimaryWeaponClass : IShipWeapon
    {
        public GenericShip HostShip { get; set; }
        public string Name { get; set; }

        public SpecialWeaponInfo WeaponInfo { get; set; }

        // TODOREVERT
        // HostShip.CallAfterGotNumberOfPrimaryWeaponAttackDice(ref result);

        public PrimaryWeaponClass(GenericShip hostShip, ShipArcInfo arcInfo)
        {
            HostShip = hostShip;
            Name = "Primary Weapon (" + arcInfo.Name + ")";

            // Firepower is temporary
            WeaponInfo = new SpecialWeaponInfo(
                arcInfo.Firepower,
                1, 3,
                arc: arcInfo.ArcType
            );
        }

        public bool IsShotAvailable(GenericShip targetShip)
        {
            bool result = true;

            int minRange = WeaponInfo.MinRange;
            int maxRange = WeaponInfo.MaxRange;
            HostShip.CallUpdateWeaponRange(this, ref minRange, ref maxRange, targetShip);

            ShotInfo shotInfo = new ShotInfo(HostShip, targetShip, this);
            if (!shotInfo.IsShotAvailable) return false;

            if (shotInfo.Range < minRange) return false;
            if (shotInfo.Range > maxRange) return false;

            return result;
        }

        public void PayAttackCost(Action callBack)
        {
            callBack();
        }
    }
}
