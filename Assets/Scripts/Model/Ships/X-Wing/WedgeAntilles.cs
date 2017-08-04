using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class WedgeAntilles : XWing
        {
            public WedgeAntilles() : base()
            {
                IsHidden = true;

                PilotName = "Wedge Antilles";
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/8/80/Wedge-antilles.png";
                IsUnique = true;
                PilotSkill = 9;
                Cost = 29;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                OnAttack += AddWedgeAntillesAbility;
            }

            public void AddWedgeAntillesAbility()
            {
                if (Selection.ThisShip.PilotName == PilotName)
                {
                    Messages.ShowError("Wedge Antilles: Agility is decreased");
                    Selection.AnotherShip.ChangeAgilityBy(-1);
                    Selection.AnotherShip.AfterCombatEnd += RemoveWedgeAntillesAbility;
                }
            }

            public void RemoveWedgeAntillesAbility(Ship.GenericShip ship)
            {
                Messages.ShowInfo("Agility is restored");
                ship.ChangeAgilityBy(+1);
                ship.AfterCombatEnd -= RemoveWedgeAntillesAbility;
            }

        }

    }
}
