using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;
using RuleSets;
using SubPhases;

namespace Ship
{
    namespace Quadjumper
    {
        public class SarcoPlank : Quadjumper, ISecondEditionPilot
        {
            public SarcoPlank() : base()
            {
                PilotName = "Sarco Plank";
                PilotSkill = 2;
                Cost = 31;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new Abilities.SecondEdition.SarcoPlankAbilitySE());

                SEImageNumber = 162;
            }

            public void AdaptPilotToSecondEdition()
            {
                
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SarcoPlankAbilitySE : GenericAbility
    {
        private int OriginalAgility;

        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsDefender += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsDefender -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            if (HostShip.AssignedManeuver != null)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDefenseStart, AskToUseSarcoPlankAbility);
            }
        }

        private void AskToUseSarcoPlankAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                ShouldUseAbility,
                ChangeAgility,
                infoText: "Treat your agility value as " + HostShip.AssignedManeuver.Speed + "?"
            );
        }

        private bool ShouldUseAbility()
        {
            return HostShip.AssignedManeuver.Speed > HostShip.Agility;
        }

        private void ChangeAgility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotName + ": Agility is " + HostShip.AssignedManeuver.Speed);

            OriginalAgility = HostShip.Agility;
            HostShip.ChangeAgilityBy(HostShip.AssignedManeuver.Speed - HostShip.Agility);

            HostShip.OnAttackFinishAsDefender += RestoreOriginalAgility;

            DecisionSubPhase.ConfirmDecision();
        }

        private void RestoreOriginalAgility(GenericShip ship)
        {
            HostShip.OnAttackFinishAsDefender -= RestoreOriginalAgility;

            Messages.ShowInfo(HostShip.PilotName + ": Agility is " + OriginalAgility);
            HostShip.ChangeAgilityBy(OriginalAgility - HostShip.AssignedManeuver.Speed);
        }
    }
}