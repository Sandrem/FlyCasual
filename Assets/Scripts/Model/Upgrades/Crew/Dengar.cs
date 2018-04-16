using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ship;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class Dengar : GenericUpgrade
    {

        public Dengar() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Dengar";
            Cost = 3;

            isUnique = true;

            AvatarOffset = new Vector2(16, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += PredatorActionEffect;
        }

        private void PredatorActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.DengarDiceModification();
            newAction.ImageUrl = ImageUrl;
            host.AddAvailableActionEffect(newAction);
        }

    }
}

namespace ActionsList
{

    public class DengarDiceModification : GenericAction
    {

        public DengarDiceModification()
        {
            Name = EffectName = "Dengar";

            // Used for abilities like Dark Curse's that can prevent rerolls
            IsReroll = true;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                //if (Combat.Attacker.HasToken(typeof(Tokens.FocusToken)))
                if (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0 )
                {
                    if (attackBlanks > 0) result = 90;
                }
                else
                {
                    if (attackBlanks + attackFocuses > 0) result = 90;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = (Combat.Defender.IsUnique) ? 2 : 1,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}

