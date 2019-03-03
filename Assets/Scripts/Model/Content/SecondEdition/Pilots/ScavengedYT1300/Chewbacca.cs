using ActionsList;
using BoardTools;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScavengedYT1300
    {
        public class Chewbacca : ScavengedYT1300
        {
            public Chewbacca() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Chewbacca",
                    4,
                    72,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ChewbaccaPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 69
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/422fc30e0e10445e80b304ef2d96dc06.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChewbaccaPilotAbility : GenericAbility
    {
        GenericShip selectedShip;
        private bool performedRegularAttack;
        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += RegisterOnDestroyedFriendly;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= RegisterOnDestroyedFriendly;
        }

        protected void RegisterOnDestroyedFriendly(GenericShip ship, bool isFled)
        {
            if (ship.Owner == HostShip.Owner && Board.IsShipBetweenRange(HostShip, ship, 0, 3))
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, PerformAction);
            }
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Chewbacca: a friendly ship was destroyed you may to perform an action");

            selectedShip = Selection.ThisShip;
            selectedShip.OnCombatCheckExtraAttack += StartBonusAttack;
            Roster.HighlightPlayer(HostShip.Owner.PlayerNo);
            Selection.ChangeActiveShip(HostShip);
            performedRegularAttack = HostShip.IsAttackPerformed;
            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();
            HostShip.AskPerformFreeAction(actions, delegate {
                Roster.HighlightPlayer(selectedShip.Owner.PlayerNo);
                Selection.ChangeActiveShip(selectedShip);
                Triggers.FinishTrigger();
            });
        }

        private void StartBonusAttack(GenericShip ship)
        {
            ship.OnCombatCheckExtraAttack -= StartBonusAttack;
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, RegisterBonusAttack);
        }

        private void RegisterBonusAttack(object sender, System.EventArgs e)
        {
            if (HostShip.IsDestroyed)
            {
                Triggers.FinishTrigger();
                return;
            }

            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartAdditionalAttack(
                        HostShip,
                        CleanupBonusAttack,
                        null,
                        HostShip.PilotInfo.PilotName,
                        "Chewbacca: a friendly ship was destroyed you may to perform a bonus attack",
                        HostShip
                    );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot preform a second bonus attack", HostShip.PilotInfo.PilotName));
                CleanupBonusAttack();
            }
        }

        private void CleanupBonusAttack()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = performedRegularAttack;
            Triggers.FinishTrigger();
        }
    }
}