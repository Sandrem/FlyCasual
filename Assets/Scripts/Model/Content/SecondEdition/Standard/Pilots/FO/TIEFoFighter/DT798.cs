using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class DT798 : TIEFoFighter
        {
            public DT798() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "DT-798",
                    "Jace Rucklin",
                    Faction.FirstOrder,
                    4,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DT798PilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/ad1d0d9c-9706-4e50-8d3b-8cd40877ea34/SWZ97_DT798legal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DT798PilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckAbilityTrigger;
        }

        private void CheckAbilityTrigger()
        {
            if (IsAvailable())
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotStart, AskUseAbility);
            }
        }

        private bool IsAvailable()
        {
            if (HostShip.IsStrained) return false;
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return false;

            return true;
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                UseAbilityDecision,
                descriptionLong: "Do you want to gain 1 Strain to roll 1 additional attack die?",
                imageHolder: HostShip
            );
        }

        private void UseAbilityDecision(object sender, EventArgs e)
        {
            AllowRollAdditionalDie();
            HostShip.Tokens.AssignToken(
                typeof(Tokens.StrainToken),
                DecisionSubPhase.ConfirmDecision
            );
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
