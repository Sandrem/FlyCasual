using Ship;
using System.Linq;
using Tokens;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class CarnorJax : TIEInterceptor
        {
            public CarnorJax() : base()
            {
                PilotName = "Carnor Jax";
                PilotSkill = 8;
                Cost = 26;
                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PilotAbilities.Add(new Abilities.CarnorJaxAbility());

                SkinName = "Royal Guard";
            }
        }
    }
}

namespace Abilities
{
    public class CarnorJaxAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnTryAddAvailableDiceModificationGlobal += CheckCarnorJaxRestriction;
            GenericShip.OnTryAddActionGlobal += CheckCarnorJaxRestriction;            
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryAddAvailableDiceModificationGlobal -= CheckCarnorJaxRestriction;
            GenericShip.OnTryAddActionGlobal -= CheckCarnorJaxRestriction;
        }

        private void CheckCarnorJaxRestriction(GenericShip ship, ActionsList.GenericAction action, ref bool isAllowed)
        {
            if (ship.Owner != HostShip.Owner && action.TokensSpend.Any(token => token == typeof(FocusToken) || token == typeof(EvadeToken)))
            {
                BoardTools.DistanceInfo positionInfo = new BoardTools.DistanceInfo(ship, HostShip);
                if (positionInfo.Range <= 1)
                {
                    Messages.ShowErrorToHuman("Carnor Jax: Cannot perform focus or evade actions, or spend focus or evade tokens");
                    isAllowed = false;
                }
            }
        }
                
    }
}