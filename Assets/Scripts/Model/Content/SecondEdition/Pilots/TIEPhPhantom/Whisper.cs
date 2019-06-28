﻿using System;
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
                    57,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WhisperAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
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
