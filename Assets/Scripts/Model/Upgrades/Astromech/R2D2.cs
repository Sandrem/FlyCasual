using System;
using Upgrade;
using Abilities;
using Ship;
using Board;
using UnityEngine;

namespace UpgradesList
{

    public class R2D2 : GenericUpgrade
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
            if (BoardManager.IsOffTheBoard(host)) return;

            if (host.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Green)
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
