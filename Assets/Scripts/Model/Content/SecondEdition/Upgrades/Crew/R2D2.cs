using Ship;
using Upgrade;
using SubPhases;
using System;

namespace UpgradesList.SecondEdition
{
    public class R2D2Crew : GenericUpgrade
    {
        public R2D2Crew() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2-D2 (crew)",
                UpgradeType.Crew,
                cost: 8,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.R2D2CrewAbility),
                seImageNumber: 91
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class R2D2CrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.Damage.IsDamaged() && HostShip.State.ShieldsCurrent == 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                AlwaysUseByDefault,
                UseAbility,
                infoText: "R2-D2: Do you want to recover shield, but have a chance to expose damage card?"
            );
        }

        private void UseAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            if (HostShip.TryRegenShields())
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " used ability of R2-D2, 1 shield is restored");
                Sounds.PlayShipSound("R2D2-Proud");
            }

            Phases.StartTemporarySubPhaseOld(
                "R2-D2: Check expose of damage card",
                typeof(R2D2CheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(R2D2CheckSubPhase));
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}

namespace SubPhases
{

    public class R2D2CheckSubPhase : DiceRollCheckSubPhase
    {
        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success)
            {
                Selection.ThisShip.Damage.ExposeRandomFacedownCard(CallBack);
            }
            else
            {
                CallBack();
            }
        }
    }

}