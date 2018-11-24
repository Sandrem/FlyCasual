using System;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEPhPhantom
    {
        public class Whisper : TIEPhPhantom
        {
            public Whisper() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Whisper\"",
                    5,
                    52,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.WhisperAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 131
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WhisperAbility : Abilities.FirstEdition.WhisperAbility
    {
        protected override Type GetTokenType()
        {
            return typeof(EvadeToken);
        }
    }
}
