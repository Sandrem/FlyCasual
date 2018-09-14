using Abilities;
using RuleSets;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using UnityEngine.UI;
using Upgrade;

namespace UpgradesList
{
    public class Zuckuss : GenericUpgrade, ISecondEditionUpgrade
    {
        public Zuckuss() : base()
        {
            Name = "Zuckuss";
            Cost = 1;

            Types.Add(UpgradeType.Crew);

            isUnique = true;

            IsHidden = true;

            AvatarOffset = new Vector2(79, 1);
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 3;

            IsHidden = false;

            UpgradeAbilities.Add(new Abilities.SecondEdition.ZuckussCrewAbility());

            SEImageNumber = 138;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ZuckussCrewAbility : GenericAbility
    {
        //While you perform an attack, if you are not stressed, you may choose 1 defense die and gain 1 stress token. If you do, the defender must reroll that die.

        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite += ZuckussAbilityEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite -= ZuckussAbilityEffect;
        }

        private void ZuckussAbilityEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.ZuckussActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{
    public class ZuckussActionEffect : GenericAction
    {

        public ZuckussActionEffect()
        {
            Name = DiceModificationName = "Zuckuss Ability";
            DiceModificationTiming = DiceModificationTimingType.Opposite;
        }

        public override int GetDiceModificationPriority()
        {
            return 80;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && !Host.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                IsOpposite = true,
                CallBack = delegate {
                    AssignStress(callBack);
                }
            };
            diceRerollManager.Start();
        }

        private void AssignStress(System.Action callBack)
        {
            Host.Tokens.AssignToken(typeof(StressToken), callBack);
        }
    }
}