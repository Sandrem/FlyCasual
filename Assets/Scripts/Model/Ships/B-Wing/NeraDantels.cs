﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Upgrade;
using Arcs;

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

                PrintedUpgradeIcons.Add(UpgradeType.Elite);

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
            HostShip.ArcInfo.GetArc<OutOfArc>().ShotPermissions.CanShootTorpedoes = isActive;
            foreach (GenericUpgrade torpedo in HostShip.UpgradeBar.GetInstalledUpgrades(UpgradeType.Torpedo))
            {
                GenericSecondaryWeapon torpedoWeapon = torpedo as GenericSecondaryWeapon;
                if (torpedoWeapon != null) torpedoWeapon.CanShootOutsideArc = isActive;
            }
        }
    }
}
