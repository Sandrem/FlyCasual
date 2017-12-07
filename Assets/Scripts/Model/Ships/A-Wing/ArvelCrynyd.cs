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

                PilotAbilities.Add(new AbilitiesNamespace.ArvelCrynydAbility());
            }
        }
    }
}

namespace AbilitiesNamespace
{
    public class ArvelCrynydAbility : GenericAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            HostShip.CanAttackBumpedTargetAlways = true;
        }
    }
}