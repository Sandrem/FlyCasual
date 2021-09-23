using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class JynErso : GenericUpgrade
    {
        public JynErso() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Jyn Erso",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.JynErsoAbility),
                seImageNumber: 85
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(426, 18),
                new Vector2(125, 125)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class JynErsoAbility : FirstEdition.JanOrsCrewAbility
    {
        protected override void MarkAbilityAsUsed()
        {
            // Do nothing
        }
    }
}
