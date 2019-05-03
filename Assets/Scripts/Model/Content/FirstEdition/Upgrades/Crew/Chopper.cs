using Ship;
using Upgrade;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class Chopper : GenericUpgrade
    {
        public Chopper() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Chopper\"",
                UpgradeType.Crew,
                cost: 0,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.ChopperCrewAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ChopperCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanPerformActionsWhileStressed = true;
            HostShip.OnActionIsPerformed += RegisterDoDamageIfStressed;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanPerformActionsWhileStressed = false;
            HostShip.OnActionIsPerformed -= RegisterDoDamageIfStressed;
        }

        private void RegisterDoDamageIfStressed(ActionsList.GenericAction action)
        {
            if (HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Check damage from \"Chopper\"",
                    TriggerType = TriggerTypes.OnActionIsPerformed,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = DoDamage,
                });
            }
        }

        private void DoDamage(object sender, System.EventArgs e)
        {
            Phases.CurrentSubPhase.Pause();

            Messages.ShowInfo("\"Chopper\": This ship suffers 1 damage.");

            DamageSourceEventArgs chopperDamage = new DamageSourceEventArgs()
            {
                Source = this,
                DamageType = DamageTypes.CardAbility
            };

            HostShip.Damage.TryResolveDamage(1, chopperDamage, delegate {
                Phases.CurrentSubPhase.Resume();
                Triggers.FinishTrigger();
            });
        }
    }
}