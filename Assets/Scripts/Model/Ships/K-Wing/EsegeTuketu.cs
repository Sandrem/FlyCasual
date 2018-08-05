using Abilities.SecondEdition;
using BoardTools;
using RuleSets;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace KWing
    {
        public class EsegeTuketu : KWing, ISecondEditionPilot
        {
            public EsegeTuketu() : base()
            {
                PilotName = "Esege Tuketu";
                PilotSkill = 8;
                Cost = 29;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new EsegeTuketuAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
               // Not needed
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EsegeTuketuAbilitySE : GenericAbility
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