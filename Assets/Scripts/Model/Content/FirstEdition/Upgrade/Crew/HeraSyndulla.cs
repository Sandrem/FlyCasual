using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class HeraSyndulla : GenericUpgrade
    {
        public HeraSyndulla() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hera Syndulla",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restrictionFaction: Faction.Rebel,
                abilityType: typeof(Abilities.FirstEdition.HeraSyndullaCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(36, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class HeraSyndullaCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanPerformRedManeuversWhileStressed = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanPerformRedManeuversWhileStressed = false;
        }
    }
}