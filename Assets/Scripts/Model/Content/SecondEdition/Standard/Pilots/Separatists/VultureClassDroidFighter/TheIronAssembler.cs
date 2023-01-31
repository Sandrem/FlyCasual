using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class TheIronAssembler : VultureClassDroidFighter
    {
        public TheIronAssembler()
        {
            PilotInfo = new PilotCardInfo25
            (
                "The Iron Assembler",
                "Scintilla Scavenger",
                Faction.Separatists,
                1,
                2,
                5,
                isLimited: true,
                charges: 3,
                abilityType: typeof(Abilities.SecondEdition.TheIronAssemblerAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );

            ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/b03bf8e3-1596-4e31-9220-ea5511043a11/SWZ97_TheIronAssemblerlegal.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After a friendly ship at range 0-1 skips its execute maneuver step, you may spend 1 Charge. If you do, if there is an asteroid or debris cloud at range 0 of it, that ship may repair 1 damage. 
    public class TheIronAssemblerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnManeuverIsSkippedGlobal += CheckAbility;

        }


        public override void DeactivateAbility()
        {
            GenericShip.OnManeuverIsSkippedGlobal -= CheckAbility;
        }


        private void CheckAbility(GenericShip ship)
        {
            bool obstacleCheck = ship.ObstaclesLanded.Any(n => n.GetTypeName == "Asteroid" || n.GetTypeName == "Debris");

            if (ship.IsManeuverSkipped && obstacleCheck && Tools.IsSameTeam(ship, HostShip)
                && new DistanceInfo(ship, HostShip).Range < 2 && ship.Damage.IsDamaged)
            {
                TargetShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsSkipped, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            TheIronAssemblerDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<TheIronAssemblerDecisionSubphase>("The Iron Assembler Decision", Triggers.FinishTrigger);

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "You may spend 1 charge to:";
            subphase.ImageSource = HostShip;

            if (TargetShip.Damage.HasFacedownCards)
            {
                subphase.AddDecision("Repair 1 facedown damage card", RepairFacedownDamageCard);
            }

            if (TargetShip.Damage.HasFaceupCards)
            {
                subphase.AddDecision("Repair 1 faceup damage card", RepairFaceupDamageCard);
            }

            subphase.DecisionOwner = HostShip.Owner;
            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
            subphase.ShowSkipButton = true;

            subphase.Start();
        }

        private void RepairFacedownDamageCard(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.SpendCharge();

            if (TargetShip.Damage.DiscardRandomFacedownCard())
            {
                Messages.ShowInfoToHuman("Facedown Damage card is discarded");
            }

            Triggers.FinishTrigger();
        }

        private void RepairFaceupDamageCard(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.SpendCharge();
            List<GenericDamageCard> shipCritsList = TargetShip.Damage.GetFaceupCrits();

            if (shipCritsList.Count == 1)
            {
                TargetShip.Damage.FlipFaceupCritFacedown(shipCritsList.First());
                Triggers.FinishTrigger();
            }
            else if (shipCritsList.Count > 1)
            {
                Phases.StartTemporarySubPhaseOld(
                    HostShip.PilotInfo.PilotName + ": Select faceup ship Crit",
                    typeof(SubPhases.FixCritDecisionSubPhase),
                    Triggers.FinishTrigger
                );
            }
        }
        private class TheIronAssemblerDecisionSubphase : DecisionSubPhase { }
    }
}
