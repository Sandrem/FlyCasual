using Bombs;
using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Captain Nym",
                    "Captain of the Lok Revenants",
                    Faction.Scum,
                    5,
                    6,
                    17,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainNymScumAbiliity),
                    charges: 1,
                    regensCharges: 1,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Turret,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Title
                    },
                    seImageNumber: 204,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
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
                Messages.ShowInfo("The attack is obstructed by a bomb token. " + HostShip.PilotInfo.PilotName + " gains +1 defense die");
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
            if (CanUseAbility() && bomb.HostShip.Owner.PlayerNo == HostShip.Owner.PlayerNo)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCheckPermissionToDetonate, AskToUseCaptainNymAbility);
            }
        }

        private void AskToUseCaptainNymAbility(object sender, System.EventArgs e)
        {
            if (CanUseAbility())
            {
                Messages.ShowInfoToHuman(string.Format("{0} token is ready for detonation", BombsManager.CurrentDevice.UpgradeInfo.Name));
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    NeverUseByDefault,
                    UseAbility,
                    descriptionLong: "Do you want to prevent a friendly bomb from detonating?",
                    imageHolder: HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman("Captain Nym has already used his ability");
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