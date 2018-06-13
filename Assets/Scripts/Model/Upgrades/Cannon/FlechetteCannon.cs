using Abilities;
using Tokens;
using Upgrade;
using UpgradesList;

namespace UpgradesList
{

	public class FlechetteCannon : GenericSecondaryWeapon
	{
		public FlechetteCannon() : base()
		{
            Types.Add(UpgradeType.Cannon);

            Name = "Flechette Cannon";
            Cost = 2;

            MinRange = 1;
            MaxRange = 3;
            AttackValue = 3;

            UpgradeAbilities.Add(new FlechetteCannonAbility());
        }
    }
}

namespace Abilities
{
    public class FlechetteCannonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterFlechetteCannonEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterFlechetteCannonEffect;
        }

        private void RegisterFlechetteCannonEffect()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotHit, FlechetteCannonEffect);
            }
        }

        private void FlechetteCannonEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            DefenderSuffersDamage();
        }

        private void DefenderSuffersDamage()
        {
            Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Success);

            var trigger = RegisterAbilityTrigger(TriggerTypes.OnDamageIsDealt, Combat.Defender.SufferDamage, new DamageSourceEventArgs()
            {
                Source = Combat.Attacker,
                DamageType = DamageTypes.ShipAttack
            });
            trigger.Skippable = true;

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CheckStress);
        }

        private void CheckStress()
        {
            if (!Combat.Defender.Tokens.HasToken(typeof(StressToken)))
            {
                Combat.Defender.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

    }
}