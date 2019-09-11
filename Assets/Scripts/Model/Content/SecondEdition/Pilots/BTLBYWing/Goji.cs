using BoardTools;
using Bombs;
using Ship;
using System;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class Goji : BTLBYWing
        {
            public Goji() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Goji\"",
                    2,
                    34,
                    isLimited: true,
                    extraUpgradeIcon: UpgradeType.Astromech,
                    abilityType: typeof(Abilities.SecondEdition.GojiAbility)
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/71/ca/71ca5555-2cf9-4d11-8163-c443669897bd/swz48_pilot-goji.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GojiAbility : GenericAbility
    {
        private int BombsAndMinesInRangeCount;
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsDefenderGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsDefenderGlobal -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (Combat.Defender.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, Combat.Defender);
            if (distInfo.Range > 3) return;

            BombsAndMinesInRangeCount = 0;
            foreach (var bombHolder in BombsManager.GetBombsOnBoard().Where(n => n.Value.HostShip.Owner.PlayerNo == HostShip.Owner.PlayerNo))
            {
                if (BombsManager.IsShipInRange(Combat.Defender, bombHolder.Key, 1))
                {
                    BombsAndMinesInRangeCount++;
                    break;
                }
            }

            if (BombsAndMinesInRangeCount > 0) RegisterAbilityTrigger(TriggerTypes.OnDefenseStart, AskToAddExtraDice);
        }

        private void AskToAddExtraDice(object sender, EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    ChooseToAddExtraDice,
                    showAlwaysUseOption: true,
                    descriptionLong: "Do you want to roll 1 additional defense dice for each friendly bomb or mine in range?",
                    imageHolder: HostShip,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
            else
            {
                PlanToAddExtraDice();
            }
        }

        private void ChooseToAddExtraDice(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            PlanToAddExtraDice();
        }

        private void PlanToAddExtraDice()
        {
            Combat.Defender.AfterGotNumberOfDefenceDice += AddExtraDice;

            Triggers.FinishTrigger();
        }

        private void AddExtraDice(ref int count)
        {
            Combat.Defender.OnAttackFinishAsDefender += PlanToDisableAbility;

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Exra dice are added");
            count += BombsAndMinesInRangeCount;
        }

        private void PlanToDisableAbility(GenericShip ship)
        {
            Combat.Defender.OnAttackFinishAsDefender -= PlanToDisableAbility;

            Combat.Defender.AfterGotNumberOfDefenceDice -= AddExtraDice;
        }
    }
}
