using Arcs;

namespace Ship
{
    namespace FirstEdition.YV666
    {
        public class MoraloEval : YV666
        {
            public MoraloEval() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "MoraloEval",
                    6,
                    34,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.MoraloEvalAbility)
                );

                ModelInfo.SkinName = "Crimson";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class MoraloEvalAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            ToggleAbility(true);
        }

        public override void DeactivateAbility()
        {
            ToggleAbility(false);
        }

        private void ToggleAbility(bool isActive)
        {
            foreach (GenericArc arc in HostShip.ArcsInfo.Arcs)
            {
                if (arc is OutOfArc) continue;

                arc.ShotPermissions.CanShootCannon = isActive;
            }
        }

    }
}