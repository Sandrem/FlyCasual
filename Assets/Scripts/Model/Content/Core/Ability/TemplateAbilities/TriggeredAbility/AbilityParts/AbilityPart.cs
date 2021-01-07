using Upgrade;

namespace Abilities
{
    public abstract class AbilityPart
    {
        public GenericUpgrade TargetUpgrade { get; set; }

        public abstract void DoAction(GenericAbility ability);
    }
}
