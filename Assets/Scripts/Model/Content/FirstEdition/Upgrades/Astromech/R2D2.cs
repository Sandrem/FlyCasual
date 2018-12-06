using System;
using Upgrade;
using Ship;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class R2D2 : GenericUpgrade
    {
        public R2D2() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2-D2",
                UpgradeType.Astromech,
                cost: 4,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.R2D2Ability)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(19, 1));
        }
    }
}

namespace Abilities.FirstEdition
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
                if (host.State.ShieldsCurrent < host.State.ShieldsMax)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnMovementExecuted, R2D2RegenShield);
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