using ActionsList;
using Bombs;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class Deathfire : TIESaBomber
        {
            public Deathfire() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Deathfire\"",
                    "Unflinching Diehard",
                    Faction.Imperial,
                    2,
                    4,
                    11,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DeathfireAbility),
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Torpedo,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    seImageNumber: 110,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Gamma Squadron";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DeathfireAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShipIsDestroyed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShipIsDestroyed -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, bool isFled)
        {
            if (!isFled)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, AskDeathfireAbility);
            }
        }

        private void AskDeathfireAbility(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            DeathfireAbilityDecision subphase = Phases.StartTemporarySubPhaseNew<DeathfireAbilityDecision>(
                HostShip.PilotInfo.PilotName,
                Triggers.FinishTrigger
            );

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "You may perform an attack and drop or launch 1 device";
            subphase.ImageSource = HostShip;

            subphase.AddDecision("Perform attack", DoAttack);
            subphase.AddDecision("Drop or launch a device", DropBomb);

            subphase.DecisionOwner = HostShip.Owner;
            subphase.DefaultDecisionName = "Perform attack";
            subphase.ShowSkipButton = true;

            subphase.Start();
        }

        private void DoAttack(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            GenericShip AttackerBeforeAbility = Combat.Attacker;
            GenericShip DefenderBeforeAbility = Combat.Defender;
            Combat.StartSelectAttackTarget(HostShip, () => {
                Combat.Attacker = AttackerBeforeAbility;
                Combat.Defender = DefenderBeforeAbility;
                Triggers.FinishTrigger();
            }, abilityName: "Attack", description: "Select target");
        }

        private void DropBomb(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            BombsManager.RegisterBombDropTriggerIfAvailable(HostShip, TriggerTypes.OnAbilityDirect);
            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private class DeathfireAbilityDecision: DecisionSubPhase { };
    }
}