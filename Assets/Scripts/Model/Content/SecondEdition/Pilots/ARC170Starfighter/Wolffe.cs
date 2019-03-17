using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class Wolffe : ARC170Starfighter
        {
            public Wolffe() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Wolffe\"",
                    4,
                    50,
                    isLimited: true,
                    factionOverride: Faction.Republic,
                    abilityType: typeof(Abilities.SecondEdition.WolffeAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    charges: 1
                );

                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/fb/ae/fbae3c90-c9bb-483a-a929-4381c205d416/swz33_wolffe.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary front arc attack, you may spend 1 charge to reroll 1 attack die.
    //While you perform a primary back arc attack, you may recover 1 charge to roll 1 additional attack die.
    public class WolffeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsFrontArcModAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                1,
                payAbilityCost: payFrontArcAbilityCost
            );

            HostShip.OnShotStartAsAttacker += CheckAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
            HostShip.OnShotStartAsAttacker -= CheckAbilityTrigger;
        }


        public int GetDiceModificationAiPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                if (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (attackBlanks > 0) result = 90;
                }
                else
                {
                    if (attackBlanks + attackFocuses > 0) result = 90;
                }
            }

            return result;
        }
        private bool IsFrontArcModAvailable()
        {
            return HostShip.State.Charges > 0
                && Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && Combat.ShotInfo.InArcByType(Arcs.ArcType.Front);
        }

        private bool IsBackArcModAvailable()
        {
            return HostShip.State.Charges < HostShip.State.MaxCharges
                && Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && Combat.ShotInfo.InArcByType(Arcs.ArcType.Rear);
        }

        private void payFrontArcAbilityCost(Action<bool> callback)
        {
            if (HostShip.State.Charges > 0) {
                HostShip.State.Charges--;
                callback(true);
            }
            else
            {
                callback(false);
            }
        }

        private void CheckAbilityTrigger()
        {
            if (IsBackArcModAvailable())
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotStart, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, UseAbilityDecision, null, null, true);
            }
            else
            {
                AllowRollAdditionalDie();
                Triggers.FinishTrigger();
            }
        }

        private void UseAbilityDecision(object sender, EventArgs e)
        {
            if (HostShip.State.Charges < HostShip.State.MaxCharges)
            {
                AllowRollAdditionalDie();
                HostShip.State.Charges++;
            }
            DecisionSubPhase.ConfirmDecision();
        }

        private void AllowRollAdditionalDie()
        {
            HostShip.AfterGotNumberOfAttackDice += RollExtraDie;
        }

        protected void RollExtraDie(ref int diceCount)
        {
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDie;
            Messages.ShowInfo(HostName + ": +1 attack die");
            diceCount++;
        }
    }
}