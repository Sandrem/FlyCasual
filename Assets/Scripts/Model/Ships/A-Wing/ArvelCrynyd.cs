namespace Ship
{
    namespace AWing
    {
        public class ArvelCrynyd : AWing
        {
            public ArvelCrynyd() : base()
            {
                PilotName = "Arvel Crynyd";
                PilotSkill = 6;
                Cost = 23;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.ArvelCrynydAbility());
            }
        }
    }
}

namespace Abilities
{
    public class ArvelCrynydAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanAttackBumpedTargetAlways = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanAttackBumpedTargetAlways = false;
        }
    }
}