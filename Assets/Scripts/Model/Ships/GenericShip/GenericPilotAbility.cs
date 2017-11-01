using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship;
using SubPhases;

namespace PilotAbilities
{
    public class GenericPilotAbility
    {
        public string Name;
        private GenericShip host;

        public GenericShip Host
        {
            get { return host; }
            private set { host = value; }
        }

        public virtual void Initialize(GenericShip host)
        {
            Host = host;
            Name = Host.PilotName + "'s ability";
        }

        protected void RegisterAbilityTrigger(TriggerTypes triggerType, EventHandler eventHandler)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = Name,
                TriggerType = triggerType,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = eventHandler,
                Sender = Host
            });
        }

        protected void AskToUseAbility(bool useByDefault, EventHandler useAbility, EventHandler dontUseAbility = null)
        {
            if (dontUseAbility == null) dontUseAbility = DontUseAbility;

            DecisionSubPhase pilotAbilityDecision = (DecisionSubPhase) Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(PilotAbilityDecisionSubphase),
                Triggers.FinishTrigger
            );

            pilotAbilityDecision.InfoText = "Use " + Name + "?";

            pilotAbilityDecision.AddDecision("Yes", useAbility);
            pilotAbilityDecision.AddDecision("No", dontUseAbility);

            pilotAbilityDecision.DefaultDecision = (useByDefault) ? "Yes" : "No";

            pilotAbilityDecision.Start();
        }

        public class PilotAbilityDecisionSubphase : DecisionSubPhase { }

        private void DontUseAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }
    }
}
