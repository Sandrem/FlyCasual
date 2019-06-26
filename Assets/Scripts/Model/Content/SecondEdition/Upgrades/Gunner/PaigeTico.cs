using ActionsList;
using Arcs;
using BoardTools;
using Bombs;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PaigeTico : GenericUpgrade
    {
        public PaigeTico() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Paige Tico",
                UpgradeType.Gunner,
                cost: 7,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.PaigeTicoAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/9dc15f634233b5daba107a07aa63d04c.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class PaigeTicoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShipIsDestroyed += TryRegisterDestructionAbility;
            HostShip.OnAttackFinishAsAttacker += RegisterFirstAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= RegisterFirstAbility;
            HostShip.OnShipIsDestroyed -= TryRegisterDestructionAbility;
        }

        private void RegisterFirstAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskRotateOrDrop);
        }

        private void AskRotateOrDrop(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            PageTicoAbilityDecision subphase = Phases.StartTemporarySubPhaseNew<PageTicoAbilityDecision>("Paige Tico's ability", Triggers.FinishTrigger);
            subphase.AddDecision("Drop a bomb", DropBomb);
            subphase.AddDecision("Rotate Arc", AskRotateArc);
            subphase.DecisionOwner = HostShip.Owner;
            subphase.DefaultDecisionName = "Rotate Arc";
            subphase.InfoText = "Ability of Paige Tico";
            subphase.ShowSkipButton = true;
            subphase.Start();
        }

        private void AskRotateArc(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.AskPerformFreeAction(new RotateArcAction(), Triggers.FinishTrigger, isForced: true);
        }

        private void TryRegisterDestructionAbility(GenericShip ship, bool isFled)
        {
            if (!isFled && HasBombsToDrop())
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, AskDropBomb);
            }
        }

        private bool HasBombsToDrop()
        {
            return HostShip.UpgradeBar.GetUpgradesAll().Any(n => 
                n is GenericBomb
                && (n as GenericBomb).UpgradeInfo.SubType == UpgradeSubType.Bomb
                && n.State.Charges > 0
            );
        }

        private void AskDropBomb(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                NeverUseByDefault,
                DropBomb,
                infoText: "Do you want to drop a bomb?"
            );
        }

        private void DropBomb(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            BombsManager.CheckBombDropAvailability(
                HostShip,
                TriggerTypes.OnAbilityDirect,
                subType: UpgradeSubType.Bomb,
                onlyDrop: true
            );

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private class PageTicoAbilityDecision : DecisionSubPhase { };

    }
};