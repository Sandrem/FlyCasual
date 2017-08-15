using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class Howlrunner : TIEFighter
        {
            public Howlrunner() : base()
            {
                PilotName = "\"Howlrunner\"";
                ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/7/71/Howlrunner.png";
                IsUnique = true;
                PilotSkill = 8;
                Cost = 18;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                GenericShip.AfterGenerateAvailableActionEffectsListGlobal += HowlrunnerAbility;
            }

            private void HowlrunnerAbility()
            {
                Combat.Attacker.AddAvailableActionEffect(new ActionsList.HowlrunnerAction());
            }

        }
    }
}

namespace ActionsList
{

    public class HowlrunnerAction : GenericAction
    {
        public HowlrunnerAction()
        {
            Name = EffectName = "Howlrunner";
            IsReroll = true;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Combat.SecondaryWeapon == null)
                {
                    if (Combat.Attacker.GetType() != typeof(Ship.TIEFighter.Howlrunner))
                    {
                        Ship.GenericShip Howlrunner = null;
                        foreach (var friendlyShip in Combat.Attacker.Owner.Ships)
                        {
                            if (friendlyShip.Value.GetType() == typeof(Ship.TIEFighter.Howlrunner))
                            {
                                Howlrunner = friendlyShip.Value;
                                break;
                            }
                        }
                        if (Howlrunner != null)
                        {
                            Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(Howlrunner, Combat.Attacker);
                            if (positionInfo.Range == 1)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public override void ActionEffect()
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDicesCanBeRerolled = 1
            };
            diceRerollManager.Start();
        }

    }

}
