using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.G1AStarfighter
    {
        public class Zuckuss : G1AStarfighter
        {
            public Zuckuss() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Zuckuss",
                    "Meditative Gand",
                    Faction.Scum,
                    3,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ZuckussAbility),
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter,
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    seImageNumber: 202,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ZuckussAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterZuckussAbility;
            HostShip.OnAttackFinishAsAttacker += RemoveZuckussAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterZuckussAbility;
            HostShip.OnAttackFinishAsAttacker -= RemoveZuckussAbility;
        }

        private void RegisterZuckussAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, ShowDecision);
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                UseAbility,
                descriptionLong: "Do you want to roll 1 additional attack die? (If you do, then the defender rolls 1 additional defense die)",
                imageHolder: HostShip
            );
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;

            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += ZuckussAddAttackDice;
            Combat.Defender.AfterGotNumberOfDefenceDice += ZuckussAddAttackDice;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void RemoveZuckussAbility(GenericShip genericShip)
        {
            // At the end of combat phase, need to remove attack value increase
            if (IsAbilityUsed)
            {
                HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= ZuckussAddAttackDice;
                Combat.Defender.AfterGotNumberOfDefenceDice -= ZuckussAddAttackDice;
                IsAbilityUsed = false;
            }
        }

        private void ZuckussAddAttackDice(ref int value)
        {
            value++;
        }
    }
}