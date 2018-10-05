using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bombs;
using Abilities;
using Ship;
using ActionsList;
using Upgrade;
using RuleSets;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class CaptainNymScum : ScurrgH6Bomber, ISecondEditionPilot
        {
            public CaptainNymScum() : base()
            {
                PilotName = "Captain Nym";
                PilotSkill = 8;
                Cost = 30;

                IsUnique = true;

                SkinName = "Captain Nym (Scum)";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new CaptainNymScumAbiliity());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 52;

                UsesCharges = true;
                MaxCharges = 1;
                RegensCharges = true;

                PilotAbilities.RemoveAll(a => a is Abilities.CaptainNymScumAbiliity);
                PilotAbilities.Add(new Abilities.SecondEdition.CaptainNymScumAbiliitySE());

                SEImageNumber = 204;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainNymScumAbiliitySE : CaptainNymRebelAbiliity
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
            return HostShip.Charges > 0;
        }

        protected override void MarkAbilityAsUsed()
        {
            //Empty delegate is safe here - Sandrem
            HostShip.SpendCharge(delegate { });
        }
    }
}

namespace Abilities
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
                Combat.Defender.AddAvailableDiceModification(new CaptainNymObstructionBonus());
            }
        }

        private void CheckIgnoreMines(GenericBomb bomb, GenericShip ship)
        {
            if (ship == null || ship.ShipId != HostShip.ShipId) return;

            if (bomb.GetType().BaseType == typeof(GenericContactMine))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCheckPermissionToDetonate, AskToIgnoreContactMine);
            }
        }

        private void AskToIgnoreContactMine(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                Messages.ShowInfoToHuman(string.Format("{0} token is ready for detonation", BombsManager.CurrentBomb.Name));
                AskToUseAbility(AlwaysUseByDefault, IgnoreContactMineDecision, null, null, true);
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
            if (BombsManager.CurrentBomb.Host.Owner.PlayerNo == HostShip.Owner.PlayerNo)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCheckSufferBombDetonation, AskToIgnoreTimedBomb);
            }
        }

        private void AskToIgnoreTimedBomb(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                Messages.ShowInfoToHuman(string.Format("{0} token is ready for deal effect", BombsManager.CurrentBomb.Name));
                AskToUseAbility(AlwaysUseByDefault, IgnoreTimedBombDecision, null, null, true);
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

namespace ActionsList
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
