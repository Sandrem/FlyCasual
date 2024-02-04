using ActionsList;
using Content;
using SubPhases;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AggressorAssaultFighter
    {
        public class IG88C : AggressorAssaultFighter
        {
            public IG88C() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "IG-88C",
                    "Conniving Contraption",
                    Faction.Scum,
                    4,
                    7,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.IG88CAbility),
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter,
                        Tags.Droid
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    seImageNumber: 198,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IG88CAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckBoostBonus;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckBoostBonus;
        }

        private void CheckBoostBonus(GenericAction action)
        {
            if (action is BoostAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionDecisionSubPhaseEnd, AskAssignEvade);
            }
        }

        private void AskAssignEvade(object sender, System.EventArgs e)
        {
            if (Selection.ThisShip.CanPerformAction(new EvadeAction()))
            {
                if (!alwaysUseAbility)
                {
                    AskToUseAbility(
                        HostShip.PilotInfo.PilotName,
                        AlwaysUseByDefault,
                        PerformFreeEvadeActionDecision,
                        descriptionLong: "Do you want to perform an Evade Action?",
                        imageHolder: HostShip,
                        showAlwaysUseOption: true
                    );
                }
                else
                {
                    PerformFreeEvadeActionDecision(null, null);
                }
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void PerformFreeEvadeActionDecision(object sender, System.EventArgs e)
        {
            //Action is forced now
            Selection.ThisShip.AskPerformFreeAction(
                new EvadeAction(),
                DecisionSubPhase.ConfirmDecision,
                HostShip.PilotInfo.PilotName,
                "You must perform a free evade action",
                HostShip,
                isForced: true
            );
        }
    }
}