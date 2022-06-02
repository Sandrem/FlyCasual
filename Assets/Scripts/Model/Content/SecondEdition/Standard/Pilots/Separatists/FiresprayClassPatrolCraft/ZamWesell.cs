using BoardTools;
using Conditions;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class ZamWesell : FiresprayClassPatrolCraft
        {
            public ZamWesell() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Zam Wesell",
                    "Clawdite Changeling",
                    Faction.Separatists,
                    5,
                    9,
                    22,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ZamWesellPilotAbility),
                    charges: 4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    skinName: "Jango Fett"
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/08/f3/08f343b7-9d01-4e1f-91d8-e6eca0eb4fe0/swz82_a1_zam-wessel.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ZamWesellPilotAbility : GenericAbility
    {
        protected ZamWesellSecretCondition AssignedCondition;
        private GenericShip ShipToPunish;
        private bool IsPerformedRegularAttack;

        protected virtual string AbilityHostName { get{ return HostShip.PilotInfo.PilotName;} }

        public override void ActivateAbility()
        {
            HostShip.OnSetupPlaced += LoseChargesOnSetup;

            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;

            HostShip.OnAttackFinishAsDefender += CheckAttackFinishCondition;
            Phases.Events.OnCombatPhaseEnd_Triggers += CheckCombatFinishCondition;

            HostShip.OnSystemsPhaseStart += RemoveOwnConditions;
        }

        protected virtual void LoseChargesOnSetup(GenericShip ship)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: 2 Charges are lost during Setup");
            HostShip.State.Charges -= 2;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            flag = true;
        }

        // ASSIGN CONDITION

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToAssignCondition);
        }

        private void AskToAssignCondition(object sender, EventArgs e)
        {
            AskForDecision(
                AbilityHostName,
                "You may assign one of your secret conditions to yourself facedown:",
                imageHolder: HostReal as IImageHolder,
                decisions: new Dictionary<string, EventHandler>()
                {
                    { "You Should Thank Me", delegate{ AssignSecretCondition(typeof(YouShouldThankMeCondition)); } },
                    { "You'd Better Mean Business", delegate{ AssignSecretCondition(typeof(YoudBetterMeanBusiness)); } }
                },
                tooltips: new Dictionary<string, string>()
                {
                    { "You Should Thank Me", "https://images-cdn.fantasyflightgames.com/filer_public/30/68/3068be81-f299-4c69-b5ee-307ac1da9c89/swz82_a1_thank-me.png" },
                    { "You'd Better Mean Business", "https://images-cdn.fantasyflightgames.com/filer_public/24/92/2492e698-3402-4ec8-9a8d-bc7e30aea98c/swz82_a1_mean-business.png" }
                },
                defaultDecision: GetDefaultDecision(),
                requiredPlayer: HostShip.Owner.PlayerNo
            );;
        }

        protected virtual void AssignSecretCondition(Type conditionType)
        {
            AssignedCondition = Activator.CreateInstance(conditionType, HostShip) as ZamWesellSecretCondition;
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Secret condition is assigned");
            HostShip.Tokens.AssignToken(AssignedCondition, Triggers.FinishTrigger);
        }

        private string GetDefaultDecision()
        {
            return "You Should Thank Me";
        }

        // AFTER ATTACK

        private void CheckAttackFinishCondition(GenericShip ship)
        {
            if (AssignedCondition != null)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AfterAttack);
            }
        }

        private void AfterAttack(object sender, EventArgs e)
        {
            if (!AssignedCondition.IsRevealed)
            {
                AssignedCondition.IsRevealed = true;
                Messages.ShowInfo($"Secret condition is revealed:\n\"{AssignedCondition.Name}\"");
            }

            if (AssignedCondition is YouShouldThankMeCondition)
            {
                RestoreCharges(1);

                AskToUseAbility(
                    AssignedCondition.Name,
                    AlwaysUseByDefault,
                    GetLockOnAttacker,
                    descriptionLong: "Do you want to acquire lock on the attacker?",
                    imageHolder: HostReal as IImageHolder,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
            else if (AssignedCondition is YoudBetterMeanBusiness && GetCharges() >= 2)
            {
                AskToUseAbility(
                    AssignedCondition.Name,
                    AlwaysUseByDefault,
                    ConfirmExtraAttackPunish,
                    descriptionLong: "Do you want to perform a bonus attack against the attacker?",
                    imageHolder: HostReal as IImageHolder,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected virtual int GetCharges()
        {
            return HostShip.State.Charges;
        }

        protected virtual void RestoreCharges(int count)
        {
            HostShip.RestoreCharges(count);
        }

        // AFTER ATTACK A: GET LOCK

        private void GetLockOnAttacker(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            ActionsHolder.AcquireTargetLock(HostShip, Combat.Attacker, Triggers.FinishTrigger, Triggers.FinishTrigger);
        }

        // AFTER ATTACK B: COUNTER ATTACK

        private void ConfirmExtraAttackPunish(object sender, EventArgs e)
        {
            LoseCharges(2);

            if (IsAbilityUsed) return;

            if (HostShip.IsCannotAttackSecondTime) return;

            // Save his attacker, becuase combat data will be cleared
            ShipToPunish = Combat.Attacker;

            Combat.Attacker.OnCombatCheckExtraAttack += RegisterExtraAttackAbility;

            DecisionSubPhase.ConfirmDecision();
        }

        protected virtual void LoseCharges(int count)
        {
            for (int i = 0; i < count; i++)
            {
                HostShip.LoseCharge();
            }
        }

        private void RegisterExtraAttackAbility(GenericShip ship)
        {
            ship.OnCombatCheckExtraAttack -= RegisterExtraAttackAbility;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, DoCounterAttack);
        }

        private void DoCounterAttack(object sender, EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                // Save his "is already attacked" flag
                IsPerformedRegularAttack = HostShip.IsAttackPerformed;

                // Plan to set IsAbilityUsed only after attack that was successfully started
                HostShip.OnAttackStartAsAttacker += MarkAbilityAsUsed;

                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishExtraAttack,
                    CounterAttackFilter,
                    AbilityHostName,
                    "You may perform an additional attack against " + ShipToPunish.PilotInfo.PilotName,
                    HostReal as IImageHolder
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        protected virtual void MarkAbilityAsUsed()
        {
            IsAbilityUsed = true;
        }

        private void FinishExtraAttack()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = IsPerformedRegularAttack;

            // Set IsAbilityUsed only after attack that was successfully started
            HostShip.OnAttackStartAsAttacker -= MarkAbilityAsUsed;

            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }

        private bool CounterAttackFilter(GenericShip targetShip, IShipWeapon weapon, bool isSilent)
        {
            bool result = true;

            if (targetShip != ShipToPunish)
            {
                if (!isSilent) Messages.ShowErrorToHuman(string.Format("{0} can only attack {1}", HostShip.PilotInfo.PilotName, ShipToPunish.PilotInfo.PilotName));
                result = false;
            }

            return result;
        }

        // END OF COMBAT

        private void CheckCombatFinishCondition()
        {
            if (AssignedCondition != null && !AssignedCondition.IsRevealed && IsInAnyEnemyArc() && !HostShip.IsDestroyed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, AfterCombat);
            }
        }

        private bool IsInAnyEnemyArc()
        {
            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                ShotInfo shotInfo = new ShotInfo(enemyShip, HostShip, enemyShip.PrimaryWeapons);
                if (shotInfo.InArc) return true;
            }

            return false;
        }

        private void AfterCombat(object sender, EventArgs e)
        {
            if (AssignedCondition is YouShouldThankMeCondition)
            {
                if (GetCharges() >= 2)
                {
                    AskToUseAbility(
                        AssignedCondition.Name,
                        AlwaysUseByDefault,
                        ConfirmExtraAttackFree,
                        descriptionLong: "Do you want to perform a bonus attack?",
                        imageHolder: HostReal as IImageHolder,
                        requiredPlayer: HostShip.Owner.PlayerNo
                    );
                }
                else
                {
                    Messages.ShowInfo("You Should Thank Me: There are not enough charges for extra attack");
                    Triggers.FinishTrigger();
                }
            }
            else if (AssignedCondition is YoudBetterMeanBusiness)
            {
                AskToUseAbility(
                    AssignedCondition.Name,
                    AlwaysUseByDefault,
                    RestoreTwoCharges,
                    descriptionLong: "Do you want to recover two charges?",
                    imageHolder: HostReal as IImageHolder,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        // END OF COMBAT A: RESTORE CHARGES

        private void RestoreTwoCharges(object sender, EventArgs e)
        {
            AssignedCondition.IsRevealed = true;
            Messages.ShowInfo($"Secret condition is revealed:\n\"{AssignedCondition.Name}\"");
            Messages.ShowInfo($"\"{AssignedCondition.Name}\":\n{AbilityHostName} recovered 2 Charges");

            RestoreCharges(2);

            DecisionSubPhase.ConfirmDecision();
        }

        // END OF COMBAT B: FREE BONUS ATTACK

        private void ConfirmExtraAttackFree(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            LoseCharges(2);

            if (IsAbilityUsed) return;

            if (HostShip.IsCannotAttackSecondTime) return;

            // Not a post-combat attack check, free attack instead
            DoFreeAttack();
        }

        private void DoFreeAttack()
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                // Save his "is already attacked" flag
                IsPerformedRegularAttack = HostShip.IsAttackPerformed;

                // Plan to set IsAbilityUsed only after attack that was successfully started
                HostShip.OnAttackStartAsAttacker += MarkAbilityAsUsed;

                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishExtraAttack,
                    FreeAttackFilter,
                    AbilityHostName,
                    "You may perform a bonus attack",
                    HostReal as IImageHolder
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private bool FreeAttackFilter(GenericShip targetShip, IShipWeapon weapon, bool isSilent)
        {
            return true;
        }

        // REMOVE CONDITIONS

        private void RemoveOwnConditions(GenericShip ship)
        {
            HostShip.Tokens.RemoveCondition(AssignedCondition);
            AssignedCondition = null;

            IsAbilityUsed = false;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSetupPlaced -= LoseChargesOnSetup;

            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;

            HostShip.OnAttackFinishAsDefender -= CheckAttackFinishCondition;
            Phases.Events.OnCombatPhaseEnd_Triggers -= CheckCombatFinishCondition;

            HostShip.OnSystemsPhaseStart -= RemoveOwnConditions;
        }
    }
}

namespace Conditions
{
    public class ZamWesellSecretCondition : GenericToken
    {
        public bool IsRevealed { get; set; }

        public ZamWesellSecretCondition(GenericShip host) : base(host)
        {
            Name = ImageName = "Zam Wesell Secret Condition";
            Temporary = false;
        }
    }

    public class YouShouldThankMeCondition : ZamWesellSecretCondition
    {
        public YouShouldThankMeCondition(GenericShip host) : base(host)
        {
            Name = "You Should Thank Me";
            Tooltip = (IsRevealed) ? "https://images-cdn.fantasyflightgames.com/filer_public/30/68/3068be81-f299-4c69-b5ee-307ac1da9c89/swz82_a1_thank-me.png" : null;
        }
    }

    public class YoudBetterMeanBusiness : ZamWesellSecretCondition
    {
        public YoudBetterMeanBusiness(GenericShip host) : base(host)
        {
            Name = "You'd Better Mean Business";
            Tooltip = (IsRevealed) ? "https://images-cdn.fantasyflightgames.com/filer_public/24/92/2492e698-3402-4ec8-9a8d-bc7e30aea98c/swz82_a1_mean-business.png" : null;
        }
    }
}