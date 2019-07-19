using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote
{
    public class Drk1ProbeDroid : GenericRemote
    {
        public Drk1ProbeDroid()
        {
            RemoteInfo = new RemoteInfo(
                "DRK-1 Probe Droid",
                0, 3, 1,
                "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/c9/Swz30_probe-card.png",
                typeof(Abilities.SecondEdition.Drk1ProbeDroidGenericAbility),
                systemPhaseAbility: typeof(Abilities.SecondEdition.Drk1ProbeDroidSystemAbility)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Drk1ProbeDroidGenericAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }

    public class Drk1ProbeDroidSystemAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}