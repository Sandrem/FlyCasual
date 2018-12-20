using Upgrade;
using Ship;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class HeraSyndulla : GenericUpgrade
    {
        public HeraSyndulla() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hera Syndulla",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.HeraSyndullaCrewAbility),
                seImageNumber: 84
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class HeraSyndullaCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanPerformRedManeuversWhileStressed = true;
            HostShip.OnMovementFinishSuccessfully += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanPerformRedManeuversWhileStressed = false;
            HostShip.OnMovementFinishSuccessfully -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, CheckStressRemoval);
        }

        private void CheckStressRemoval(object sender, System.EventArgs e)
        {
            if (HostShip.Tokens.CountTokensByType<StressToken>() >= 3)
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Stress token is removed, damage is suffered");
                HostShip.Tokens.RemoveToken(typeof(StressToken), SufferDamage);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SufferDamage()
        {
            DamageSourceEventArgs damageArgs = new DamageSourceEventArgs()
            {
                DamageType = DamageTypes.CardAbility,
                Source = HostUpgrade
            };

            HostShip.Damage.TryResolveDamage(1, damageArgs, Triggers.FinishTrigger);
        }
    }
}