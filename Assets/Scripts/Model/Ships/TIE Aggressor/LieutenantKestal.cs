using RuleSets;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace TIEAggressor
    {
        public class LieutenantKestal : TIEAggressor, ISecondEditionPilot
        {
            public LieutenantKestal() : base()
            {
                PilotName = "Lieutenant Kestal";
                PilotSkill = 4;
                Cost = 36;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.LieutenantKestalAbility());

                SEImageNumber = 127;
            }

            public void AdaptPilotToSecondEdition()
            {

            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantKestalAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Cancel,
                int.MaxValue,
                new List<DieSide>() { DieSide.Focus, DieSide.Blank },
                timing: DiceModificationTimingType.Opposite,
                payAbilityCost: PayCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Defence && HostShip.Tokens.HasToken<FocusToken>();
        }

        private int GetAiPriority()
        {
            return 80;
        }

        private void PayCost(Action<bool> callback)
        {
            if (HostShip.Tokens.HasToken<FocusToken>())
            {
                HostShip.Tokens.RemoveToken(typeof(FocusToken), () => callback(true));
            }
            else callback(false);
        }
    }
}
