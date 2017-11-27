using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ship
{
    namespace BWing
    {
        public class NeraDantels : BWing
        {
            public NeraDantels() : base()
            {
                PilotName = "Nera Dantels";
                PilotSkill = 5;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Red";

                PilotAbilities.Add(new PilotAbilitiesNamespace.NeraDantelsAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class NeraDantelsAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Host.ArcInfo.OutOfArcShotPermissions.CanShootTorpedoes = true;
            foreach (Upgrade.GenericSecondaryWeapon torpedo in Host.UpgradeBar.GetInstalledUpgrades().Where(n => n.Type == Upgrade.UpgradeType.Torpedo))
            {
                torpedo.CanShootOutsideArc = true;
            }
        }
    }
}
