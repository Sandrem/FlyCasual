namespace Ship
{
    namespace FirstEdition.AWing
    {
        public class ArvelCrynyd : AWing
        {
            public ArvelCrynyd() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Arvel Crynyd",
                    6,
                    23,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ArvelCrynydAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
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