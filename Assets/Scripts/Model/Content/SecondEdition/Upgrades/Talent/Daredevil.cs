using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using System.Linq;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class Daredevil : GenericUpgrade
    {
        public Daredevil() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Daredevil",
                UpgradeType.Talent,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.DareDevilAbility),
                restrictions: new UpgradeCardRestrictions(
                    new BaseSizeRestriction(BaseSize.Small),
                    new ActionBarRestriction(typeof(BoostAction), ActionColor.White)
                ),
                seImageNumber: 2
            );
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