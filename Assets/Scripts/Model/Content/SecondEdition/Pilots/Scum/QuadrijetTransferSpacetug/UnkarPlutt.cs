using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.QuadrijetTransferSpacetug
    {
        public class UnkarPlutt : QuadrijetTransferSpacetug
        {
            public UnkarPlutt() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Unkar Plutt",
                    "Miserly Portion Master",
                    Faction.Scum,
                    2,
                    4,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.UnkarPluttAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Tech,
                        UpgradeType.Crew,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    seImageNumber: 163,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
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
                    Name = "Assign tractor beam to " + ship.PilotInfo.PilotName,
                    TriggerType = TriggerTypes.OnCombatPhaseStart,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = delegate { AssignTractorBeamToken(ship); }
                });
            }

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Assign tractor beam to " + HostShip.PilotInfo.PilotName,
                TriggerType = TriggerTypes.OnCombatPhaseStart,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = delegate { AssignTractorBeamToken(HostShip); }
            });
        }

        protected void AssignTractorBeamToken(GenericShip bumpedShip)
        {
            Selection.ChangeActiveShip(HostShip);
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " assigns a Tractor Beam Token\nto " + bumpedShip.PilotInfo.PilotName);

            bumpedShip.Tokens.AssignToken(new Tokens.TractorBeamToken(bumpedShip, HostShip.Owner), Triggers.FinishTrigger);
        }
    }
}