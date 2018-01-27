using Abilities;
using Upgrade;
using UpgradesList;

namespace UpgradesList
{

    public class FlechetteCannon : GenericSecondaryWeapon
    {
        public FlechetteCannon() : base()
        {
            Type = UpgradeType.Cannon;

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
            if (Combat.ChosenWeapon is FlechetteCannon)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Flechette Cannon effect",
                    TriggerType = TriggerTypes.OnShotHit,
                    TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                    EventHandler = FlechetteCannonEffect
                });
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

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Combat.Defender.Owner.PlayerNo,
                EventHandler = Combat.Defender.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = Combat.Attacker,
                    DamageType = DamageTypes.ShipAttack
                },
                Skippable = true
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CheckStress);
        }

        private void CheckStress()
        {
            if (!Combat.Defender.HasToken(typeof(Tokens.StressToken)))
            {
                Combat.Defender.AssignToken(
                    new Tokens.StressToken(),
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

    }
}