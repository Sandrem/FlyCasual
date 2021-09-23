using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESeBomber
    {
        public class Grudge : TIESeBomber
        {
            public Grudge() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo
                (
                    "\"Grudge\"",
                    2,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GrudgePilotAbility)
                );

                ImageUrl = "https://i.imgur.com/f24aFJJ.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GrudgePilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
