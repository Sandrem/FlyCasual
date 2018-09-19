using Arcs;
using RuleSets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YV666
    {
        public class MoraloEval : YV666, ISecondEditionPilot
        {
            public MoraloEval() : base()
            {
                PilotName = "Moralo Eval";
                PilotSkill = 6;
                Cost = 34;

                IsUnique = true;

                SkinName = "Crimson";

                PilotAbilities.Add(new Abilities.MoraloEvalAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 72;

                UsesCharges = true;
                MaxCharges = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.RemoveAll(a => a is Abilities.MoraloEvalAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.MoraloEvalAbilitySE());

                SEImageNumber = 211;
            }
        }
    }
}

namespace Abilities
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
            foreach (GenericArc arc in HostShip.ArcInfo.Arcs)
            {
                if (arc is OutOfArc) continue;

                arc.ShotPermissions.CanShootCannon = isActive;
            }
        }

    }
}

namespace Abilities.SecondEdition
{
    public class MoraloEvalAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnOffTheBoard += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnOffTheBoard -= CheckAbility;
        }

        private void CheckAbility(ref bool shouldBeDestroyed, Direction direction)
        {
            Debug.Log(direction);

            if (HostShip.Charges > 0)
            {
                HostShip.SpendCharge(delegate { }); // Safe - Sandrem
                shouldBeDestroyed = false;

                Messages.ShowInfo(HostShip.PilotName + " is moved to Reserve");

                Roster.MoveToReserve(HostShip);

                Phases.Events.OnPlanningPhaseStart += RegisterSetup;
            }
        }

        private void RegisterSetup()
        {
            Phases.Events.OnPlanningPhaseStart -= RegisterSetup;

            RegisterAbilityTrigger(TriggerTypes.OnPlanningSubPhaseStart, SetupShip);
        }

        private void SetupShip(object sender, System.EventArgs e)
        {
            Roster.ReturnFromReserve(HostShip);
            HostShip.SetPosition(Vector3.zero);

            Triggers.FinishTrigger();
        }
    }
}
