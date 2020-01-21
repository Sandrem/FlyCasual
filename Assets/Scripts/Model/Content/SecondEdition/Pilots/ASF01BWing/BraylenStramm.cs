using Abilities.SecondEdition;
using System.Collections;
using System.Collections.Generic;
using Tokens;
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
                    52,
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
            HostShip.OnModifyAIStressPriority += ModifyAIStressPriority;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
            HostShip.OnModifyAIStressPriority -= ModifyAIStressPriority;
        }

        private void ModifyAIStressPriority(Ship.GenericShip ship, ref int value)
        {
            //Braylen wants to always have 1 stress token
            if (HostShip.Tokens.CountTokensByType<StressToken>() == 0) value = 90;
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
