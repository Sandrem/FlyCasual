using Upgrade;
using Ship;
using Tokens;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class SawGerreraCrew : GenericUpgrade
    {
        public SawGerreraCrew() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Saw Gerrera",
                UpgradeType.Crew,
                cost: 9,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.SawGerreraCrewAbility),
                seImageNumber: 93
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(384, 0),
                new Vector2(200, 200)
            );
        }        
    }
}

namespace Abilities.SecondEdition
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
            newAction.HostShip = HostShip;
            host.AddAvailableDiceModificationOwn(newAction);
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

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.Focuses;
                if (attackFocuses > 0) result = true;
            }

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
            HostShip.AssignedDamageDiceroll.AddDice(DieSide.Success);

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