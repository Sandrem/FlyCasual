using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class ExpertHandling : GenericUpgrade
    {

        public ExpertHandling() : base()
        {
            IsHidden = true;

            Type = UpgradeSlot.Elite;
            Name = "Expert Handling";
            ShortName = "Exp. Handling";
            ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/1/11/Expert-handling.png";
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += AddExpertHandlingAction;
        }

        private void AddExpertHandlingAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.ExpertHandlingAction();
            newAction.ImageUrl = ImageUrl;
            host.AddAvailableAction(newAction);
        }

    }
}

namespace ActionsList
{

    public class ExpertHandlingAction : GenericAction
    {
        private Ship.GenericShip host;

        public ExpertHandlingAction()
        {
            Name = EffectName = "Expert Handling";
        }

        public override void ActionTake()
        {
            host = Selection.ThisShip;
            //TODO:
            // Start barrel roll
            GenericAction action = new BarrelRollAction();
            action.ActionTake();
            // On success remove tl (selection?)
            host.RemoveToken(typeof(Tokens.RedTargetLockToken), '*');
            // On success if cannot Barrel Roll - add stress
            bool hasBuiltInAction = false;
            foreach (var builtInAction in host.GetActionsFromActionBar())
            {
                if (builtInAction.Name == action.Name)
                {
                    hasBuiltInAction = true;
                    break;
                }
            }

            if (!hasBuiltInAction)
            {
                host.AssignToken(new Tokens.StressToken());
            }
            // !!!  A ship equipped with Expert Handling cannot perform a barrel roll and use the Expert Handling action in the same round.
            //Phases.Next();
        }

    }

}
