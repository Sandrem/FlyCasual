using System;
using Ship;
using Upgrade;
using Abilities;
using Tokens;

namespace UpgradesList
{
	public class MultiSpectralCamouflage : GenericUpgrade
    {
		public MultiSpectralCamouflage() : base()
        {
            Types.Add(UpgradeType.Modification);
			Name = "Multi-spectral Camouflage";
			Cost = 1;

            UpgradeAbilities.Add(new MultiSpectralCamouflageAbility());
		}
    }
}

namespace Abilities
{
    public class MultiSpectralCamouflageAbility : GenericAbility
    {

        GenericShip TargetingShip;
        char TargetLockLetter;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += RegisterMultiSpectralCamouflageAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += RegisterMultiSpectralCamouflageAbility;
        }

        private void RegisterMultiSpectralCamouflageAbility(GenericShip ship, Type tokenType)
        {
            // Trigger only when a ship receives a BlueTargetLockToken
            if (tokenType != typeof(BlueTargetLockToken)) return;

            BlueTargetLockToken assignedBlueLock = (BlueTargetLockToken)ship.Tokens.GetToken(typeof(BlueTargetLockToken), '*');
            TargetLockLetter = assignedBlueLock.Letter;

            // Make sure the targeted ship is the HostShip
            if (HostShip.Tokens.GetToken(typeof(RedTargetLockToken), TargetLockLetter) == null) return;
         
            // Make sure Host Ship only has one red target lock
            int redTargetLockCount = 0;
            foreach (GenericToken token in HostShip.Tokens.GetAllTokens())
            {
                if (token is RedTargetLockToken)
                {
                    redTargetLockCount += 1;
                }
            }

            TargetingShip = ship;

            if (redTargetLockCount == 1)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Multi-spectral Camouflage",
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnTokenIsAssigned,
                    EventHandler = RollForMultiSpectralCamouflage
                });
            }
        }
        

        private void RollForMultiSpectralCamouflage(object sender, EventArgs e)
        {
            
            Selection.ActiveShip = HostShip;
            Selection.ThisShip = HostShip;

            var subphase = Phases.StartTemporarySubPhaseNew<SubPhases.MultiSpectralCamouflageCheckSubPhase>("Multi-spectral Camouflage", () =>
            {
                Phases.FinishSubPhase(typeof(SubPhases.MultiSpectralCamouflageCheckSubPhase));
                Triggers.FinishTrigger();
                Selection.ActiveShip = TargetingShip;
                Selection.ThisShip = TargetingShip;
            });
            subphase.HostShip = HostShip;
            subphase.TargetingShip = TargetingShip;
            subphase.TargetLockLetter = TargetLockLetter;
            Messages.ShowInfoToHuman("Multi-spectral Camouflage");
            subphase.Start();
        }
    }
}

namespace SubPhases
{

    public class MultiSpectralCamouflageCheckSubPhase : DiceRollCheckSubPhase
    {
        public GenericShip HostShip;
        public GenericShip TargetingShip;
        public char TargetLockLetter;

        public override void Prepare()
        {
            DiceKind = DiceKind.Defence;
            DiceCount = 1;
            AfterRoll = FinishAction;
            Name = "Multi-spectral Camouflage";
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();
            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success)
            {
                HostShip.Tokens.RemoveToken(
                    typeof(RedTargetLockToken), 
                    delegate {
                        TargetingShip.Tokens.RemoveToken(
                            typeof(BlueTargetLockToken),
                            CallBack,
                            TargetLockLetter
                        );
                    }, 
                    TargetLockLetter
                );
            } else {
                CallBack();
            }
        }
    }

}