using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YT1300
    {
        public class Chewbacca : YT1300
        {
            public Chewbacca() : base()
            {
                PilotName = "Chewbacca";
                PilotSkill = 5;
                Cost = 42;

                IsUnique = true;

                Firepower = 3;
                MaxHull = 8;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new PilotAbilitiesNamespace.ChewbaccaAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class ChewbaccaAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Host.OnCheckFaceupCrit += FlipCrits;
        }

        private void FlipCrits(ref bool result)
        {
            if (result == true)
            {
                Messages.ShowInfo("Chewbacca: Crit is flipped facedown");
                Sounds.PlayShipSound("Chewbacca");
                result = false;
            }
        }
    }
}
