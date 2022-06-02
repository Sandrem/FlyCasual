using Content;
using MainPhases;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class Rush : TIEVnSilencer
        {
            public Rush() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Rush\"",
                    "Adrenaline Junkie",
                    Faction.FirstOrder,
                    2,
                    5,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RushAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/886d715885da65bdf10ad7c68e4d0a93.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RushAbility : GenericAbility, IModifyPilotSkill
    {
        public override void ActivateAbility()
        {
            HostShip.OnDamageCardIsDealt += ApplyInitiative;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDamageCardIsDealt -= ApplyInitiative;
            HostShip.State.RemovePilotSkillModifier(this);
        }

        private void ApplyInitiative(GenericShip ship)
        {
            if (IsMissedWindowOfAttack()) UpdateCombatInitiativeForOneActivation();

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Initiative is set to 6");
            HostShip.State.AddPilotSkillModifier(this);
            HostShip.OnDamageCardIsDealt -= ApplyInitiative;
        }

        private bool IsMissedWindowOfAttack()
        {
            return !HostShip.IsAttackPerformed
                && Phases.CurrentPhase is CombatPhase
                && CombatPhase.LastInitiative >= HostShip.State.CombatActivationAtInitiative;
        }

        private void UpdateCombatInitiativeForOneActivation()
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " will have combat activation after all ships at Initiative " + CombatPhase.LastInitiative);

            // Disable combat acivations for this ship until change of initiative
            HostShip.State.CombatActivationAtInitiative = 0;
            Phases.Events.OnEngagementInitiativeIsReadyToChange += WaitForLastShipToAddNewCombatActivation;
        }

        private void WaitForLastShipToAddNewCombatActivation(ref bool stopInitiativeChange)
        {
            Phases.Events.OnEngagementInitiativeIsReadyToChange -= WaitForLastShipToAddNewCombatActivation;

            stopInitiativeChange = true;
            Phases.CurrentSubPhase.RequiredInitiative = CombatPhase.LastInitiative;
            Phases.CurrentSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " delayed combat activation");
            HostShip.State.CombatActivationAtInitiative = CombatPhase.LastInitiative;
            HostShip.AfterAttackWindow += RestoreCombatInitiativeToDefault;
        }

        private void RestoreCombatInitiativeToDefault()
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " will have combat activations as usual");
            HostShip.AfterAttackWindow -= RestoreCombatInitiativeToDefault;
            HostShip.State.CombatActivationAtInitiative = -1;
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 6;
        }
    }
}