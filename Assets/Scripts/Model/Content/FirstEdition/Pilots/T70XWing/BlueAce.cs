using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.T70XWing
    {
        public class BlueAce : T70XWing
        {
            public BlueAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Blue Ace\"",
                    5,
                    27,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.BlueAceAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class BlueAceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates += ChangeBoostTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates -= ChangeBoostTemplate;
        }

        private void ChangeBoostTemplate(List<ActionsList.BoostMove> availableMoves)
        {
            availableMoves.Add(new ActionsList.BoostMove(ActionsHolder.BoostTemplates.LeftTurn1));
            availableMoves.Add(new ActionsList.BoostMove(ActionsHolder.BoostTemplates.RightTurn1));
        }
    }
}