using ActionsList;
using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class TychoCelchu : RZ1AWing
        {
            public TychoCelchu() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Tycho Celchu",
                    "Son of Alderaan",
                    Faction.Rebel,
                    5,
                    4,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TychoCelchuAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/790214c2-924a-4066-894a-ac71d59cc82b/SWZ97_TychoCelchulegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TychoCelchuAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckCanPerformActionsWhileStressed += ConfirmThatIsPossible;
            HostShip.OnCanPerformActionWhileStressed += CheckTwoOrFewerStress;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCanPerformActionWhileStressed -= CheckTwoOrFewerStress;
            HostShip.OnCheckCanPerformActionsWhileStressed -= ConfirmThatIsPossible;
        }


        private void ConfirmThatIsPossible(ref bool isAllowed)
        {
            isAllowed = (HostShip.Tokens.CountTokensByType<Tokens.StressToken>() <= 2);
        }

        private void CheckTwoOrFewerStress(GenericAction action, ref bool isAllowed)
        {
            isAllowed = (HostShip.Tokens.CountTokensByType<Tokens.StressToken>() <= 2);
        }
    }
}