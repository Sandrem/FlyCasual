using ActionsList;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using Tokens;
using SubPhases;
using System.Linq;
using System;

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
                  true,
                  null,
                  HostUpgrade.Name,
                  "Choose ship at range 1 of the target that isn't jammed.\nIt gets a Jam token.",
                  HostUpgrade.ImageUrl
              );
        }

        private void AssignExtraJamToken()
        {
            TargetShip.Tokens.AssignToken(new JamToken(TargetShip), SelectShipSubPhase.FinishSelection);
        }
        
        private bool IsShipWithoutJamAtRangeOneOfTarget(GenericShip ship)
        {            
            var match = !ship.Tokens.HasToken<JamToken>() && Board.BoardManager.GetRangeOfShips(jamTarget, ship) <= 1;
            return match;
        }

        private int GetAiJamPriority(GenericShip ship)
        {
            int result = 0;

            result += HasTokenPriority(ship);
            result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

            return result;
        }

        private int HasTokenPriority(GenericShip ship)
        {
            if (ship.Tokens.HasToken(typeof(Tokens.FocusToken))) return 100;
            if (ship.PrintedActions.Any(n => n.GetType() == typeof(ActionsList.EvadeAction)) || ship.Tokens.HasToken(typeof(Tokens.EvadeToken))) return 50;
            if (ship.PrintedActions.Any(n => n.GetType() == typeof(ActionsList.TargetLockAction)) || ship.Tokens.HasToken(typeof(Tokens.BlueTargetLockToken), '*')) return 50;
            return 0;
        }
    }
}
