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
        public override void ActivateAbility()
        {
            ToggleAbility(true);
        }

        public override void DeactivateAbility()
        {
            ToggleAbility(false);
        }

        private void ToggleAbility(bool isActive)
        {
            HostShip.ArcInfo.OutOfArcShotPermissions.CanShootTorpedoes = isActive;
            foreach (Upgrade.GenericSecondaryWeapon torpedo in HostShip.UpgradeBar.GetInstalledUpgrades().Where(n => n.Type == Upgrade.UpgradeType.Torpedo))
            {
                torpedo.CanShootOutsideArc = isActive;
            }
        }
    }
}
