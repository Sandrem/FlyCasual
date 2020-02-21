using ActionsList;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AWing
    {
        public class TychoCelchu : AWing
        {
            public TychoCelchu() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tycho Celchu",
                    8,
                    26,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TychoCelchuAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class TychoCelchuAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckCanPerformActionsWhileStressed += ConfirmThatIsPossible;
            HostShip.OnCanPerformActionWhileStressed += AlwaysAllow;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCanPerformActionWhileStressed -= AlwaysAllow;
            HostShip.OnCheckCanPerformActionsWhileStressed -= ConfirmThatIsPossible;
        }

        private void ConfirmThatIsPossible(ref bool isAllowed)
        {
            AlwaysAllow(null, ref isAllowed);
        }

        private void AlwaysAllow(GenericAction action, ref bool isAllowed)
        {
            isAllowed = true;
        }
    }
}