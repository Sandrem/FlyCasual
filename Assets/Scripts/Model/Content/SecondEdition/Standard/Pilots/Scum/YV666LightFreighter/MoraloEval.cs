using Content;
using SubPhases;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YV666LightFreighter
    {
        public class MoraloEval : YV666LightFreighter
        {
            public MoraloEval() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Moralo Eval",
                    "Criminal Mastermind",
                    Faction.Scum,
                    4,
                    7,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MoraloEvalAbility),
                    charges: 2,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    seImageNumber: 211,
                    skinName: "Crimson"
                );
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
