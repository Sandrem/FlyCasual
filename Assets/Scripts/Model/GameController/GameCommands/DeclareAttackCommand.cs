﻿using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Ship;
using Upgrade;

namespace GameCommands
{
    public class DeclareAttackCommand : GameCommand
    {
        public DeclareAttackCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            Combat.ChosenWeapon = GetWeaponByName(GetString("weapon"));

            Combat.DeclareIntentToAttack(
                int.Parse(GetString("id")),
                int.Parse(GetString("target")),
                bool.Parse(GetString("weaponIsAlreadySelected"))
            );
        }

        private IShipWeapon GetWeaponByName(string weaponName)
        {
            GenericShip attacker = Roster.GetShipById("ShipId:" + int.Parse(GetString("id")));

            foreach (IShipWeapon weapon in attacker.GetAllWeapons())
            {
                if (weapon.WeaponType == WeaponTypes.PrimaryWeapon)
                {
                    return weapon;
                }
                else
                {
                    GenericSpecialWeapon secUpgrade = weapon as GenericSpecialWeapon;

                    if (secUpgrade == null) continue;

                    if (!secUpgrade.State.IsFaceup) continue;
                    if (secUpgrade.State.UsesCharges && secUpgrade.State.Charges == 0) continue;

                    if (secUpgrade.UpgradeInfo.Name == weaponName) return weapon;
                }
            }

            return null;
        }
    }

}
