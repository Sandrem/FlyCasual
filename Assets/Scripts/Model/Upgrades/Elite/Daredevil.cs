using Upgrade;
using RuleSets;
using Ship;
using ActionsList;

namespace UpgradesList
{
    public class Daredevil : GenericUpgrade, ISecondEditionUpgrade
    {
        private bool isSecondEdition = false;
        public Daredevil() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Daredevil";
            Cost = 3;

            UpgradeRuleType = typeof(SecondEdition);

            IsHidden = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (isSecondEdition)
            {
                return ship.ShipBase.Size == BaseSize.Small && ship.ActionBar.HasAction(typeof(BoostAction), false);
            }
            else return true;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            IsHidden = false;

            Cost = 3;

            UpgradeAbilities.Add(new Abilities.SecondEdition.DareDevilAbility());

            SEImageNumber = 2;
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

        private void ChangeBoostTemplates(System.Collections.Generic.List<BoostMove> availableMoves)
        {
            availableMoves.Add(new BoostMove(Actions.BoostTemplates.LeftTurn1, true));
            availableMoves.Add(new BoostMove(Actions.BoostTemplates.RightTurn1, true));
        }
    }
}
