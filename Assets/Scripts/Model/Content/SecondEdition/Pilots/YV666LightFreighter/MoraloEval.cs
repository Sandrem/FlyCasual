﻿using SubPhases;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YV666LightFreighter
    {
        public class MoraloEval : YV666LightFreighter
        {
            public MoraloEval() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Moralo Eval",
                    4,
                    66,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MoraloEvalAbility),
                    charges: 2,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 211
                );

                ModelInfo.SkinName = "Crimson";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MoraloEvalAbility : GenericAbility
    {
        Direction ShipFledSide;

        public override void ActivateAbility()
        {
            HostShip.OnOffTheBoard += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnOffTheBoard -= CheckAbility;
        }

        private void CheckAbility(ref bool shouldBeDestroyed, Direction direction)
        {
            if (HostShip.State.Charges > 0)
            {
                ShipFledSide = direction;

                HostShip.SpendCharge();
                shouldBeDestroyed = false;

                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " has been moved to the Reserve");

                Roster.MoveToReserve(HostShip);

                Phases.Events.OnPlanningPhaseStart += RegisterSetup;
            }
        }

        private void RegisterSetup()
        {
            Phases.Events.OnPlanningPhaseStart -= RegisterSetup;

            RegisterAbilityTrigger(TriggerTypes.OnPlanningSubPhaseStart, SetupShip);
        }

        private void SetupShip(object sender, System.EventArgs e)
        {
            Roster.ReturnFromReserve(HostShip);

            var subphase = Phases.StartTemporarySubPhaseNew<SetupShipMidgameSubPhase>(
                "Setup",
                delegate {
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + " has returned to the play area");
                    Triggers.FinishTrigger();
                }
            );

            subphase.ShipToSetup = HostShip;
            subphase.SetupSide = ShipFledSide;
            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "Place yourself within range 1 of the edge of the play area that you fled from";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }
    }
}
