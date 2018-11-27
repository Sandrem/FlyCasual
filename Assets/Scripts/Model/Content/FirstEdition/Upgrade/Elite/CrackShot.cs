using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class CrackShot : GenericUpgrade
    {
        public CrackShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Crack Shot",
                UpgradeType.Elite,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.CrackShotAbility)
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(43, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class CrackShotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults += CrackShotDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults -= CrackShotDiceModification;
        }

        private void CrackShotDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.CrackShotDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host,
                Source = this.HostUpgrade
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{
    public class CrackShotDiceModification : GenericAction
    {

        public CrackShotDiceModification()
        {
            Name = DiceModificationName = "Crack Shot";
            DiceModificationTiming = DiceModificationTimingType.CompareResults;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.DiceRollDefence.Successes <= Combat.DiceRollAttack.Successes) result = 100;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.DiceRollDefence.Successes > 0 && Combat.ShotInfo.InArc)
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollDefence.ChangeOne(DieSide.Success, DieSide.Blank, false);
            Source.TryDiscard(callBack);
        }

    }

}