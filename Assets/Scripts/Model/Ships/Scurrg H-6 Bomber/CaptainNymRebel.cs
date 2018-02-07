using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bombs;
using Abilities;
using Upgrade;
using Ship;
using SubPhases;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class CaptainNymRebel : ScurrgH6Bomber
        {
            public CaptainNymRebel() : base()
            {
                PilotName = "Captain Nym";
                PilotSkill = 8;
                Cost = 30;

                IsUnique = true;

                faction = Faction.Rebel;
                SkinName = "Captain Nym (Rebel)";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new CaptainNymRebelAbiliity());
            }
        }
    }
}

namespace Abilities
{
    public class CaptainNymRebelAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {
            BombsManager.OnCheckPermissionToDetonate += CheckCaptainNymAbility;
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            BombsManager.OnCheckPermissionToDetonate -= CheckCaptainNymAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckCaptainNymAbility(GenericBomb bomb, GenericShip detonatedShip)
        {
            if (!IsAbilityUsed && bomb.Host.Owner.PlayerNo == HostShip.Owner.PlayerNo)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCheckPermissionToDetonate, AskToUseCaptainNymAbility);
            }
        }

        private void AskToUseCaptainNymAbility(object sender, System.EventArgs e)
        {
            if (!IsAbilityUsed)
            {
                Messages.ShowInfoToHuman(string.Format("{0} token is ready for detonation", BombsManager.CurrentBomb.Name));
                AskToUseAbility(NeverUseByDefault, UseAbility);
            }
            else
            {
                Messages.ShowErrorToHuman("Captain Nym already have used his ability");
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            BombsManager.DetonationIsAllowed = false;
            IsAbilityUsed = true;

            DecisionSubPhase.ConfirmDecision();
        }
    }
}
