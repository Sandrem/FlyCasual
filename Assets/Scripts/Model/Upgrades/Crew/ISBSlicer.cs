using ActionsList;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using Tokens;

namespace UpgradesList
{
    public class ISBSlicer : GenericUpgrade
    {
        public ISBSlicer() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "ISB Slicer";
            Cost = 2;

            UpgradeAbilities.Add(new ISBSlicerAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}

namespace Abilities
{
    public class ISBSlicerAbility : GenericAbility
    {
        GenericShip jamTarget = null;

        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
            HostShip.OnJamTargetIsSelected += NoteDownJamTarget;
        }
                
        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
            HostShip.OnJamTargetIsSelected -= NoteDownJamTarget;
        }

        private void NoteDownJamTarget(GenericShip ship)
        {
            jamTarget = ship;
        }

        private void CheckConditions(GenericAction action)
        {            
            if (action is JamAction && jamTarget != null)
            {
                HostShip.OnActionDecisionSubphaseEnd += RegisterTrigger;
            }
        }

        private void RegisterTrigger(Ship.GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= RegisterTrigger;
            RegisterAbilityTrigger(TriggerTypes.OnActionDecisionSubPhaseEnd, DoISBSlicerEffect);            
        }

        private void DoISBSlicerEffect(object sender, System.EventArgs e)
        {
            if (jamTarget.Tokens.CountTokensByType<JamToken>() == 0)
            {
                jamTarget.Tokens.AssignToken(new JamToken(jamTarget), Triggers.FinishTrigger);
            }
        }
    }
}
