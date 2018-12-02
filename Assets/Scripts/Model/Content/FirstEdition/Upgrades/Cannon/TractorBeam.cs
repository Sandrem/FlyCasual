using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class TractorBeam : GenericSpecialWeapon
    {
        public TractorBeam() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Tractor Beam",
                UpgradeType.Cannon,
                cost: 1,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 2
                ),
                abilityType: typeof(Abilities.FirstEdition.TractorBeamAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            if (Combat.ChosenWeapon.GetType() == HostUpgrade.GetType())
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotHit, TractorBeamEffect);
            }
        }

        protected virtual void TractorBeamEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            TractorBeamToken token = new TractorBeamToken(Combat.Defender, Combat.Attacker.Owner);
            Combat.Defender.Tokens.AssignToken(token, Triggers.FinishTrigger);
        }
    }
}