using Upgrade;
using Ship;
using Tokens;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class SawGerreraCrew : GenericUpgrade
    {
        public SawGerreraCrew() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Saw Gerrera",
                UpgradeType.Crew,
                cost: 9,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.SawGerreraCrewAbility),
                seImageNumber: 93
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(384, 0),
                new Vector2(200, 200)
            );
        }        
    }
}