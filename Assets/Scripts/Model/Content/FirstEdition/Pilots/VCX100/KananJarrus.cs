using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace FirstEdition.VCX100
    {
        public class KananJarrus : VCX100
        {
            public KananJarrus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kanan Jarrus",
                    5,
                    38,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KananJarrusPilotAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class KananJarrusPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            throw new System.NotImplementedException();
        }

        public override void DeactivateAbility()
        {
            throw new System.NotImplementedException();
        }
    }

}