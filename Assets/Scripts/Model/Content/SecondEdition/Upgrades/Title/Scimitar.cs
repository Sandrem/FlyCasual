using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Scimitar : GenericUpgrade
    {
        public Scimitar() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Scimitar",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                addActions: new List<ActionInfo>()
                {
                    new ActionInfo(typeof(CloakAction), ActionColor.Red),
                    new ActionInfo(typeof(JamAction))
                },
                restrictions: new UpgradeCardRestrictions
                (
                    new FactionRestriction(Faction.Separatists),
                    new ShipRestriction(typeof(Ship.SecondEdition.SithInfiltrator.SithInfiltrator))
                ),
                abilityType: typeof(Abilities.SecondEdition.ScimitarAbility)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ScimitarAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterAskToCloakAbility;
            HostShip.OnDecloak += RegisterAskToChooseOpponent;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterAskToCloakAbility;
            HostShip.OnDecloak -= RegisterAskToChooseOpponent;
        }

        private void RegisterAskToChooseOpponent()
        {
            RegisterAbilityTrigger(TriggerTypes.OnDecloak, StartSelectShipSubphase);
        }

        private void StartSelectShipSubphase(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                AssignJamToken,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                "You may assign Jam Token to a ship in your bullseye arc",
                imageSource: HostUpgrade
            );
        }

        private void AssignJamToken()
        {
            SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();

            Messages.ShowInfo("Scimitar: Jam Token is assigned to " + TargetShip.PilotInfo.PilotName);

            TargetShip.Tokens.AssignToken(
                new Tokens.JamToken(TargetShip, HostShip.Owner),
                Triggers.FinishTrigger
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            return ship.Owner.PlayerNo != HostShip.Owner.PlayerNo
                && HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Bullseye);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ((ship.Tokens.HasGreenTokens || ship.Tokens.HasTokenByColor(Tokens.TokenColors.Blue)) ? 100 : 0) + ship.PilotInfo.Cost; 
        }

        private void RegisterAskToCloakAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnSetupEnd, AskToCloak);
        }

        private void AskToCloak(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                DoCloak,
                descriptionLong: "Do you want to cloak?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void DoCloak(object sender, EventArgs e)
        {
            new CloakAction().DoOnlyEffect(SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}