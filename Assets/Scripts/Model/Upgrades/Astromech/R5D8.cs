﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class R5D8 : GenericUpgrade
    {

        public R5D8() : base()
        {
            Type = UpgradeType.Astromech;
            Name = "R5-D8";
            isUnique = true;
            Cost = 3;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += R5D8AddAction;
        }

        private void R5D8AddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction action = new ActionsList.R5D8Action();
            action.ImageUrl = ImageUrl;
            host.AddAvailableAction(action);
        }

    }

}

namespace ActionsList
{

    public class R5D8Action : GenericAction
    {
        public R5D8Action()
        {
            Name = EffectName = "R5-D8: Try to repair";
        }

        public override void ActionTake()
        {
            Selection.ActiveShip = Selection.ThisShip;
            Phases.StartTemporarySubPhase(
                "R5-D8: Try to repair",
                typeof(SubPhases.R5D8CheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.R5D8CheckSubPhase));
                    Phases.CurrentSubPhase.CallBack();
                }
            );
        }
    }

}

namespace SubPhases
{

    public class R5D8CheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            diceType = DiceKind.Defence;
            diceCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success || CurrentDiceRoll.DiceList[0].Side == DieSide.Focus)
            {
                if (Selection.ThisShip.TryDiscardFaceDownDamageCard())
                {
                    Sounds.PlayShipSound("R2D2-Proud");
                    Messages.ShowInfoToHuman("Facedown Damage card is discarded");
                }
            }

            CallBack();
        }

    }

}
