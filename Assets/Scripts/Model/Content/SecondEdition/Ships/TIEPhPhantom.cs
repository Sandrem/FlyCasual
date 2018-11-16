using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;
using Ship;
using Tokens;

namespace Ship
{
    namespace SecondEdition.TIEPhPhantom
    {
        public class TIEPhPhantom : FirstEdition.TIEPhantom.TIEPhantom, TIE
        {
            public TIEPhPhantom() : base()
            {
                ShipInfo.ShipName = "TIE/ph Phantom";

                ShipInfo.ArcInfo.Firepower = 3;
                ShipInfo.Hull = 3;

                ShipAbilities.Add(new Abilities.SecondEdition.StygiumArray());

                IconicPilots[Faction.Imperial] = typeof(Whisper);

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class StygiumArray : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDecloak += RegisterPerformFreeEvadeAction;
            Phases.Events.OnEndPhaseStart_Triggers += RegisterCloakAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDecloak -= RegisterPerformFreeEvadeAction;
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterCloakAbility;
        }

        private void RegisterPerformFreeEvadeAction()
        {
            RegisterAbilityTrigger(TriggerTypes.OnDecloak, ProposeFreeEvadeAction);
        }

        private void ProposeFreeEvadeAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Stygium Array: You may perform an Evade action");
            HostShip.AskPerformFreeAction(new EvadeAction() { Host = HostShip }, Triggers.FinishTrigger);
        }

        private void RegisterCloakAbility()
        {
            if (HostShip.Tokens.HasToken<EvadeToken>() && !(HostShip.Tokens.HasToken<CloakToken>()))
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskToCloak);
            }
        }

        private void AskToCloak(object sender, System.EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, TradeEvadeForCloakToken, infoText: "Spend Evade Token to gain Cloak Token?");
        }

        private void TradeEvadeForCloakToken(object sender, System.EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            if (HostShip.Tokens.HasToken<EvadeToken>())
            {
                HostShip.Tokens.RemoveToken(typeof(EvadeToken), AssignCloakToken);
            }
            else
            {
                Messages.ShowError("Ship doesn't have Evade token to spend!");
                Triggers.FinishTrigger();
            }
        }

        private void AssignCloakToken()
        {
            HostShip.Tokens.AssignToken(typeof(CloakToken), Triggers.FinishTrigger);
        }
    }
}
