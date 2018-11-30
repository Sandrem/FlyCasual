using Arcs;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.BWing
    {
        public class NeraDantels : BWing
        {
            public NeraDantels() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Nera Dantels",
                    5,
                    26,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.NeraDantelsAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Red";
            }
        }
    }
}

namespace Abilities.FirstEdition
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
            HostShip.ArcsInfo.GetArc<OutOfArc>().ShotPermissions.CanShootTorpedoes = isActive;
            foreach (GenericUpgrade torpedo in HostShip.UpgradeBar.GetInstalledUpgrades(UpgradeType.Torpedo))
            {
                GenericSpecialWeapon torpedoWeapon = torpedo as GenericSpecialWeapon;
                if (torpedoWeapon != null) torpedoWeapon.CanShootOutsideArc = isActive;
            }
        }
    }
}
