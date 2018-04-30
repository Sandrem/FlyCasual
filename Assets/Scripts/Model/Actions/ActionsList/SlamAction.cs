using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;

namespace ActionsList
{

    public class SlamAction : GenericAction
    {

        public SlamAction()
        {
            Name = EffectName = "SLAM";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/SlamAction.png";
        }

        public override bool CanBePerformedAsAFreeAction
        {
            get
            {
                return false;
            }
        }

        public override void ActionTake()
        {
            if (Selection.ThisShip.Owner.GetType() == typeof(Players.HotacAiPlayer))
            {
                Phases.CurrentSubPhase.CallBack();
            }
            else
            {
                Phases.CurrentSubPhase.Pause();

                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "SLAM Planning",
                        TriggerType = TriggerTypes.OnAbilityDirect,
                        TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                        EventHandler = SelectSlamManeuver
                    }
                );

                Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, RegisterSlamManeuverExecutionTrigger);
            }
        }

        private void SelectSlamManeuver(object sender, System.EventArgs e)
        {
            Selection.ThisShip.Owner.SelectManeuver(GameMode.CurrentGameMode.AssignManeuver, IsSameSpeed);
        }

        private void RegisterSlamManeuverExecutionTrigger()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "SLAM Execution",
                TriggerType = TriggerTypes.OnManeuver,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = PerformSlamManeuver
            });

            Triggers.ResolveTriggers(
                TriggerTypes.OnManeuver,
                AssignWeaponsDisabledToken
            );
        }

        private void AssignWeaponsDisabledToken()
        {
            Selection.ThisShip.Tokens.AssignToken(
                new Tokens.WeaponsDisabledToken(Selection.ThisShip),
                Phases.CurrentSubPhase.CallBack
            );
        }

        private void PerformSlamManeuver(object sender, System.EventArgs e)
        {
            Selection.ThisShip.AssignedManeuver.Perform();
        }

        private bool IsSameSpeed(string maneuverString)
        {
            bool result = false;
            Movement.MovementStruct movementStruct = new Movement.MovementStruct(maneuverString);
            if (movementStruct.Speed == Selection.ThisShip.AssignedManeuver.ManeuverSpeed)
            {
                result = true;
            }
            return result;
        }

        public override int GetActionPriority()
        {
            int result = 0;
            return result;
        }

    }

}
