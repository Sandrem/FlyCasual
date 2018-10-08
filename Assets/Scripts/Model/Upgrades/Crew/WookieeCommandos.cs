using Upgrade;
using Ship;
using System.Linq;
using Tokens;
using System.Collections.Generic;
using UnityEngine;
using SquadBuilderNS;
using System;
using Abilities;

namespace UpgradesList
{
    public class WookieeCommandos : GenericUpgrade
    {
        public WookieeCommandos() : base()
        {
            Types.Add(UpgradeType.Crew);
            Types.Add(UpgradeType.Crew);
            Name = "Wookiee Commandos";
            Cost = 1;

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(23, 0));

            UpgradeAbilities.Add(new WookieeCommandosCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class WookieeCommandosCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += WookieeCommandosDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= WookieeCommandosDiceModification;
        }

        private void WookieeCommandosDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.WookieeCommandosDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            };
            HostShip.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{

    public class WookieeCommandosDiceModification : GenericAction
    {

        public WookieeCommandosDiceModification()
        {
            Name = DiceModificationName = "Wookiee Commandos";

            IsReroll = true;            
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack;            
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;                

                if (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess || n.IsTurnsOneFocusIntoSuccess) == 0)
                {
                    result = 95;
                }                
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                SidesCanBeRerolled = new List<DieSide> { DieSide.Focus },
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}
