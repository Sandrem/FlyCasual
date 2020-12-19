using Ship;

namespace Abilities
{
    public class AfterYourAttackHits : TriggerForAbility
    {
        private TriggeredAbility Ability;

        public IShipWeapon WeaponIsNeeded;

        public AfterYourAttackHits(IShipWeapon weaponIsNeeded = null)
        {
            WeaponIsNeeded = weaponIsNeeded;
        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            ability.HostShip.OnShotHitAsAttacker += CheckConditions;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            ability.HostShip.OnShotHitAsAttacker -= CheckConditions;
        }

        private void CheckConditions()
        {
            if (WeaponIsNeeded == null || Combat.ChosenWeapon == WeaponIsNeeded)
            {
                Ability.RegisterAbilityTrigger
                (
                    TriggerTypes.OnShotHit,
                    delegate { Ability.Action.DoAction(Ability); }
                );
            }
        }
    }
}
