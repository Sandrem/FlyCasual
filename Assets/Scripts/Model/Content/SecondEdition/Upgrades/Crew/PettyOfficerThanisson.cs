using Ship;
using Upgrade;
using Arcs;
using SubPhases;
//using System.Collections.Generic;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class PettyOfficerThanisson : GenericUpgrade
    {
        public PettyOfficerThanisson() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Petty Officer Thanisson",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.PettyOfficerThanissonCrewAbility)
            );
            
            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/900397d209adfd3f7bd6429909cc20cc.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class PettyOfficerThanissonCrewAbility : GenericAbility
    {
        private GenericShip ShipWithToken;
        private System.Type token;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += PettyOfficerThanissonEffect;
        }
                
        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal -= PettyOfficerThanissonEffect;
        }

        private void PettyOfficerThanissonEffect(GenericShip ship, System.Type tokenType)
        {
            TokenColors tokenColor = TokensManager.GetTokenColorByType(tokenType);

            //During the Activation or Engagement Phase, after an enemy ship in your "arc standard
            //front" at range 0-1 gains a red or orange token, if you are not stressed, you may gain 1 stress 
            //token. If you do, that ship gains 1 additional token of the type that it gained.          
            if ((Phases.CurrentPhase is MainPhases.ActivationPhase || Phases.CurrentPhase is MainPhases.CombatPhase) 
                && (tokenColor == TokenColors.Red || tokenColor == TokenColors.Orange)
                && tokenType != typeof(RedTargetLockToken)
                && !HostShip.Tokens.HasToken(typeof(StressToken))
                && ship.Owner.PlayerNo != HostShip.Owner.PlayerNo
                && HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Front)
                && HostShip.SectorsInfo.RangeToShipBySector(ship, ArcType.Front) <= 1)
            {
                ShipWithToken = ship;
                token = tokenType;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, ShowDecision);
            }
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            if(!HostShip.Tokens.HasToken(typeof(StressToken))){
                AskToUseAbility(
                    ShouldUseAbility,
                    UseAbility,
                    infoText: HostShip.PilotInfo.PilotName + ": Gain a Stress Token to assign an additional " + token.Name + " to " + ShipWithToken.PilotInfo.PilotName + "?" 
                );
            } 
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool ShouldUseAbility()
        {
            return ShipWithToken.Owner.PlayerNo != HostShip.Owner.PlayerNo;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
         
            HostShip.Tokens.AssignToken(
                typeof(StressToken), delegate { AssignExtraToken(token); }
            );
        }

        private void AssignExtraToken(System.Type tokenToAssign)
        {
            if(token == typeof(TractorBeamToken))
            {
                GenericToken tractorToken = new Tokens.TractorBeamToken(ShipWithToken, HostShip.Owner);
                ShipWithToken.Tokens.AssignToken(tractorToken, Triggers.FinishTrigger);
            } 
            else 
            {
                ShipWithToken.Tokens.AssignToken(tokenToAssign, Triggers.FinishTrigger);
            }
            
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Gained a Stress Token to assign an additional " + tokenToAssign.Name 
                + " to " + ShipWithToken.PilotInfo.PilotName + ".");
        }
    }
}