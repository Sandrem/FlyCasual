using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;
using RuleSets;

namespace Ship
{
    namespace Quadjumper
    {
        public class UnkarPlutt : Quadjumper, ISecondEditionPilot
        {
            public UnkarPlutt() : base()
            {
                PilotName = "Unkar Plutt";
                PilotSkill = 3;
                Cost = 17;

                IsUnique = true;

                PilotAbilities.Add (new UnkarPluttAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 30;

                PilotAbilities.RemoveAll(ability => ability is Abilities.UnkarPluttAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.UnkarPluttAbilitySE());

                SEImageNumber = 163;
            }
        }
    }
}

namespace Abilities
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
                Triggers.RegisterTrigger(new Trigger() {
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
    public class UnkarPluttAbilitySE : UnkarPluttAbility
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