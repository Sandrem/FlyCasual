using System;
using Upgrade;
using Abilities;
using Ship;
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

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 8;
            MaxCharges = 3;
            UsesCharges = true;

            UpgradeAbilities.RemoveAll(a => a is R2D2Ability);
            UpgradeAbilities.Add(new Abilities.SecondEdition.R2AstromechAbility());
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