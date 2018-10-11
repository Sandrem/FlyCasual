using Arcs;
using RuleSets;
using SubPhases;
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
        Direction ShipFledSide;

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
            if (HostShip.Charges > 0)
            {
                ShipFledSide = direction;

                HostShip.SpendCharge();
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

            var subphase = Phases.StartTemporarySubPhaseNew<SetupShipMidgameSubPhase>(
                "Setup",
                delegate{
                    Messages.ShowInfo(HostShip.PilotName + " returned to the play area");
                    Triggers.FinishTrigger();
                }
            );

            subphase.ShipToSetup = HostShip;
            subphase.SetupSide = ShipFledSide;
            subphase.AbilityName = HostShip.PilotName;
            subphase.Description = "Place yourself within range 1 of the edge of the play area that you fled from";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }
    }
}
