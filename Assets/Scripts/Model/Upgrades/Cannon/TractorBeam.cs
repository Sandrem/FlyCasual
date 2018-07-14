using Abilities;
using Upgrade;
using UpgradesList;
using Tokens;

namespace UpgradesList
{

    public class TractorBeam : GenericSecondaryWeapon
    {
        public TractorBeam() : base()
        {
            Types.Add(UpgradeType.Cannon);

            Name = "Tractor Beam";
            Cost = 1;

            MinRange = 1;
            MaxRange = 3;
            AttackValue = 3;

            UpgradeAbilities.Add(new TractorBeamAbility());
        }
    }
}

namespace Abilities
{
    public class TractorBeamAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterTractorBeamEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterTractorBeamEffect;
        }

        private void RegisterTractorBeamEffect()
        {
            if (Combat.ChosenWeapon is TractorBeam)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotHit, TractorBeamEffect);
            }
        }

        private void TractorBeamEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            TractorBeamToken token = new TractorBeamToken(Combat.Defender, Combat.Attacker.Owner);
            Combat.Defender.Tokens.AssignToken(token, Triggers.FinishTrigger);
        }
    }
}