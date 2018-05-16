﻿using System;
using Upgrade;
using Abilities;
using Ship;
using BoardTools;
using UnityEngine;
using RuleSets;

namespace UpgradesList
{

    public class R2D2 : GenericUpgrade, ISecondEditionUpgrade
    {
        public R2D2() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R2-D2";
            isUnique = true;
            Cost = 4;

            AvatarOffset = new Vector2(19, 1);

            UpgradeAbilities.Add(new R2D2Ability());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            MaxCharges = 3;

            ImageUrl = "https://i.imgur.com/BDujMs7.png";
        }
    }

}

namespace Abilities
{
    public class R2D2Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementExecuted += R2D2PlanRegenShield;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementExecuted -= R2D2PlanRegenShield;
        }

        private void R2D2PlanRegenShield(GenericShip host)
        {
            if (BoardTools.Board.IsOffTheBoard(host)) return;

            if (host.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                if (host.Shields < host.MaxShields)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnShipMovementExecuted, R2D2RegenShield);
                }
            }
        }

        private void R2D2RegenShield(object sender, EventArgs e)
        {
            if (HostShip.TryRegenShields())
            {
                Sounds.PlayShipSound("R2D2-Proud");
                Messages.ShowInfo("R2-D2: Shield is restored");
            }
            Triggers.FinishTrigger();
        }
    }
}
