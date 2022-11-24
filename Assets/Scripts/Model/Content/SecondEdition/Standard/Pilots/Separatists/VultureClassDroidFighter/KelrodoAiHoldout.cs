using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class KelrodoAiHoldout : VultureClassDroidFighter
    {
        public KelrodoAiHoldout()
        {
            //TODO: Half-bio?

            IsWIP = true;

            PilotInfo = new PilotCardInfo25
            (
                "Kelrodo-Ai Holdout",
                "Separatist Stalwart",
                Faction.Separatists,
                2,
                2,
                7,
                limited: 3,
                abilityType: typeof(Abilities.SecondEdition.KelrodoAiHoldoutAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile
                }
            );

            ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/4ea8c128-aa07-420e-8046-9adffd1180e6/SWZ97_KelrodoAIHoldoutlegal.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KelrodoAiHoldoutAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
