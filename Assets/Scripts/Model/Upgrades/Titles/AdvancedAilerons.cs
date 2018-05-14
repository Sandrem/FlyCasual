using Ship;
using Ship.TIEReaper;
using Upgrade;
using System.Collections.Generic;
using System;
using UpgradesList;
using SubPhases;
using ActionsList;
using Abilities;
using Movement;
using GameModes;
using Tokens;

namespace UpgradesList
{
    public class AdvancedAilerons : GenericUpgrade
    {
        public AdvancedAilerons() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Advanced Ailerons";
            Cost = 0;

            UpgradeAbilities.Add(new AdvancedAileronsAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEReaper;
        }
    }
}

namespace Abilities
{
    public class AdvancedAileronsAbility : AdaptiveAileronsAbility
    {

        public override void ActivateAbility()
        {
            base.ActivateAbility();

            HostShip.Maneuvers["3.L.B"] = MovementComplexity.Normal;
            HostShip.Maneuvers["3.R.B"] = MovementComplexity.Normal;
        }

        public override void DeactivateAbility()
        {
            base.ActivateAbility();

            HostShip.Maneuvers["3.L.B"] = MovementComplexity.Complex;
            HostShip.Maneuvers["3.R.B"] = MovementComplexity.Complex;
        }
    }
}