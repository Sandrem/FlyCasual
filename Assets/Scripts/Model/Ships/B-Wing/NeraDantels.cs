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

                PilotAbilities.Add(new Abilities.NeraDantelsAbility());
            }
        }
    }
}

namespace Abilities
{
    public class NeraDantelsAbility : GenericAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            HostShip.ArcInfo.OutOfArcShotPermissions.CanShootTorpedoes = true;
            foreach (Upgrade.GenericSecondaryWeapon torpedo in HostShip.UpgradeBar.GetInstalledUpgrades().Where(n => n.Type == Upgrade.UpgradeType.Torpedo))
            {
                torpedo.CanShootOutsideArc = true;
            }
        }
    }
}
