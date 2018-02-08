using Abilities;
using Ship;
using SubPhases;
using System;
using Upgrade;

namespace UpgradesList
{
    public class KyloRenCrew : GenericUpgrade
    {
        public KyloRenCrew() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Kylo Ren";
            Cost = 3;

            isUnique = true;

            UpgradeAbilities.Add(new KyloRenCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}

namespace Abilities
{
    public class KyloRenCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}