using Ship;
using Upgrade;
using SubPhases;
using ActionsList;
using System;
using System.Linq;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class ISBSlicer : GenericUpgrade
    {
        public ISBSlicer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "ISB Slicer",
                UpgradeType.Crew,
                cost: 2,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.FirstEdition.ISBSlicerAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
                var anyCandidate = Roster.AllShips.Values
                    .Any(s => IsShipWithoutJamAtRangeOneOfTarget(s));
                if (anyCandidate)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, ISBSlicerEffect);
                }
            }
        }

        private void ISBSlicerEffect(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                  AssignExtraJamToken,
                  IsShipWithoutJamAtRangeOneOfTarget,
                  GetAiJamPriority,
                  HostShip.Owner.PlayerNo,
                  HostUpgrade.UpgradeInfo.Name,
                  "Choose ship at range 1 of the target that isn't jammed.\nIt gets a Jam token.",
                  HostUpgrade
              );
        }

        private void AssignExtraJamToken()
        {
            TargetShip.Tokens.AssignToken(typeof(JamToken), SelectShipSubPhase.FinishSelection);
        }

        private bool IsShipWithoutJamAtRangeOneOfTarget(GenericShip ship)
        {
            var match = !ship.Tokens.HasToken<JamToken>() && BoardTools.Board.GetRangeOfShips(jamTarget, ship) <= 1;
            return match;
        }

        private int GetAiJamPriority(GenericShip ship)
        {
            int result = 0;

            result += HasTokenPriority(ship);
            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private int HasTokenPriority(GenericShip ship)
        {
            if (ship.Tokens.HasToken(typeof(FocusToken))) return 100;
            if (ship.ActionBar.HasAction(typeof(EvadeAction)) || ship.Tokens.HasToken(typeof(EvadeToken))) return 50;
            if (ship.ActionBar.HasAction(typeof(TargetLockAction)) || ship.Tokens.HasToken(typeof(BlueTargetLockToken), '*')) return 50;
            return 0;
        }
    }
}