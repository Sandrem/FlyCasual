using System;
using Upgrade;

namespace UpgradesList
{
    public class Chopper : GenericUpgrade
    {
        public Chopper() : base()
        {
            Type = UpgradeType.Crew;
            Name = "\"Chopper\"";
            Cost = 0;

            isUnique = true;
        }

        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.CanPerformActionsWhileStressed = true;
            host.OnActionIsPerformed += RegisterDoDamageIfStressed;
        }

        private void RegisterDoDamageIfStressed(ActionsList.GenericAction action)
        {
            if (Host.HasToken(typeof(Tokens.StressToken)))
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Check damage from \"Chopper\"",
                    TriggerType = TriggerTypes.OnActionIsPerformed,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = DoDamage,
                });
            }
        }

        private void DoDamage(object sender, System.EventArgs e)
        {
            Messages.ShowError("\"Chopper\": Damage is dealt");

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage from \"Chopper\"",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = Host.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = this,
                    DamageType = DamageTypes.CardAbility
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Triggers.FinishTrigger);
        }

        public override void Discard(Action callBack)
        {
            Host.CanPerformActionsWhileStressed = false;
            Host.OnActionIsPerformed -= RegisterDoDamageIfStressed;

            base.Discard(callBack);
        }
    }
}
