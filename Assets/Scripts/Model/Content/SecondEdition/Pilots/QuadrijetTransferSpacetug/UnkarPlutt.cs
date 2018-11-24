using Ship;

namespace Ship
{
    namespace SecondEdition.QuadrijetTransferSpacetug
    {
        public class UnkarPlutt : QuadrijetTransferSpacetug
        {
            public UnkarPlutt() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Unkar Plutt",
                    2,
                    30,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.UnkarPluttAbility),
                    seImageNumber: 163
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class UnkarPluttAbility : Abilities.FirstEdition.UnkarPluttAbility
    {
        protected override void AssignTokensToBumpedEnemies()
        {
            foreach (GenericShip ship in HostShip.ShipsBumped)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Assign tractor beam to " + ship.PilotName,
                    TriggerType = TriggerTypes.OnCombatPhaseStart,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = delegate { AssignTractorBeamToken(ship); }
                });
            }

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Assign tractor beam to " + HostShip.PilotName,
                TriggerType = TriggerTypes.OnCombatPhaseStart,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = delegate { AssignTractorBeamToken(HostShip); }
            });
        }
    }
}