using ActionsList;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class CommanderMalarus : TIEFoFighter
        {
            public CommanderMalarus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Commander Malarus",
                    5,
                    41,
                    charges: 2,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CommanderMalarusAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 120
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/d2/Swz26_a1_malarus.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CommanderMalarusAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            if (IsAbilityAvailable())
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = HostShip.ShipId + ": " + Name,
                    TriggerType = TriggerTypes.OnCombatPhaseStart,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = AskAskToUsePilotAbility
                });
            }
        }

        protected virtual bool IsAbilityAvailable()
        {
            return HostShip.State.Charges > 0;
        }

        private void AskAskToUsePilotAbility(object sender, System.EventArgs e)
        {
            CommanderMalarusDecisionSubPhase newSubPhase = (CommanderMalarusDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Commander Malarus Dicision",
                typeof(CommanderMalarusDecisionSubPhase),
                Triggers.FinishTrigger
            );
            newSubPhase.CommanderMalarusAbility = this;
            newSubPhase.Start();
        }

        public void ActivateCommandeMalarusAbility()
        {
            HostShip.Tokens.AssignCondition(typeof(Conditions.CommanderMalarusCondition));

            Phases.Events.OnEndPhaseStart_NoTriggers += DeactivateCommandeMalarusAbility;

            HostShip.SpendCharge();
            HostShip.Tokens.AssignToken(typeof(StressToken), CommanderMalarusEffect);
        }

        private void CommanderMalarusEffect()
        {
            HostShip.OnGenerateDiceModifications += AddCommanderMalarusEffect;
        }

        private void AddCommanderMalarusEffect(GenericShip host)
        {
            GenericAction newAction = new CommanderMalarusDiceModification()
            {
                ImageUrl = HostShip.ImageUrl,
                HostShip = HostShip
            };
            host.AddAvailableDiceModification(newAction);
        }

        public void DeactivateCommandeMalarusAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddCommanderMalarusEffect;

            Phases.Events.OnEndPhaseStart_NoTriggers -= DeactivateCommandeMalarusAbility;
        }
    }
}

namespace SubPhases
{

    public class CommanderMalarusDecisionSubPhase : DecisionSubPhase
    {
        public Abilities.SecondEdition.CommanderMalarusAbility CommanderMalarusAbility;

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Use ability of Commander Malarus?";
            RequiredPlayer = CommanderMalarusAbility.HostShip.Owner.PlayerNo;

            AddDecision("Yes", UseCommanderMalarusAbility);
            AddDecision("No", DontCommanderMalarusAbility);

            DefaultDecisionName = (IsTimeToUseAbility()) ? "Yes" : "No";

            UI.ShowSkipButton();

            callBack();
        }

        private bool IsTimeToUseAbility()
        {
            bool result = false;

            GenericShip host = CommanderMalarusAbility.HostShip;

            if (host.Owner.UsesHotacAiRules)
            {
                if (!host.Tokens.HasToken(typeof(FocusToken)))
                {
                    int priority = 0;
                    if (ActionsHolder.HasTarget(host)) priority += 20;
                    priority += ActionsHolder.CountEnemiesTargeting(host) * 10;
                    if (host.State.HullCurrent < host.State.HullMax) priority += 5;
                    if (host.Tokens.HasToken(typeof(StressToken))) priority -= 10;

                    if (priority > 10)
                    {
                        Messages.ShowInfo("AI decides to use Commander Malarus ability");
                        result = true;
                    }
                }
            }

            return result;
        }

        private void UseCommanderMalarusAbility(object sender, System.EventArgs e)
        {
            CommanderMalarusAbility.ActivateCommandeMalarusAbility();
            ConfirmDecision();
        }

        private void DontCommanderMalarusAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

    }

}

namespace ActionsList
{

    public class CommanderMalarusDiceModification : GenericAction
    {

        public CommanderMalarusDiceModification()
        {
            Name = DiceModificationName = "Commander Malarus";

            IsTurnsAllFocusIntoSuccess = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            return true;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.CurrentDiceRoll.Focuses > 0)
            {
                if (Combat.AttackStep == CombatStep.Attack)
                {
                    result = 55;
                }
                else if (Combat.AttackStep == CombatStep.Defence)
                {
                    result = 80;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus, DieSide.Success);
            callBack();
        }

    }

}

namespace Conditions
{
    public class CommanderMalarusCondition : GenericToken
    {
        public CommanderMalarusCondition(GenericShip host) : base(host)
        {
            Name = "Buff Token";
            Temporary = true;
            Tooltip = new Ship.SecondEdition.TIEFoFighter.CommanderMalarus().ImageUrl;
        }
    }
}