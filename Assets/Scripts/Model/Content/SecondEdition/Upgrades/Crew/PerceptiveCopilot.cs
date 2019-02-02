using Ship;
using Upgrade;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class PerceptiveCopilot : GenericUpgrade
    {
        public PerceptiveCopilot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Perceptive Copilot",
                UpgradeType.Crew,
                cost: 8,
                abilityType: typeof(Abilities.FirstEdition.ReconSpecialistAbility),
                seImageNumber: 46
            );
        }        
    }
}