using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class R2F2 : GenericUpgrade
    {

        public R2F2() : base()
        {
            Type = UpgradeSlot.Astromech;
            Name = ShortName = "R2-F2";
            ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/8/86/R2-F2.jpg";
            isUnique = true;
            Cost = 3;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += R2F2AddAction;
        }

        private void R2F2AddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction action = new ActionsList.R2F2Action();
            action.ImageUrl = ImageUrl;
            host.AddAvailableAction(action);
        }

    }

}

namespace ActionsList
{ 

    public class R2F2Action : GenericAction
    {
        private Ship.GenericShip host;

        public R2F2Action()
        {
            Name = EffectName = "R2-F2: Increase Agility";
        }

        public override void ActionTake()
        {
            Sounds.PlaySoundOnce("Astromech-Beeping-and-whistling");

            host = Selection.ThisShip;
            host.ChangeAgilityBy(+1);
            host.AssignToken(new Conditions.R2F2Condition());
            Phases.OnEndPhaseStart += R2F2DecreaseAgility;
            Phases.CurrentSubPhase.callBack();
        }

        private void R2F2DecreaseAgility()
        {
            host.ChangeAgilityBy(-1);
            host.RemoveToken(typeof(Conditions.R2F2Condition));
            Phases.OnEndPhaseStart -= R2F2DecreaseAgility;
        }

    }

}

namespace Conditions
{

    public class R2F2Condition : Tokens.GenericToken
    {
        public R2F2Condition()
        {
            Name = "Buff Token";
            Temporary = false;
            Tooltip = new UpgradesList.R2F2().ImageUrl;
        }
    }

}
