using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class MagPulseWarheads : GenericSpecialWeapon
    {
        public MagPulseWarheads() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Mag-Pulse Warheads",
                UpgradeType.Missile,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    charges: 2
                ),
                abilityType: typeof(Abilities.SecondEdition.MagPulseDamageAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/8f/95/8f95b9f7-5990-4060-acea-0fc73d026d2a/swz62_mag-pulse-warheads.png";
        }
    }
}


namespace Abilities.SecondEdition
{
    //Attack (Lock): Spend 1 charge. If this attack hits, the defender suffers 1 crit damage and gains 1 deplete and 1 jam token. 
    //Then cancel all hit / crit results.
    public class MagPulseDamageAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterWeaponEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterWeaponEffect;
        }

        protected void RegisterWeaponEffect()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Mag-Pulse weapon effect",
                    TriggerType = TriggerTypes.OnShotHit,
                    TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                    EventHandler = WeaponEffect
                });
            }
        }

        protected void WeaponEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            DamageSourceEventArgs weaponDamage = new DamageSourceEventArgs()
            {
                Source = HostShip,
                DamageType = DamageTypes.ShipAttack
            };

            Combat.Defender.Damage.TryResolveDamage(0, weaponDamage, AssignTokens, 1);                     
        }

        protected void AssignTokens()
        {
            Combat.Defender.Tokens.AssignToken(
                typeof(DepleteToken),
                () => Combat.Defender.Tokens.AssignToken(
                    new JamToken(Combat.Defender, HostShip.Owner), Triggers.FinishTrigger));
        }
    }

}