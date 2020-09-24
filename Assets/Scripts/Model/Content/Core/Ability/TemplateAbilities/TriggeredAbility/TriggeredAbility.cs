using UnityEngine;

namespace Abilities
{
    public abstract class TriggeredAbility : TemplateAbility
    {
        public abstract TriggerForAbility Trigger { get; }
        public abstract AbilityPart Action { get; }

        public override void ActivateAbility()
        {
            Trigger.Register(this);
        }

        public override void DeactivateAbility()
        {
            Trigger.Unregister(this);
        }
    }
}
