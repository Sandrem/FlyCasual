using Arcs;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class BombardmentDrone : HyenaClassDroidBomber
    {
        public BombardmentDrone()
        {
            IsHidden = true;

            PilotInfo = new PilotCardInfo(
                "Bombardment Drone",
                3,
                30,
                limited: 3,
                abilityType: typeof(Abilities.SecondEdition.BombardmentDroneAbility),
                pilotTitle: "Time on Target"
            );
            
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c8/3c/c83c098d-1a87-4f08-bcad-ad4cd9c8a00c/swz41_hyena-class-bomber.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BombardmentDroneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            // TODO: Add ability
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}