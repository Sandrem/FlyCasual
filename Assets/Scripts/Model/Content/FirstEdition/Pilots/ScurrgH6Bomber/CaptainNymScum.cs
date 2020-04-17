﻿using ActionsList.FirstEdition;
using Bombs;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.ScurrgH6Bomber
    {
        public class CaptainNymScum : ScurrgH6Bomber
        {
            public CaptainNymScum() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Nym",
                    8,
                    30,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.CaptainNymScumAbiliity),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Captain Nym (Scum)";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class CaptainNymScumAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {
            BombsManager.OnCheckPermissionToDetonate += CheckIgnoreMines;
            GenericShip.OnGenerateDiceModificationsGlobal += CheckObstructionBonus;
            HostShip.OnCheckSufferBombDetonation += CheckIgnoreTimedBombs;
        }

        public override void DeactivateAbility()
        {
            BombsManager.OnCheckPermissionToDetonate -= CheckIgnoreMines;
            GenericShip.OnGenerateDiceModificationsGlobal -= CheckObstructionBonus;
            HostShip.OnCheckSufferBombDetonation -= CheckIgnoreTimedBombs;
        }

        private void CheckObstructionBonus(GenericShip ship)
        {
            if (Combat.Defender.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            if (Combat.AttackStep != CombatStep.Defence) return;

            if (Combat.ShotInfo.IsObstructedByBombToken)
            {
                Combat.Defender.AddAvailableDiceModification(new CaptainNymObstructionBonus(), HostShip);
            }
        }

        private void CheckIgnoreMines(GenericBomb bomb, GenericShip ship)
        {
            if (ship == null || ship.ShipId != HostShip.ShipId) return;

            if (bomb.GetType().BaseType == typeof(GenericContactMineFE))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCheckPermissionToDetonate, AskToIgnoreContactMine);
            }
        }

        private void AskToIgnoreContactMine(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                Messages.ShowInfoToHuman(string.Format("{0} token is ready for detonation", BombsManager.CurrentDevice.UpgradeInfo.Name));
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    IgnoreContactMineDecision,
                    descriptionLong: "Do you want to ignore this friendly bomb?",
                    imageHolder: HostShip,
                    showAlwaysUseOption: true
                );
            }
            else
            {
                IgnoreContactMine(Triggers.FinishTrigger);
            }
        }

        private void IgnoreContactMineDecision(object sender, System.EventArgs e)
        {
            IgnoreContactMine(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void IgnoreContactMine(System.Action callback)
        {
            BombsManager.DetonationIsAllowed = false;
            callback();
        }

        private void CheckIgnoreTimedBombs(GenericShip detonatedShip)
        {
            if (BombsManager.CurrentDevice.HostShip.Owner.PlayerNo == HostShip.Owner.PlayerNo)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCheckSufferBombDetonation, AskToIgnoreTimedBomb);
            }
        }

        private void AskToIgnoreTimedBomb(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                Messages.ShowInfoToHuman(string.Format("{0} token is ready for deal effect", BombsManager.CurrentDevice.UpgradeInfo.Name));
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    IgnoreTimedBombDecision,
                    descriptionLong: "Do you want to ignore detonation of this friendly bomb?",
                    imageHolder: HostShip,
                    showAlwaysUseOption: true
                );
            }
            else
            {
                IgnoreTimedBomb(Triggers.FinishTrigger);
            }
        }

        private void IgnoreTimedBombDecision(object sender, System.EventArgs e)
        {
            IgnoreTimedBomb(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void IgnoreTimedBomb(System.Action callback)
        {
            HostShip.IgnoressBombDetonationEffect = true;
            callback();
        }
    }
}

namespace ActionsList.FirstEdition
{
    public class CaptainNymObstructionBonus : GenericAction
    {
        public CaptainNymObstructionBonus()
        {
            Name = DiceModificationName = "Captain Nym: Free Evade";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ApplyEvade();
            callBack();
        }

        public override int GetDiceModificationPriority()
        {
            return 110;
        }
    }
}
