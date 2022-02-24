using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YV666LightFreighter
    {
        public class Bossk : YV666LightFreighter
        {
            public Bossk() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Bossk",
                    "Fearsome Hunter",
                    Faction.Scum,
                    4,
                    7,
                    22,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BosskPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.BountyHunter
                    },
                    seImageNumber: 210
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BosskPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterBosskPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterBosskPilotAbility;
        }

        protected virtual void RegisterBosskPilotAbility()
        {
            if (Combat.DiceRollAttack.CriticalSuccesses > 0 && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                Phases.CurrentSubPhase.Pause();
                RegisterAbilityTrigger(TriggerTypes.OnShotHit, PromptToChangeCritSuccess);
            }
        }

        private void PromptToChangeCritSuccess(object sender, EventArgs e)
        {
            DecisionSubPhase decisionSubPhase = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(DecisionSubPhase),
                Triggers.FinishTrigger);

            decisionSubPhase.DescriptionShort = HostShip.PilotInfo.PilotName;
            decisionSubPhase.DescriptionLong = "Would you like to cancel 1 critical result to add 2 success results?";
            decisionSubPhase.ImageSource = HostShip;

            decisionSubPhase.AddDecision("Yes", ConvertCriticalsToSuccesses);
            decisionSubPhase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); }
            );

            decisionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
            decisionSubPhase.ShowSkipButton = true;

            decisionSubPhase.DefaultDecisionName = "No";

            decisionSubPhase.Start();
        }

        private void ConvertCriticalsToSuccesses(object sender, EventArgs e)
        {
            Combat.DiceRollAttack.AddDice(DieSide.Success);
            Combat.DiceRollAttack.AddDice(DieSide.Success);

            Combat.DiceRollAttack.DiceList.Remove(
                Combat.DiceRollAttack.DiceList.First(die => die.Side == DieSide.Crit));

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Changed one critical result into two success results");
            DecisionSubPhase.ConfirmDecision();
            Phases.CurrentSubPhase.Resume();
        }
    }
}