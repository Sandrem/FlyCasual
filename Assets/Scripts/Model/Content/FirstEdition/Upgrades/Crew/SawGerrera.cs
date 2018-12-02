using Ship;
using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class SawGerrera : GenericUpgrade
    {
        public SawGerrera() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Saw Gerrera",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.SawGerreraCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(7, 2));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class SawGerreraCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddSawGerreraDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddSawGerreraDiceModification;
        }

        private void AddSawGerreraDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SawGerreraCrewDiceModification();
            newAction.ImageUrl = HostShip.ImageUrl;
            newAction.Host = HostShip;
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{

    public class SawGerreraCrewDiceModification : GenericAction
    {

        public SawGerreraCrewDiceModification()
        {
            Name = DiceModificationName = "Saw Gerrera's ability";

            IsTurnsAllFocusIntoSuccess = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack && Host.State.HullCurrent > 1) result = true;
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.Focuses;
                if (attackFocuses > 1) result = 60;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Host.AssignedDamageDiceroll.AddDice(DieSide.Success);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Saw Gerrera's damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = Selection.ThisShip.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = this,
                    DamageType = DamageTypes.CardAbility
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, delegate { ChangeEyesToCritsAndFinish(callBack); });
        }

        private void ChangeEyesToCritsAndFinish(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus, DieSide.Crit);
            callBack();
        }

    }

}