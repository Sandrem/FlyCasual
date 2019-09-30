using BoardTools;
using Movement;
using Players;
using Remote;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Remote
{
    public class BuzzDroidSwarm : GenericRemote
    {
        public BuzzDroidSwarm(GenericPlayer owner) : base(owner)
        {
            RemoteInfo = new RemoteInfo(
                "Buzz Droid Swarm",
                0, 3, 1,
                "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/3/38/Remote_BuzzDroidSwarm.png",
                typeof(Abilities.SecondEdition.BuzzDroidSwarmAbiliy)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BuzzDroidSwarmAbiliy : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }

}