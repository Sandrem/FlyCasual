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
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.LandoCalrissianScumPilotAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 223;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LandoCalrissianScumPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotName,
                IsDiceModificationAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                2,
                new List<DieSide>() { DieSide.Blank },
                timing: DiceModificationTimingType.AfterRolled,
                payAbilityCost: PayAbilityCost
            );
        }

        private bool IsDiceModificationAvailable()
        {
            return true;
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

