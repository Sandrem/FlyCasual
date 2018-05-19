using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using Abilities;
using ActionsList;
using Ship;
using RuleSets;

namespace UpgradesList
{

    public class Predator : GenericUpgrade, ISecondEditionUpgrade
    {
        public Predator() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Predator";
            Cost = 3;

            UpgradeAbilities.Add(new PredatorAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            ImageUrl = "https://i.imgur.com/ahiBajk.png";
            UpgradeAbilities.RemoveAll(a => a is PredatorAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.PredatorAbility());
        }
    }
}

namespace Abilities
{
    public class PredatorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += PredatorActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= PredatorActionEffect;
        }

        private void PredatorActionEffect(GenericShip host)
        {
            GenericAction newAction = new PredatorActionEffect
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableActionEffect(newAction);
        }
    }
}

namespace ActionsList
{

    public class PredatorActionEffect : GenericAction
    {

        public PredatorActionEffect()
        {
            Name = EffectName = "Predator";

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
                NumberOfDiceCanBeRerolled = (Combat.Defender.PilotSkill > 2) ? 1 : 2,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}



namespace Abilities.SecondEdition
{
    //While you perform a primary attack, if the defender is in your bullseye firing arc, you may reroll 1 attack die.
    public class PredatorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += PredatorActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= PredatorActionEffect;
        }

        private void PredatorActionEffect(GenericShip host)
        {
            GenericAction newAction = new ActionsList.SecondEdition.PredatorActionEffect
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableActionEffect(newAction);
        }
    }
}


namespace ActionsList.SecondEdition
{
    //While you perform a primary attack, if the defender is in your bullseye firing arc, you may reroll 1 attack die.
    public class PredatorActionEffect : ActionsList.PredatorActionEffect
    {

        public PredatorActionEffect() : base()
        {
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack && Combat.Attacker == Host && Combat.ChosenWeapon is PrimaryWeaponClass && Combat.ShotInfo.InArcByType(Arcs.ArcTypes.Bullseye)) result = true;
            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}

