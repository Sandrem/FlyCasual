using Ship;

namespace Ship
{
    namespace FirstEdition.Quadjumper
    {
        public class UnkarPlutt : Quadjumper
        {
            public UnkarPlutt() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Unkar Plutt",
                    3,
                    17,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.UnkarPluttAbility)
                );
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
