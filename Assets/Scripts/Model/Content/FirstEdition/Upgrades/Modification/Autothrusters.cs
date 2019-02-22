using ActionsList;
using Ship;
using SquadBuilderNS;
using Upgrade;
using Actions;

namespace UpgradesList.FirstEdition
{
    public class Autothrusters : GenericUpgrade
    {
        public Autothrusters() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Autothrusters",
                UpgradeType.Modification,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.AutothrustersAbility),
                restriction: new ActionBarRestriction(typeof(BoostAction))
            );
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            // TODO

            bool result = false;

            result = IsAllowedForShip(HostShip);
            if (!result) Messages.ShowError("Autothrusters can be installed only if ship has Boost action icon");

            return result;
        }
    }
}

namespace Abilities.FirstEdition
{
    public class AutothrustersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += TryAddAutothrustersDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= TryAddAutothrustersDiceModification;
        }

        private void TryAddAutothrustersDiceModification(GenericShip host)
        {
            GenericAction newAction = new AutothrustersDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = host
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{
    public class AutothrustersDiceModification : GenericAction
    {
        public AutothrustersDiceModification()
        {
            Name = DiceModificationName = "Autothrusters";
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.CurrentDiceRoll.Blanks > 0) result = 100;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (Combat.ShotInfo.InArc)
                {
                    if (Combat.ShotInfo.Range > 2)
                    {
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);

            callBack();
        }
    }

}