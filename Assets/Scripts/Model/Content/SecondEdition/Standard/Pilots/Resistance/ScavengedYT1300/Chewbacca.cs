using ActionsList;
using BoardTools;
using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Chewbacca",
                    "Loyal Companion",
                    Faction.Resistance,
                    4,
                    7,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ChewbaccaPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Title,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    }
                );

                PilotNameCanonical = "chewbacca-scavengedyt1300";
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
            selectedShip = Selection.ThisShip;
            selectedShip.OnCombatCheckExtraAttack += StartBonusAttack;
            Roster.HighlightPlayer(HostShip.Owner.PlayerNo);
            Selection.ChangeActiveShip(HostShip);
            performedRegularAttack = HostShip.IsAttackPerformed;
            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();

            CameraScript.RestoreCamera();

            HostShip.AskPerformFreeAction(
                actions, 
                delegate {
                    Roster.HighlightPlayer(selectedShip.Owner.PlayerNo);
                    Selection.ChangeActiveShip(selectedShip);
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "After a friendly ship at range 0-3 is destroyed, you may perform an action",
                HostShip
            );
        }

        private void StartBonusAttack(GenericShip ship)
        {
            ship.OnCombatCheckExtraAttack -= StartBonusAttack;
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, RegisterBonusAttack);
        }

        private void RegisterBonusAttack(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    CleanupBonusAttack,
                    null,
                    HostShip.PilotInfo.PilotName,
                    "A friendly ship was destroyed - you may perform a bonus attack",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot perform second bonus attack", HostShip.PilotInfo.PilotName));
                CleanupBonusAttack();
            }
        }

        private void CleanupBonusAttack()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = performedRegularAttack;
            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;
            Triggers.FinishTrigger();
        }
    }
}