using Abilities.SecondEdition;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ASF01BWing
    {
        public class TenNumb : ASF01BWing
        {
            public TenNumb() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ten Numb",
                    4,
                    46,
                    isLimited: true,
                    abilityType: typeof(TenNumbAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 24
                );

                ModelInfo.SkinName = "Dark Blue";
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
            }

            public override void DeactivateAbility()
            {
                RemoveDiceModification();
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