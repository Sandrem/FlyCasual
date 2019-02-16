using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CustomizedYT1300LightFreighter
    {
        public class LandoCalrissian : CustomizedYT1300LightFreighter
        {
            public LandoCalrissian() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lando Calrissian",
                    4,
                    49,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LandoCalrissianScumPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 223
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you roll dice, if you are not stressed, you may gain 1 stress token to reroll all of your blank results.
    public class LandoCalrissianScumPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsDiceModificationAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                int.MaxValue,
                new List<DieSide>() { DieSide.Blank },
                timing: DiceModificationTimingType.AfterRolled,
                payAbilityCost: PayAbilityCost
            );
        }

        private bool IsDiceModificationAvailable()
        {
            return !HostShip.IsStressed;
        }

        private int GetAiPriority()
        {
            return 95;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.StressToken), () => callback(true));
        }
    }
}

