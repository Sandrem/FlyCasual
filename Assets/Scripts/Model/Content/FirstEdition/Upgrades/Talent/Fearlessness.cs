using Ship;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;
using BoardTools;

namespace UpgradesList.FirstEdition
{
    public class Fearlessness : GenericUpgrade
    {
        public Fearlessness() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Fearlessness",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.FearlessnessAbility),
                restriction: new FactionRestriction(Faction.Scum)
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(80, 0));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class FearlessnessAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += FearlessnessAddDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= FearlessnessAddDiceModification;
        }

        protected virtual void FearlessnessAddDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.FearlessnessAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip
            };
            HostShip.AddAvailableDiceModification(newAction);
        }

    }
}

namespace ActionsList
{

    public class FearlessnessAction : GenericAction
    {

        public FearlessnessAction()
        {
            Name = DiceModificationName = "Fearlessness";
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (!Combat.ShotInfo.InArc) return false;

            ShotInfo reverseShotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
            if (!reverseShotInfo.InArc || reverseShotInfo.Range != 1) return false;

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            return 110;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.AddDice(DieSide.Success).ShowWithoutRoll();
            Combat.CurrentDiceRoll.OrganizeDicePositions();
            callBack();
        }

    }

}