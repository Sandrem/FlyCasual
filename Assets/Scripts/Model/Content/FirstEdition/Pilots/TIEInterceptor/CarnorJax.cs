using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEInterceptor
    {
        public class CarnorJax : TIEInterceptor
        {
            public CarnorJax() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Carnor Jax",
                    8,
                    26,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.CarnorJaxAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Royal Guard";
            }
        }
    }
}

namespace Abilities.FirstEdition
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
                    Messages.ShowErrorToHuman("Carnor Jax's ability: The target ship cannot perform focus or evade actions, or spend focus or evade tokens.");
                    isAllowed = false;
                }
            }
        }

    }
}