using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace Ship
{
    namespace XWing
    {
        public class WedgeAntilles : XWing
        {
            public WedgeAntilles() : base()
            {
                PilotName = "Wedge Antilles";
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/8/80/Wedge-antilles.png";
                PilotSkill = 9;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new PilotAbilitiesNamespace.WedgeAntillesAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class WedgeAntillesAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Host.OnAttack += AddWedgeAntillesAbility;
        }

        public void AddWedgeAntillesAbility()
        {
            if (Selection.ThisShip.ShipId == Host.ShipId)
            {
                Messages.ShowError("Wedge Antilles: Agility is decreased");
                Selection.AnotherShip.ChangeAgilityBy(-1);
                Selection.AnotherShip.AfterCombatEnd += RemoveWedgeAntillesAbility;
            }
        }

        public void RemoveWedgeAntillesAbility(GenericShip ship)
        {
            Messages.ShowInfo("Agility is restored");
            ship.ChangeAgilityBy(+1);
            ship.AfterCombatEnd -= RemoveWedgeAntillesAbility;
        }
    }
}
