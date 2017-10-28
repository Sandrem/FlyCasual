using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tokens;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class LieutenantKarsabi : AlphaClassStarWing
        {
            public LieutenantKarsabi() : base()
            {
                PilotName = "Lieutenant Karsabi";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/Alpha-class%20Star%20Wing/lieutenant-karsabi.png";
                PilotSkill = 5;
                Cost = 24;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                OnTokenIsAssigned += RegisterLieutenantKarsabiAbility;
            }

            private void RegisterLieutenantKarsabiAbility(GenericShip ship, System.Type tokenType)
            {
                if (tokenType == typeof(WeaponsDisabledToken))
                {
                    Triggers.RegisterTrigger(new Trigger() {
                        Name = "Lieutenant Karsabi's ability",
                        TriggerType = TriggerTypes.OnTokenIsAssigned,
                        TriggerOwner = ship.Owner.PlayerNo,
                        EventHandler = CheckStress,
                        Sender = ship
                    });
                }
            }

            private void CheckStress(object sender, System.EventArgs e)
            {
                if (!(sender as GenericShip).HasToken(typeof(StressToken)))
                {
                    StartLieutenantKarsabiDecisionSubPhase();
                }
                else
                {
                    Triggers.FinishTrigger();
                }
            }

            private void StartLieutenantKarsabiDecisionSubPhase()
            {
                Phases.StartTemporarySubPhase(
                    "Lieutenant Karsabi's ability",
                    typeof(SubPhases.LieutenantKarsabiDecisionSubphase),
                    Triggers.FinishTrigger
                );
            }
        }
    }
}

namespace SubPhases
{

    public class LieutenantKarsabiDecisionSubphase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Use Lieutenant Karsabi's ability?";

            AddDecision("Yes", UseAbility);
            AddDecision("No", DontUseAbility);

            defaultDecision = "Yes";
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            if (Selection.ThisShip.HasToken(typeof(WeaponsDisabledToken))) Selection.ThisShip.RemoveToken(typeof(WeaponsDisabledToken));
            Selection.ThisShip.AssignToken(new StressToken(), ConfirmDecision);
        }

        private void DontUseAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}

