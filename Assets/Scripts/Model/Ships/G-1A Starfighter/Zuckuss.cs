using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;
using RuleSets;

namespace Ship
{
    namespace G1AStarfighter
    {
        public class Zuckuss : G1AStarfighter, ISecondEditionPilot
        {
            public Zuckuss() : base()
            {
                PilotName = "Zuckuss";
                PilotSkill = 7;
                Cost = 28;

                IsUnique = true;

                UpgradeBar.AddSlot(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new ZuckussAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 47;
            }
        }
    }
}

namespace Abilities
{
    public class ZuckussAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterZuckussAbility;
            HostShip.OnAttackFinishAsAttacker += RemoveZuckussAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterZuckussAbility;
            HostShip.OnAttackFinishAsAttacker -= RemoveZuckussAbility;
        }

        private void RegisterZuckussAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, ShowDecision);
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, UseAbility);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;

            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += ZuckussAddAttackDice;
            Combat.Defender.AfterGotNumberOfDefenceDice += ZuckussAddAttackDice;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void RemoveZuckussAbility(GenericShip genericShip)
        {
            // At the end of combat phase, need to remove attack value increase
            if (IsAbilityUsed)
            {
                HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= ZuckussAddAttackDice;
                Combat.Defender.AfterGotNumberOfDefenceDice -= ZuckussAddAttackDice;
                IsAbilityUsed = false;
            }
        }
        private void ZuckussAddAttackDice(ref int value)
        {
            value++;
        }
    }
}