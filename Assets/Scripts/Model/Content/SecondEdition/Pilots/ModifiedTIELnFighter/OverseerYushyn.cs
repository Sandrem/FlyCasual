using Ship;
using SubPhases;
using System;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class OverseerYushyn : ModifiedTIELnFighter
        {
            public OverseerYushyn() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Overseer Yushyn",
                    2,
                    26,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.OverseerYushynAbility),
                    charges: 1,
                    regensCharges: true
                );
                
                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c1/78/c178c035-befe-44dd-bdb6-541c377ed85b/swz23_overseer-yushyn.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //Before a friendly ship at range 1 would gain a disarm token, if that ship is not stressed, you may spend 1 charge. 
    //If you do, that ship gains 1 stress token instead.
    public class OverseerYushynAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            GenericShip.BeforeTokenIsAssignedGlobal += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.BeforeTokenIsAssignedGlobal -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, Type tokenType)
        {
            if (tokenType == typeof(Tokens.WeaponsDisabledToken) && ship.Owner == HostShip.Owner && !ship.IsStressed && ship != HostShip && HostShip.State.Charges > 0)
            {
                BoardTools.DistanceInfo positionInfo = new BoardTools.DistanceInfo(ship, HostShip);
                if (positionInfo.Range == 1)
                {
                    TargetShip = ship;
                    RegisterAbilityTrigger(TriggerTypes.OnBeforeTokenIsAssigned, ShowDecision);
                }
            }
        }

        private void ShowDecision(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, UseAbility);
        }

        private void UseAbility(object sender, EventArgs e)
        {
            HostShip.SpendCharge();
            TargetShip.Tokens.TokenToAssign = new Tokens.StressToken(TargetShip);
            DecisionSubPhase.ConfirmDecision();
        }
    }
}