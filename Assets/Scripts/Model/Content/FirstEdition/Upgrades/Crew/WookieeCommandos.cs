using Ship;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;
using ActionsList;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class WookieeCommandos : GenericUpgrade
    {
        public WookieeCommandos() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Wookiee Commandos",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew
                },
                cost: 1,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.WookieeCommandosCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(23, 0));
        }        
    }
}

namespace Abilities.FirstEdition
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
            GenericAction newAction = new WookieeCommandosDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip
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