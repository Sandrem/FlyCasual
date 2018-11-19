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
                    abilityType: typeof(Abilities.SecondEdition.UnkarPluttAbility)
                );

                SEImageNumber = 163;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class UnkarPluttAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += TryRegisterPilotAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= TryRegisterPilotAbility;
        }

        private void TryRegisterPilotAbility()
        {
            if (TargetsForAbilityExist(HostShip.ShipsBumped.Contains))
            {
                AssignTokensToBumpedEnemies();
            }
        }

        protected virtual void AssignTokensToBumpedEnemies()
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
        }

        protected void AssignTractorBeamToken(GenericShip bumpedShip)
        {
            Messages.ShowError(HostShip.PilotName + " assigns Tractor Beam Token\nto " + bumpedShip.PilotName);
            bumpedShip.Tokens.AssignToken(new Tokens.TractorBeamToken(bumpedShip, HostShip.Owner), Triggers.FinishTrigger);
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