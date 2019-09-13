using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class OddBall : BTLBYWing
        {
            public OddBall() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Odd Ball\"",
                    5,
                    44,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Astromech },
                    abilityText: "After you fully execute a red maneuver or perform a red action, if there is an enemy ship in your bullseye arc, you may acquire a lock on that ship.",
                    abilityType: typeof(Abilities.SecondEdition.OddBallAbility)
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/99/a7/99a78a22-4e8c-4197-a7fb-2163746daa90/swz48_pilot-odd-ball.png";
            }
        }
    }
}
