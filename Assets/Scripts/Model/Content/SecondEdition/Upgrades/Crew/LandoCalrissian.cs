using Ship;
using SubPhases;
using System;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class LandoCalrissian : GenericUpgrade
    {
        public LandoCalrissian() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Lando Calrissian",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.LandoCalrissianCrewAbility),
                seImageNumber: 87
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //Action: Roll 2 defense dice. For each focus result, gain 1 focus token. For each evade result, gain 1 evade token. 
    //If both results are blank, the opposing player chooses focus or evade. You gain 1 token of that type.
    public class LandoCalrissianCrewAbility : FirstEdition.LandoCalrissianCrewAbility
    {        
        protected override void DiceCheckFinished()
        {
            if (DiceCheckRoll.Blanks == 2)
            {
                var phase = Phases.StartTemporarySubPhaseNew<LandoDecisionSubphase>(
                "Lando Calrissian: Chose token to gain",
                AbilityDiceCheck.ConfirmCheck);
                phase.HostShip = HostShip;
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

            public override void PrepareDecision(Action callBack)
            {
                InfoText = "Lando Calrissian: Chose token to gain";

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
