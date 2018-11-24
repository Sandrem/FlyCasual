using System.Collections;
using System.Collections.Generic;
using Abilities;
using ActionsList;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class LeevanTenza : XWing
        {
            public LeevanTenza() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Leevan Tenza",
                    5,
                    25,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.LeevanTenzaAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Partisan";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class LeevanTenzaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckLeevanTenzaAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckLeevanTenzaAbility;
        }

        private void CheckLeevanTenzaAbility(GenericAction action)
        {
            if (action is BoostAction || action is BarrelRollAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToUseLeevanTenzaAbility);
            }
        }

        private void AskToUseLeevanTenzaAbility(object sender, System.EventArgs e)
        {
            HostShip.AskPerformFreeAction(new EvadeAction() { IsRed = true }, Triggers.FinishTrigger);
        }
    }
}
