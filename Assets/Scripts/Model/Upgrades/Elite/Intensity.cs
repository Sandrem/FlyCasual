using Upgrade;
using System.Linq;
using Ship;
using ActionsList;
using Tokens;
using Abilities;

namespace UpgradesList
{
    public class Intensity : GenericDualUpgrade
    {
        public Intensity() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Intensity";
            Cost = 2;

            AnotherSide = typeof(IntensityExhausted);

            UpgradeAbilities.Add(new IntensityAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;
        }
    }

    public class IntensityExhausted : GenericDualUpgrade
    {
        public IntensityExhausted() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Intensity (Exhausted)";
            Cost = 2;

            AnotherSide = typeof(Intensity);

            UpgradeAbilities.Add(new IntensityExhaustedAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;
        }
    }
}

namespace Abilities
{
    public class IntensityAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction actionTaken)
        {
            if (actionTaken is BoostAction || actionTaken is BarrelRollAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AssignToken);
            }
        }

        private void AssignToken(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(
                new FocusToken(HostShip),
                FlipUpgade
            );
        }

        private void FlipUpgade()
        {
            (HostUpgrade as GenericDualUpgrade).Flip();
            Triggers.FinishTrigger();
        }
    }

    public class IntensityExhaustedAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseEnd += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseEnd -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.Tokens.HasToken(typeof(FocusToken))) //  || HostShip.Tokens.HasToken(typeof(EvadeToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, RestoreUpgrade);
            }
        }

        private void RestoreUpgrade(object sender, System.EventArgs e)
        {
            HostShip.Tokens.SpendToken(
                typeof(FocusToken),
                FlipUpgade
            );
        }

        private void FlipUpgade()
        {
            (HostUpgrade as GenericDualUpgrade).Flip();
            Triggers.FinishTrigger();
        }
    }
}