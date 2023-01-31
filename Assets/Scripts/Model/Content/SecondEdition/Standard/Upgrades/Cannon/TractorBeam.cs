using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TractorBeam : GenericSpecialWeapon
    {
        public TractorBeam() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Tractor Beam",
                UpgradeType.Cannon,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3
                ),
                abilityType: typeof(Abilities.SecondEdition.TractorBeamAbility),
                seImageNumber: 30
            );
        }        
    }
}

namespace Abilities.SecondEdition
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

        private void TractorBeamEffect(object sender, System.EventArgs e)
        {
            int tractorBeamTokens = Combat.DiceRollAttack.Successes;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignTokens(() => new TractorBeamToken(Combat.Defender, Combat.Attacker.Owner), tractorBeamTokens, Triggers.FinishTrigger);
        }
    }
}