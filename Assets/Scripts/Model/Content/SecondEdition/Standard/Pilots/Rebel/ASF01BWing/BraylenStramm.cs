using Abilities.SecondEdition;
using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Braylen Stramm",
                    "Blade Leader",
                    Faction.Rebel,
                    4,
                    5,
                    12,
                    isLimited: true,
                    abilityType: typeof(BraylenStrammAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.BWing
                    },
                    seImageNumber: 23,
                    skinName: "Dark Blue"
                );
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
