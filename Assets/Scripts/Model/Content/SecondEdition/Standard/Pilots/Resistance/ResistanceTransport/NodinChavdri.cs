using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.SecondEdition;
using Upgrade;
using ActionsList;
using Tokens;

namespace Ship.SecondEdition.ResistanceTransport
{
    public class NodinChavdri : ResistanceTransport
    {
        public NodinChavdri()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Nodin Chavdri",
                "Insubordinate Insurgent",
                Faction.Resistance,
                2,
                5,
                20,
                isLimited: true,
                abilityType: typeof(NodinChavdriGoodeAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Tech,
                    UpgradeType.Cannon,
                    UpgradeType.Torpedo,
                    UpgradeType.Missile,
                    UpgradeType.Modification
                }
            );

            ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/9f41de269cb1ff091487554fb53b2374.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NodinChavdriGoodeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAfterCoordinateAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAfterCoordinateAbility;
        }

        private void CheckAfterCoordinateAbility(GenericAction action)
        {
            if (HasFewStressTokens())
            {
                if (action is CoordinateAction || action.IsCoordinatedAction)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskPerformActionAsRed);
                }
            }            
        }

        private void AskPerformActionAsRed(object sender, EventArgs e)
        {
            List<GenericAction> actions = HostShip.ActionBar.AllActions.Select(n => n.AsRedAction).ToList();
            actions.ForEach(n => n.CanBePerformedWhileStressed = true);

            HostShip.AskPerformFreeAction(
                actions,
                Triggers.FinishTrigger,
                descriptionShort: HostShip.PilotInfo.PilotName,
                descriptionLong: "You may perform 1 action on your action bar as red action",
                imageHolder: HostShip
            );
        }

        private bool HasFewStressTokens()
        {
            return HostShip.Tokens.CountTokensByType(typeof(StressToken)) <= 2;
        }
    }
}