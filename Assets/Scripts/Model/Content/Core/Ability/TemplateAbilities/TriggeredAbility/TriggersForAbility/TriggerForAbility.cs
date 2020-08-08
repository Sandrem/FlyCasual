namespace Abilities
{
    public abstract class TriggerForAbility
    {
        public abstract void Register(TriggeredAbility ability);
        public abstract void Unregister(TriggeredAbility ability);
    }
}
