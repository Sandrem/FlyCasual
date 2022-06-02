using ActionsList;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Commander Malarus",
                    "First Order Enforcer",
                    Faction.FirstOrder,
                    5,
                    3,
                    5,
                    charges: 2,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CommanderMalarusAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
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
            host.AddAvailableDiceModificationOwn(newAction);
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
            DescriptionShort = "Commander Malarus";
            DescriptionLong = "Do you want to spend 1 Charge and gain 1 Stress Token to activate ability?";
            ImageSource = CommanderMalarusAbility.HostShip;

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
                        Messages.ShowInfo("The AI decides to use Commander Malarus' ability");
                        result = true;
                    }
                }
            }

            return result;
        }

        private void UseCommanderMalarusAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            CommanderMalarusAbility.ActivateCommandeMalarusAbility();
            Triggers.FinishTrigger();
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
        public override string Name => HostShip.PilotInfo.PilotName;
        public override string DiceModificationName => HostShip.PilotInfo.PilotName;
        public override string ImageUrl => HostShip.ImageUrl;

        public CommanderMalarusDiceModification()
        {
            IsTurnsAllFocusIntoSuccess = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.CurrentDiceRoll.Focuses != 0;
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
            Name = ImageName = "Buff Token";
            Temporary = true;
            Tooltip = new Ship.SecondEdition.TIEFoFighter.CommanderMalarus().ImageUrl;
        }
    }
}