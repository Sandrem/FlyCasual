using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using RuleSets;

namespace Ship
{
    namespace TIEFighter
    {
        public class NightBeast : TIEFighter, ISecondEditionPilot
        {
            public NightBeast() : base()
            {
                PilotName = "\"Night Beast\"";
                PilotSkill = 5;
                Cost = 15;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.NightBeastAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 26;

                PilotAbilities.RemoveAll(ability => ability is Abilities.NightBeastAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.NightBeastAbilitySE());

                SEImageNumber = 88;
            }
        }
    }
}

namespace Abilities
{
    public class NightBeastAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += NightBeastPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= NightBeastPilotAbility;
        }

        protected void NightBeastPilotAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (HostShip.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Night Beast: Free Focus action",
                        TriggerOwner = ship.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnMovementFinish,
                        EventHandler = PerformFreeFocusAction
                    }
                );
            }
        }

        private void PerformFreeFocusAction(object sender, System.EventArgs e)
        {
            List<ActionsList.GenericAction> actions = new List<ActionsList.GenericAction>() { new ActionsList.FocusAction() };

            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NightBeastAbilitySE : NightBeastAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += NightBeastPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= NightBeastPilotAbility;
        }
    }
}