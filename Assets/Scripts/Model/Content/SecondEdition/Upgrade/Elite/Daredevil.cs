using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class Daredevil : GenericUpgrade
    {
        public Daredevil() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Daredevil",
                UpgradeType.Elite,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.DareDevilAbility),
                seImageNumber: 2
            );
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipInfo.BaseSize == BaseSize.Small && ship.ActionBar.HasAction(typeof(BoostAction), isRed: false);
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a white boost action, you may treat it as red to use the 1 left turn or 1 right turn template instead.
    public class DareDevilAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates += ChangeBoostTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates -= ChangeBoostTemplates;
        }

        private void ChangeBoostTemplates(List<BoostMove> availableMoves)
        {
            availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.LeftTurn1, true));
            availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.RightTurn1, true));
        }
    }
}