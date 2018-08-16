using System;
using UnityEngine;
using Upgrade;
using Ship;
using Tokens;
using Abilities;

namespace UpgradesList
{
    public class Chopper : GenericUpgrade
    {
        public Chopper() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "\"Chopper\"";
            Cost = 0;

            isUnique = true;

            UpgradeAbilities.Add(new ChopperCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
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

            Messages.ShowError("\"Chopper\": Damage is dealt");

            DamageSourceEventArgs chopperDamage = new DamageSourceEventArgs()
            {
                Source = HostShip,
                SourceDescription = this.Name,
                DamageType = DamageTypes.CardAbility
            };

            HostShip.Damage.TryResolveDamage(1, chopperDamage, delegate {
                Phases.CurrentSubPhase.Resume();
                Triggers.FinishTrigger();
            });
        }
    }
}
