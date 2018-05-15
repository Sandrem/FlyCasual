using Ship;
using Ship.Firespray31;
using Upgrade;
using Mods.ModsList;
using System.Collections.Generic;
using Abilities;
using System;
using Arcs;

namespace UpgradesList
{
    public class KSEPursuitSpecial : GenericUpgradeSlotUpgrade
    {
        public bool IsAlwaysUse;

        public KSEPursuitSpecial() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "KSE Pursuit Special";
            Cost = -2;

            ImageUrl = "https://i.imgur.com/TmDkcUR.png";

            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Title) { MustBeDifferent = true }
            };

            FromMod = typeof(FiresprayFix);

            UpgradeAbilities.Add(new KSEPursuitSpecialAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Firespray31;
        }
    }
}

namespace Abilities
{
    public class KSEPursuitSpecialAbility : GenericAbility
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
            GenericArc rearArc = HostShip.ArcInfo.GetArc<ArcRear>();

            rearArc.ShotPermissions.CanShootCannon = isActive;
            rearArc.ShotPermissions.CanShootTorpedoes = isActive;
            rearArc.ShotPermissions.CanShootMissiles = isActive;
        }
    }
}