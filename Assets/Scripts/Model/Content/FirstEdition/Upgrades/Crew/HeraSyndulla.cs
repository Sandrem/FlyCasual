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
                restriction: new FactionRestriction(Faction.Rebel),
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
            HostShip.OnTryCanPerformRedManeuverWhileStressed += AllowRedManeuversWhileStressed;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTryCanPerformRedManeuverWhileStressed -= AllowRedManeuversWhileStressed;
        }

        private void AllowRedManeuversWhileStressed(ref bool isAllowed)
        {
            isAllowed = true;
        }
    }
}