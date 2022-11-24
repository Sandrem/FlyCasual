using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YV666LightFreighter
    {
        public class DoctorAphra : YV666LightFreighter
        {
            public DoctorAphra() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Doctor Aphra",
                    "Professional Disaster Zone",
                    Faction.Scum,
                    3,
                    6,
                    22,
                    charges: 3,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DoctorAphraPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/cab18bcf-9835-49d1-840b-82cc8f5ac976/SWZ97_DoctorAphralegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DoctorAphraPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += TryRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= TryRegisterAbility;
        }

        private void TryRegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, TryStartDoctorAphraAbility);
        }

        private void TryStartDoctorAphraAbility(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasGreenTokens
                && HostShip.State.Charges > 0
                && HasAnyAnotherShipsNearWithoutStress())
            {
                AskForDoctorAphraTarget();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AskForDoctorAphraTarget()
        {
            SelectTargetForAbility(
                ShipIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a ship to assign Stress token to it",
                HostShip
            );
        }

        private bool HasAnyAnotherShipsNearWithoutStress()
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (Tools.IsSameShip(HostShip, ship)) continue;
                if (ship.IsStressed) continue;

                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range <= 1) return true;
            }

            return false;
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.OtherAny)
                && FilterTargetsByRange(ship, 0, 1);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = ship.PilotInfo.Cost;

            if (Tools.IsSameTeam(HostShip, ship)) priority = 0;

            return priority;
        }

        private void ShipIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            StartSelectGreenTokenDecisionSubphase();
        }

        private void StartSelectGreenTokenDecisionSubphase()
        {
            DoctorAphraScumAbilityDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<DoctorAphraScumAbilityDecisionSubPhase>(
                "Doctor Aphra: Select green token to spend",
                FinishAbility
            );

            subphase.DescriptionShort = "Doctor Aphra";
            subphase.DescriptionLong = "Select a green token to spend to assign a Stress token";
            subphase.ImageSource = HostShip;

            subphase.HostShip = HostShip;
            subphase.DecisionOwner = HostShip.Owner;
            subphase.Start();
        }

        private void FinishAbility()
        {
            HostShip.SpendCharge();

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " assigned a Stress Token to " + TargetShip.PilotInfo.PilotName);
            TargetShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }
    }
}

namespace SubPhases
{
    public class DoctorAphraScumAbilityDecisionSubPhase : SpendGreenTokenDecisionSubPhase { }
}
