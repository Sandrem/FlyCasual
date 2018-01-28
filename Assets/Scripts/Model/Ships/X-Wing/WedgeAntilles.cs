using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;

namespace Ship
{
    namespace XWing
    {
        public class WedgeAntilles : XWing
        {
            public WedgeAntilles() : base()
            {
                PilotName = "Wedge Antilles";
                PilotSkill = 9;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.WedgeAntillesAbility());
            }
        }
    }
}

namespace Abilities
{
    public class WedgeAntillesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddWedgeAntillesAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddWedgeAntillesAbility;
        }

        public void AddWedgeAntillesAbility()
        {
            if (Selection.ThisShip.ShipId == HostShip.ShipId)
            {
                if (Combat.Defender.Agility != 0)
                {
                    Messages.ShowError("Wedge Antilles: Agility is decreased");
                    Combat.Defender.AssignToken(new Conditions.WedgeAntillesCondition(), delegate { });
                    Combat.Defender.ChangeAgilityBy(-1);
                    Combat.Defender.OnAttackFinish += RemoveWedgeAntillesAbility;
                }
            }
        }

        public void RemoveWedgeAntillesAbility(GenericShip ship)
        {
            Messages.ShowInfo("Agility is restored");
            Combat.Defender.RemoveCondition(typeof(Conditions.WedgeAntillesCondition));
            ship.ChangeAgilityBy(+1);
            ship.OnAttackFinish -= RemoveWedgeAntillesAbility;
        }
    }
}

namespace Conditions
{
    public class WedgeAntillesCondition : Tokens.GenericToken
    {
        public WedgeAntillesCondition()
        {
            Name = "Debuff Token";
            Temporary = false;
            Tooltip = new Ship.XWing.WedgeAntilles().ImageUrl;
        }
    }
}
