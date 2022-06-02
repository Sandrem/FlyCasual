using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class Ibtisam : ARC170Starfighter
        {
            public Ibtisam() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Ibtisam",
                    "Survivor of Endor",
                    Faction.Rebel,
                    3,
                    5,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.IbtisamAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    seImageNumber: 68
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IbtisamAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterBraylenStrammPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterBraylenStrammPilotAbility;
        }

        private void RegisterBraylenStrammPilotAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskBraylenStrammAbility);
        }

        private void AskBraylenStrammAbility(object sender, System.EventArgs e)
        {
            if (HostShip.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseBraylenStrammAbility,
                    DontUseBraylenStrammAbility,
                    descriptionLong: "Do you want to roll 1 attack die? (On a \"hit\" or \"crit\" result, remove 1 stress token)",
                    imageHolder: HostShip
                );
            }
            else
            {
                //No decision subphase or BraylenStrammCheckSubPhase initiated - simply finish trigger
                Triggers.FinishTrigger();
            }
        }

        private void UseBraylenStrammAbility(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld(
                HostShip.PilotInfo.PilotName + ": Try to remove stress",
                typeof(SubPhases.BraylenStrammCheckSubPhase),
                delegate {
                    //We have a BraylenStrammCheckSubPhase open, so finish it
                    Phases.FinishSubPhase(typeof(SubPhases.BraylenStrammCheckSubPhase));

                    //We have a Decision SubPhase open, so finish it
                    SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

                    //The trigger is still active, so finish it.  Must be explicitly finished since ConfirmDecisionNoCallback was used
                    Triggers.FinishTrigger();
                }
            );
        }

        private void DontUseBraylenStrammAbility(object sender, System.EventArgs e)
        {
            //We have only a Decision SubPhase open, so finish it
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            //The trigger is still active, so finish it.  Must be explicitly finished since ConfirmDecisionNoCallback was used
            Triggers.FinishTrigger();
        }
    }
}

namespace SubPhases
{
    public class BraylenStrammCheckSubPhase : DiceRollCheckSubPhase
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

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success || CurrentDiceRoll.DiceList[0].Side == DieSide.Crit)
            {
                Messages.ShowInfoToHuman("Stress is removed");
                this.TheShip.Tokens.RemoveToken(
                    typeof(Tokens.StressToken),
                    CallBack
                );
            }
            else
            {
                CallBack();
            }
        }
    }
}