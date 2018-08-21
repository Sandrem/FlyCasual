using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;
using Ship;
using System;
using Tokens;

namespace UpgradesList
{

    public class R2Astromech : GenericUpgrade, ISecondEditionUpgrade
    {

        public R2Astromech() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R2 Astromech";
            Cost = 1;

            UpgradeAbilities.Add(new R2AstromechAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            MaxCharges = 2;
            Cost = 6;

            UpgradeAbilities.RemoveAll(a => a is R2AstromechAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.R2AstromechAbility());
        }
    }

}

namespace Abilities
{
    public class R2AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckR2AstromechAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckR2AstromechAbility;
        }

        private void CheckR2AstromechAbility(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.ColorComplexity != Movement.MovementComplexity.None)
            {
                if ((movement.Speed == Movement.ManeuverSpeed.Speed1) || (movement.Speed == Movement.ManeuverSpeed.Speed2))
                {
                    movement.ColorComplexity = Movement.MovementComplexity.Easy;
                }
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you reveal your dial, you may spend 1 charge and gain 1 disarm token to recover 1 shield.
    public class R2AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += PlanRegenShield;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= PlanRegenShield;
        }

        private void PlanRegenShield(GenericShip host)
        {
            if (HostShip.Shields < HostShip.MaxShields && HostUpgrade.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, RegenShield, null, null, false, HostName + ": Spend 1 charge and gain 1 disarm token to recover 1 shield?");
        }

        private void RegenShield(object sender, EventArgs e)
        {
            HostUpgrade.SpendCharge(() =>
            {
                HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), () =>
                {
                    if (HostShip.TryRegenShields())
                    {
                        Sounds.PlayShipSound("R2D2-Proud");
                        Messages.ShowInfo(HostName + ": Shield is restored");
                    }
                    Triggers.FinishTrigger();
                });
            });
        }
    }
}

