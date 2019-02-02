using ActionsList;
using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class AssaultMissiles : GenericSpecialWeapon
    {
        public AssaultMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Assault Missiles",
                UpgradeType.Missile,
                cost: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    spendsToken: typeof(BlueTargetLockToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.AssaultMissilesAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class AssaultMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterAssaultMissleHit;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnShotHitAsAttacker -= RegisterAssaultMissleHit;
        }

        private void RegisterAssaultMissleHit()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Assault Missile Hit",
                    TriggerType = TriggerTypes.OnShotHit,
                    TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                    EventHandler = delegate {
                        AssaultMissilesHitEffect();
                    }
                });
            }
        }

        private void AssaultMissilesHitEffect()
        {
            var ships = Roster.AllShips.Select(x => x.Value).ToList();

            foreach (GenericShip ship in ships)
            {

                // null refs?
                if (ship.Model == null || Combat.Defender == null || Combat.Defender.Model == null)
                {
                    continue;
                }

                // Defending ship shouldn't suffer additional damage
                if (ship.Model == Combat.Defender.Model)
                    continue;

                BoardTools.DistanceInfo shotInfo = new BoardTools.DistanceInfo(Combat.Defender, ship);

                if (shotInfo.Range == 1)
                {

                    //Messages.ShowErrorToHuman(string.Format("{0} is within range 1 of {1}; assault missile deals 1 damage!", ship.PilotName, Combat.Defender.PilotName));

                    DamageSourceEventArgs assaultsplashDamage = new DamageSourceEventArgs()
                    {
                        Source = "Assault Missile",
                        DamageType = DamageTypes.CardAbility
                    };

                    ship.Damage.TryResolveDamage(1, assaultsplashDamage, Triggers.FinishTrigger);
                }
            }
        }
    }
}
