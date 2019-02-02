using System;
using ActionsList;
using Ship;
using SubPhases;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class KetsuOnyo : GenericUpgrade
    {
        public KetsuOnyo()
        {
            UpgradeInfo = new UpgradeCardInfo(
            "Ketsu Onyo",
            UpgradeType.Crew,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.KetsuOnyoAbility),
                seImageNumber: 134,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum));
        }
    }
}

namespace Abilities.SecondEdition
{
    // At the start of the End Phase, you may choose 1 enemy ship
    // at range 0-2 in your firing arc. If you do, that ship does not remove its tractor tokens
    public class KetsuOnyoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, UseAbility);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            if (TargetsForAbilityExist(FilterTargetShip))
            {
                Selection.ChangeActiveShip(HostShip);
                SelectTargetForAbility(TargetIsSelected, FilterTargetShip, GetAiPriority, HostShip.Owner.PlayerNo, "Keysu Onyo's Ability", "You may choose 1 enemy ship. That ship does not remove its tractor tokens", HostShip);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void TargetIsSelected()
        { 
            TargetShip.BeforeRemovingTokenInEndPhase += BeforeRemovingTokenInEndPhase;
            TargetShip.OnSystemsPhaseStart += RemoveEvents;
           
            SelectShipSubPhase.FinishSelectionNoCallback();
            Triggers.FinishTrigger();

        }

        void BeforeRemovingTokenInEndPhase(Tokens.GenericToken token, ref bool remove)
        {
            if (token is Tokens.TractorBeamToken) 
            {
                remove = false;
            }
        }

        void RemoveEvents(GenericShip ship)
        {
            TargetShip.BeforeRemovingTokenInEndPhase -= BeforeRemovingTokenInEndPhase;
            TargetShip.OnSystemsPhaseStart -= RemoveEvents;
        }


        private bool FilterTargetShip(GenericShip otherShip){
            return otherShip.Owner != HostShip.Owner && otherShip.InPrimaryWeaponFireZone(HostShip, 0, 2)
                            && otherShip.Tokens.HasToken<Tokens.TractorBeamToken>();
        }

        private int GetAiPriority(GenericShip otherShip) {
            return 1;
        }

       
    }
}