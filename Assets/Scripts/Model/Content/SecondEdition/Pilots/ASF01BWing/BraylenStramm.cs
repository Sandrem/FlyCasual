using Abilities.SecondEdition;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ASF01BWing
    {
        public class BraylenStramm : ASF01BWing
        {
            public BraylenStramm() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Braylen Stramm",
                    4,
                    47,
                    isLimited: true,
                    abilityType: typeof(BraylenStrammAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 23
                );

                ModelInfo.SkinName = "Dark Blue";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you defend or perform an attack, if you are stressed, you may reroll up to 2 of your dice.
    public class BraylenStrammAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                2
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsDiceModificationAvailable()
        {
            return HostShip.IsStressed && (HostShip.IsAttacking || HostShip.IsDefending);
        }

        private int GetDiceModificationAiPriority()
        {
            if (HostShip.IsStressed)
            {
                return 90;
            }
            return 0;
        }
    }
}
