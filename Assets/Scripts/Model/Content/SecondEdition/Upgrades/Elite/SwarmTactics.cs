using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class SwarmTactics : GenericUpgrade
    {
        public SwarmTactics() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Swarm Tactics",
                UpgradeType.Elite,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.SwarmTacticsAbility),
                seImageNumber: 17
            );
        }        
    }
}