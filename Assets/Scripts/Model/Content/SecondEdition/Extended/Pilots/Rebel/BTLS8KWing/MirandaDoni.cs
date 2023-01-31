using Content;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLS8KWing
    {
        public class MirandaDoni : BTLS8KWing
        {
            public MirandaDoni() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Miranda Doni",
                    "Heavy Hitter",
                    Faction.Rebel,
                    4,
                    5,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MirandaDoniAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Crew,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    seImageNumber: 62,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MirandaDoniAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditions;
            Phases.Events.OnRoundEnd += ClearAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
            Phases.Events.OnRoundEnd -= ClearAbility;
        }

        protected virtual void CheckConditions()
        {
            if (Combat.ChosenWeapon.WeaponType == Ship.WeaponTypes.PrimaryWeapon)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotStart, StartQuestionSubphase);
            }
        }

        protected virtual void StartQuestionSubphase(object sender, System.EventArgs e)
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

        protected virtual string GetDefaultDecision()
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

        protected void RegisterRollExtraDice(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostShip.AfterGotNumberOfAttackDice += RollExtraDice;

            DecisionSubPhase.ConfirmDecision();
        }

        private void RollExtraDice(ref int count)
        {
            count++;
            HostShip.LoseShield();

            Messages.ShowInfo("Miranda Doni spends 1 shield to gain +1 attack die");

            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }

        protected void RegisterRegeneration(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostShip.AfterGotNumberOfAttackDice += RegenerateShield;

            DecisionSubPhase.ConfirmDecision();
        }

        private void RegenerateShield(ref int count)
        {
            count--;
            HostShip.TryRegenShields();

            Messages.ShowInfo("Miranda Doni rolls 1 fewer defense die to recover 1 shield");

            HostShip.AfterGotNumberOfAttackDice -= RegenerateShield;
        }

        private void ClearAbility()
        {
            IsAbilityUsed = false;
        }

        protected class MirandaDoniDecisionSubPhase : DecisionSubPhase { }
    }
}
