using Ship;
using Upgrade;
using System.Linq;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class Zuckuss : GenericUpgrade
    {
        public Zuckuss() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Zuckuss",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.ZuckussCrewAbility),
                seImageNumber: 138
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class ZuckussCrewAbility : GenericAbility
    {
        //While you perform an attack, if you are not stressed, you may choose 1 defense die and gain 1 stress token. If you do, the defender must reroll that die.

        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite += ZuckussAbilityEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite -= ZuckussAbilityEffect;
        }

        private void ZuckussAbilityEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.ZuckussActionEffect()
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
    public class ZuckussActionEffect : GenericAction
    {

        public ZuckussActionEffect()
        {
            Name = DiceModificationName = "Zuckuss Ability";
            DiceModificationTiming = DiceModificationTimingType.Opposite;
        }

        public override int GetDiceModificationPriority()
        {
            return 80;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && !HostShip.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                IsOpposite = true,
                CallBack = delegate {
                    AssignStress(callBack);
                }
            };
            diceRerollManager.Start();
        }

        private void AssignStress(System.Action callBack)
        {
            HostShip.Tokens.AssignToken(typeof(StressToken), callBack);
        }
    }
}