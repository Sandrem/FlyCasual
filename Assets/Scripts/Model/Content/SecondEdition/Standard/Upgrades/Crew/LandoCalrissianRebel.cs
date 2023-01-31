using Ship;
using SubPhases;
using System;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class LandoCalrissianRebelCrew : GenericUpgrade
    {
        public LandoCalrissianRebelCrew() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Lando Calrissian",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.LandoCalrissianRebelAbility),
                seImageNumber: 87
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(379, 8),
                new Vector2(150, 150)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //Action: Roll 2 defense dice. For each focus result, gain 1 focus token. For each evade result, gain 1 evade token. 
    //If both results are blank, the opposing player chooses focus or evade. You gain 1 token of that type.
    public class LandoCalrissianRebelAbility : FirstEdition.LandoCalrissianCrewAbility
    {        
        protected override void DiceCheckFinished()
        {
            if (DiceCheckRoll.Blanks == 2)
            {
                var phase = Phases.StartTemporarySubPhaseNew<LandoDecisionSubphase>(
                "Lando Calrissian: Chose token to gain",
                AbilityDiceCheck.ConfirmCheck);
                phase.HostShip = HostShip;
                phase.SourceUpgrade = HostUpgrade;
                phase.DecisionOwner = HostShip.Owner.AnotherPlayer;
                phase.Start();
            }
            else
            {
                base.DiceCheckFinished();
            }
        }

        protected class LandoDecisionSubphase : DecisionSubPhase
        {
            public GenericShip HostShip;
            public GenericUpgrade SourceUpgrade;

            public override void PrepareDecision(Action callBack)
            {
                DescriptionShort = "Lando Calrissian";
                DescriptionLong = "Choose a token to gain";
                ImageSource = SourceUpgrade;

                DecisionViewType = DecisionViewTypes.TextButtons;

                AddDecision("Focus", delegate { GainToken(typeof(Tokens.FocusToken)); });
                AddDecision("Evade", delegate { GainToken(typeof(Tokens.EvadeToken)); });

                DefaultDecisionName = "Evade";
                ShowSkipButton = false;
                callBack();
            }

            private void GainToken(Type type)
            {
                HostShip.Tokens.AssignToken(type, ConfirmDecision);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    //ACTION: Roll 2 defense dice. For each focus result, assign 1 focus token to your ship. For each evade result, assign 1 evade token to your ship.
    public class LandoCalrissianCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddAction;
        }

        protected void AddAction(Ship.GenericShip ship)
        {
            ship.AddAvailableAction(new ActionsList.GenericAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip,
                Source = HostUpgrade,
                DoAction = DoAction,
                Name = HostName
            });
        }

        protected void DoAction()
        {
            PerformDiceCheck(
                HostName,
                DiceKind.Defence,
                2,
                DiceCheckFinished,
                Phases.CurrentSubPhase.CallBack
            );
        }

        protected virtual void DiceCheckFinished()
        {
            HostShip.Tokens.AssignTokens(() => new Tokens.FocusToken(HostShip), DiceCheckRoll.Focuses, () => {
                HostShip.Tokens.AssignTokens(() => new Tokens.EvadeToken(HostShip), DiceCheckRoll.Successes, () =>
                {
                    AbilityDiceCheck.ConfirmCheck();
                });
            });
        }
    }
}