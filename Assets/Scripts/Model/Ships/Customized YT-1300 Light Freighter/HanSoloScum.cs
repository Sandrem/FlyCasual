using RuleSets;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ScumYT1300
    {
        public class HanSoloScum : ScumYT1300, ISecondEditionPilot
        {
            public HanSoloScum() : base()
            {
                PilotName = "Han Solo";
                PilotSkill = 6;
                Cost = 54;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.HanSoloScumPilotAbilitySE());

                SEImageNumber = 222;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HanSoloScumPilotAbilitySE : GenericAbility
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
            if (Combat.ShotInfo.IsObstructedByAsteroid && Combat.ShotInfo.Weapon == HostShip.PrimaryWeapon)
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
                Messages.ShowInfo("Han Solo: +1 defense die");
                UseAbility = false;
            }
        }

        private void CheckAttackObstructionBonus(ref int count)
        {
            if (UseAbility && Combat.ShotInfo.IsObstructedByAsteroid && Combat.ShotInfo.Weapon == HostShip.PrimaryWeapon)
            {
                count++;
                Messages.ShowInfo("Han Solo: +1 attack die");
                UseAbility = false;
            }
        }
    }
}
