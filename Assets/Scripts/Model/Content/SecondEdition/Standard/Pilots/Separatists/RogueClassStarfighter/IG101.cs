using Abilities.SecondEdition;
using Content;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class IG101 : RogueClassStarfighter
        {
            public IG101() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "IG-101",
                    "Tenacious Bodyguard",
                    Faction.Separatists,
                    4,
                    5,
                    19,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.IG101Ability),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Droid
                    }
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                DeadToRights oldAbility = (DeadToRights)ShipAbilities.First(n => n.GetType() == typeof(DeadToRights));
                oldAbility.DeactivateAbility();
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new NetworkedCalculationsAbility());

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/ig101.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IG101Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsPhaseStart += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsPhaseStart -= RegisterAbility;
        }

        private void RegisterAbility(Ship.GenericShip ship)
        {
            if (HostShip.Damage.HasFaceupCards)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsPhaseStart, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);
            AskToUseAbility(
                HostShip.PilotName,
                AlwaysUseByDefault,
                UseOwnAbility,
                descriptionLong: "Do you want to repair 1 Faceup Damage Card?",
                imageHolder: HostShip
            );
        }

        private void UseOwnAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            if (HostShip.Damage.GetFaceupCrits().Count == 1)
            {
                DoAutoRepair();
            }
            else
            {
                AskToSelectCrit();
            }
        }

        private void DoAutoRepair()
        {
            HostShip.Damage.FlipFaceupCritFacedown(HostShip.Damage.GetFaceupCrits().First());
            Triggers.FinishTrigger();
        }

        private void AskToSelectCrit()
        {
            IG101DecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<IG101DecisionSubPhase>(
                "IG-101: Select faceup damage card",
                Triggers.FinishTrigger
            );

            subphase.DescriptionShort = "IG-101";
            subphase.DescriptionLong = "Select Faceup Damage Card to repair";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }
    }
}

namespace SubPhases
{

    public class IG101DecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            foreach (var faceupCrit in Selection.ThisShip.Damage.GetFaceupCrits().ToList())
            {
                AddDecision(faceupCrit.Name, delegate { Repair(faceupCrit); }, faceupCrit.ImageUrl);
            }

            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        private void Repair(GenericDamageCard critCard)
        {
            Selection.ThisShip.Damage.FlipFaceupCritFacedown(critCard);
            ConfirmDecision();
        }

    }

}