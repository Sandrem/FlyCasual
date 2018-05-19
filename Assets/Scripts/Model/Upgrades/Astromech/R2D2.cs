using System;
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

            UpgradeAbilities.RemoveAll(a => a is R2D2Ability);
            UpgradeAbilities.Add(new Abilities.SecondEdition.R2D2Ability());
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



namespace Abilities.SecondEdition
{
    //After you reveal your dial, you may spend 1 charge and gain 1 disarm token to recover 1 shield.
    public class R2D2Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += R2D2PlanRegenShield;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= R2D2PlanRegenShield;
        }

        private void R2D2PlanRegenShield(GenericShip host)
        {
            if (HostShip.Shields < HostShip.MaxShields)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, R2D2RegenShield, null, null, false, "R2-D2: Spend 1 charge and gain 1 disarm token to recover 1 shield?");
        }

        private void R2D2RegenShield(object sender, EventArgs e)
        {
            HostUpgrade.SpendCharge(() =>
            {
                HostShip.Tokens.AssignToken(new Tokens.WeaponsDisabledToken(HostShip), () =>
                {
                    if (HostShip.TryRegenShields())
                    {
                        Sounds.PlayShipSound("R2D2-Proud");
                        Messages.ShowInfo("R2-D2: Shield is restored");
                    }
                    Triggers.FinishTrigger();
                });
            });
        }
    }
}

