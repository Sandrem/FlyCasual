using Bombs;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScurrgH6Bomber
    {
        public class CaptainNym : ScurrgH6Bomber
        {
            public CaptainNym() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Nym",
                    5,
                    52,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.CaptainNymScumAbiliity),
                    charges: 1,
                    regensCharges: true
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 204;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainNymScumAbiliity : Abilities.FirstEdition.CaptainNymRebelAbiliity
    {
        public override void ActivateAbility()
        {
            base.ActivateAbility();
            HostShip.AfterGotNumberOfDefenceDice += CheckBombObstruction;
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            HostShip.AfterGotNumberOfDefenceDice -= CheckBombObstruction;
        }

        private void CheckBombObstruction(ref int count)
        {
            if (Combat.ShotInfo.IsObstructedByBombToken)
            {
                Messages.ShowInfo("Captain Nym: +1 defense die");
                count++;
            }
        }

        protected override bool CanUseAbility()
        {
            return HostShip.State.Charges > 0;
        }

        protected override void MarkAbilityAsUsed()
        {
            HostShip.SpendCharge();
        }
    }
}

namespace Abilities.FirstEdition
{
    public class CaptainNymRebelAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {
            BombsManager.OnCheckPermissionToDetonate += CheckCaptainNymAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            BombsManager.OnCheckPermissionToDetonate -= CheckCaptainNymAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckCaptainNymAbility(GenericBomb bomb, GenericShip detonatedShip)
        {
            if (CanUseAbility() && bomb.Host.Owner.PlayerNo == HostShip.Owner.PlayerNo)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCheckPermissionToDetonate, AskToUseCaptainNymAbility);
            }
        }

        private void AskToUseCaptainNymAbility(object sender, System.EventArgs e)
        {
            if (CanUseAbility())
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
            MarkAbilityAsUsed();

            DecisionSubPhase.ConfirmDecision();
        }

        protected virtual bool CanUseAbility()
        {
            return !IsAbilityUsed;
        }

        protected virtual void MarkAbilityAsUsed()
        {
            IsAbilityUsed = true;
        }
    }
}