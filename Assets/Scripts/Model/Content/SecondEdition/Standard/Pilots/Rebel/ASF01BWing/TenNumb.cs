using Abilities.SecondEdition;
using Content;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ASF01BWing
    {
        public class TenNumb : ASF01BWing
        {
            public TenNumb() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Ten Numb",
                    "Blue Five",
                    Faction.Rebel,
                    4,
                    5,
                    14,
                    isLimited: true,
                    abilityType: typeof(TenNumbAbility),
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
                    seImageNumber: 24,
                    skinName: "Dark Blue"
                );
            }
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        //While you defend or perform an attack, you may spend 1 stress token to change all of your focus results to evade or hit results.
        public class TenNumbAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                AddDiceModification(
                    HostName,
                    IsDiceModificationAvailable,
                    GetDiceModificationAiPriority,
                    DiceModificationType.Change,
                    0,
                    new List<DieSide>() { DieSide.Focus },
                    DieSide.Success,
                    payAbilityCost: PayAbilityCost
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
                //Ten Numb likes to have at least one stress
                var stressCount = HostShip.Tokens.CountTokensByType<StressToken>();
                if (stressCount == 0) value = 90;
                else if (stressCount == 1) value = 50;
                else value = 0;
            }

            private bool IsDiceModificationAvailable()
            {
                return HostShip.IsStressed
                    && (HostShip.IsAttacking || HostShip.IsDefending)
                    && Combat.CurrentDiceRoll.Focuses != 0;
            }

            private int GetDiceModificationAiPriority()
            {
                var focusCount = HostShip.Tokens.CountTokensByType<Tokens.FocusToken>();

                switch (focusCount)
                {
                    case 0:
                        return 0;
                    case 1:
                        return 40;
                    default:
                        return 50;
                }
            }

            private void PayAbilityCost(Action<bool> callback)
            {
                if (HostShip.IsStressed)
                {
                    HostShip.Tokens.RemoveToken(typeof(Tokens.StressToken), () => callback(true));
                }
                else callback(false);
            }
        }
    }
}