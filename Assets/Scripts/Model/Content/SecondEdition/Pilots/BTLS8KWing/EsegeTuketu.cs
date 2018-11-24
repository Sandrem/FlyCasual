using BoardTools;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace SecondEdition.BTLS8KWing
    {
        public class EsegeTuketu : BTLS8KWing
        {
            public EsegeTuketu() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Esege Tuketu",
                    3,
                    50,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.EsegeTuketuAbility),
                    seImageNumber: 63
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EsegeTuketuAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                () => 20,
                DiceModificationType.Change,
                int.MaxValue,
                new List<DieSide> { DieSide.Focus },
                DieSide.Success,
                isGlobal: true,
                payAbilityCost: PayAbilityCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            GenericShip activeShip = Combat.AttackStep == CombatStep.Attack ? Combat.Attacker : Combat.Defender;
            return activeShip.Owner == HostShip.Owner
                && activeShip != HostShip
                && Board.IsShipBetweenRange(HostShip, activeShip, 0, 2)
                && HostShip.Tokens.HasToken(typeof(FocusToken));
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            if (HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                HostShip.Tokens.RemoveToken(typeof(FocusToken), () => callback(true));
            }
            else callback(false);
        }
    }
}
