using RuleSets;
using Ship;
using System.Linq;

namespace Ship
{
    namespace Firespray31
    {
        public class BobaFettSV : Firespray31, ISecondEditionPilot
        {
            public BobaFettSV() : base()
            {
                PilotName = "Boba Fett";
                PilotSkill = 8;
                Cost = 39;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                faction = Faction.Scum;

                SkinName = "Boba Fett";

                PilotAbilities.Add(new Abilities.BobaFettSVAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 80;

                ImageUrl = "https://i.imgur.com/Q64XbW2.png";
            }
        }
    }
}

namespace Abilities
{
    public class BobaFettSVAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += AddBobaFettSVActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddBobaFettSVActionEffect;
        }

        private void AddBobaFettSVActionEffect(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.BobaFettSVActionEffect();
            newAction.Host = host;
            newAction.ImageUrl = host.ImageUrl;
            host.AddAvailableActionEffect(newAction);
        }
    }
}

namespace ActionsList
{

    public class BobaFettSVActionEffect : GenericAction
    {
        public BobaFettSVActionEffect()
        {
            Name = EffectName = "Boba Fett";

            // Used for abilities like Dark Curse's that can prevent rerolls
            IsReroll = true;
        }

        private int getDices()
        {
            int dices = Roster.AllShips.Values.Where(ship => FilterTargets(ship)).Count();
            return dices;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack) ||
                (Combat.AttackStep == CombatStep.Defence))
            {
                if (getDices() > 0) result = true;
            }
            return result;
        }

        public override int GetActionEffectPriority()
        {
            return 90;
        }

        private bool FilterTargets(GenericShip ship)
        {
            //Filter other friendly ships range 1
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Host, ship);
            return ship.Owner.PlayerNo != Host.Owner.PlayerNo &&
                    ship != Host &&
                    distanceInfo.Range == 1;
        }

        public override void ActionEffect(System.Action callBack)
        {
            int dices = getDices();
            if (dices > 0)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = dices,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }
        }

    }

}