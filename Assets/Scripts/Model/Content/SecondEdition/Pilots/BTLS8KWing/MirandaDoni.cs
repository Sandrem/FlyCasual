using SubPhases;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.BTLS8KWing
    {
        public class MirandaDoni : BTLS8KWing
        {
            public MirandaDoni() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Miranda Doni",
                    4,
                    42,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MirandaDoniAbility),
                    seImageNumber: 62
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MirandaDoniAbility : Abilities.FirstEdition.MirandaDoniAbility
    {
        protected override void CheckConditions()
        {
            if (Combat.ChosenWeapon.WeaponType == Ship.WeaponTypes.PrimaryWeapon)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotStart, StartQuestionSubphase);
            }
        }

        protected override void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            MirandaDoniDecisionSubPhase selectMirandaDoniSubPhase = (MirandaDoniDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(MirandaDoniDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectMirandaDoniSubPhase.DescriptionShort = HostShip.PilotInfo.PilotName;
            selectMirandaDoniSubPhase.DescriptionLong = "You may either spend 1 shield to roll 1 additional attack die or, if you are not shielded, you may roll 1 fewer attack die to recover 1 shield";
            selectMirandaDoniSubPhase.ImageSource = HostShip;

            if (HostShip.State.ShieldsCurrent > 0)
            {
                selectMirandaDoniSubPhase.AddDecision("Spend 1 shield to roll 1 extra die", RegisterRollExtraDice);
                selectMirandaDoniSubPhase.AddTooltip("Spend 1 shield to roll 1 extra die", HostShip.ImageUrl);
            }

            if (HostShip.State.ShieldsCurrent == 0)
            {
                selectMirandaDoniSubPhase.AddDecision("Roll 1 fewer die to recover 1 shield", RegisterRegeneration);
                selectMirandaDoniSubPhase.AddTooltip("Roll 1 fewer die to recover 1 shield", HostShip.ImageUrl);
            }

            selectMirandaDoniSubPhase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });

            selectMirandaDoniSubPhase.DefaultDecisionName = GetDefaultDecision();

            selectMirandaDoniSubPhase.ShowSkipButton = true;

            selectMirandaDoniSubPhase.Start();
        }

        protected override string GetDefaultDecision()
        {
            string result = "No";

            if (HostShip.State.ShieldsCurrent == 0)
            {
                result = "Roll 1 fewer die to recover 1 shield";
            }
            else
            {
                result = "Spend 1 shield to roll 1 extra die";
            }

            return result;
        }
    }
}
