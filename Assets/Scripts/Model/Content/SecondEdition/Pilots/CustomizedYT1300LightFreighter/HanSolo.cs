using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CustomizedYT1300LightFreighter
    {
        public class HanSolo : CustomizedYT1300LightFreighter
        {
            public HanSolo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Han Solo",
                    6,
                    54,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HanSoloScumPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 222
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HanSoloScumPilotAbility : GenericAbility
    {
        private bool UseAbility;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += CheckAttackAbility;
            HostShip.OnDefenceStartAsDefender += CheckDefenseAbility;
            HostShip.AfterGotNumberOfDefenceDice += CheckDefenseObstructionBonus;
            HostShip.AfterGotNumberOfAttackDice += CheckAttackObstructionBonus;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= CheckAttackAbility;
            HostShip.OnDefenceStartAsDefender -= CheckDefenseAbility;
            HostShip.AfterGotNumberOfDefenceDice -= CheckDefenseObstructionBonus;
            HostShip.AfterGotNumberOfAttackDice -= CheckAttackObstructionBonus;
        }

        private void CheckAttackAbility()
        {
            if (Combat.ShotInfo.IsObstructedByAsteroid && Combat.ShotInfo.Weapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                if (alwaysUseAbility)
                {
                    UseAbility = true;
                }
                else
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskToUseAdditionalDie);
                }
            }
        }

        private void CheckDefenseAbility()
        {
            if (Combat.ShotInfo.IsObstructedByAsteroid)
            {
                if (alwaysUseAbility)
                {
                    UseAbility = true;
                }
                {
                    RegisterAbilityTrigger(TriggerTypes.OnDefenseStart, AskToUseAdditionalDie);
                }
            }
        }

        private void AskToUseAdditionalDie(object sender, EventArgs e)
        {
            AskToUseAbility(
                AlwaysUseByDefault,
                UseAdditionalDie,
                showAlwaysUseOption: true,
                infoText: "Han Solo: Roll an additional die?"
            );
        }

        private void UseAdditionalDie(object sender, EventArgs e)
        {
            UseAbility = true;
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void CheckDefenseObstructionBonus(ref int count)
        {
            if (UseAbility && Combat.ShotInfo.IsObstructedByAsteroid)
            {
                count++;
                Messages.ShowInfo("The attack against Han Solo is obstructed, Han rolls +1 defense die");
                UseAbility = false;
            }
        }

        private void CheckAttackObstructionBonus(ref int count)
        {
            if (UseAbility && Combat.ShotInfo.IsObstructedByAsteroid && Combat.ShotInfo.Weapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                count++;
                Messages.ShowInfo("Han Solo's attack is obstructed, Han rolls +1 attack die");
                UseAbility = false;
            }
        }
    }
}