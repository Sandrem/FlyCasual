using Content;
using Ship;
using System;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Han Solo",
                    "The Corellian Kid",
                    Faction.Scum,
                    6,
                    6,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HanSoloScumPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    },
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
            if (Combat.ShotInfo.IsObstructedByObstacle && Combat.ShotInfo.Weapon.WeaponType == WeaponTypes.PrimaryWeapon)
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
            if (Combat.ShotInfo.IsObstructedByObstacle)
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
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                UseAdditionalDie,
                showAlwaysUseOption: true,
                descriptionLong: "Do you want to roll 1 additional die?",
                imageHolder: HostShip
            );
        }

        private void UseAdditionalDie(object sender, EventArgs e)
        {
            UseAbility = true;
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void CheckDefenseObstructionBonus(ref int count)
        {
            if (UseAbility && Combat.ShotInfo.IsObstructedByObstacle)
            {
                count++;
                Messages.ShowInfo("The attack against " + HostShip.PilotInfo.PilotName + " is obstructed, " + HostShip.PilotInfo.PilotName + " rolls +1 defense die");
            }
        }

        private void CheckAttackObstructionBonus(ref int count)
        {
            if (UseAbility && Combat.ShotInfo.IsObstructedByObstacle && Combat.ShotInfo.Weapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                count++;
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + "'s attack is obstructed, " + HostShip.PilotInfo.PilotName + " rolls +1 attack die");
            }
        }
    }
}