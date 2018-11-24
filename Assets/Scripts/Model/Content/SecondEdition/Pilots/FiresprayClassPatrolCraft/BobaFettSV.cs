﻿using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class BobaFettSV : FiresprayClassPatrolCraft
        {
            public BobaFettSV() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Boba Fett",
                    5,
                    80,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.BobaFettScumAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 149
                );

                ModelInfo.SkinName = "Boba Fett";
            }
        }
    }
}