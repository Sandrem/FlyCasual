﻿using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class FifthBrother : GenericUpgrade
    {
        public FifthBrother() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Fifth Brother",
                UpgradeType.Gunner,
                cost: 12,
                isLimited: true,
                addForce: 1,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.FifthBrotherGunnerAbility),
                seImageNumber: 122
            );

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(422, 14),
                new Vector2(150, 150)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class FifthBrotherGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                new System.Collections.Generic.List<DieSide>() { DieSide.Focus },
                DieSide.Crit,
                payAbilityCost: PayForce
            );
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && HostShip.State.Force > 0
                && Combat.CurrentDiceRoll.Focuses > 0;
        }

        private int GetAiPriority()
        {
            return 45;
        }

        private void PayForce(Action<bool> callback)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.State.SpendForce(1, delegate { callback(true); });
            }
            else
            {
                Messages.ShowError(HostUpgrade.UpgradeInfo.Name + ": " + HostShip.PilotInfo.PilotName + " has no available Force to spend");
                callback(false);
            }
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}