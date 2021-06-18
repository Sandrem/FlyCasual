using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace Abilities
{
    public class HasTokenCondition : Condition
    {
        private List<Type> AllowedTokensList;
        private List<TokenColors> AllowedTokenColors;
        private ShipRole ShipRoleToCheck;

        public HasTokenCondition(List<Type> tokensList, ShipRole shipRoleToCheck = ShipRole.CheckingShip)
        {
            AllowedTokensList = tokensList;
            ShipRoleToCheck = shipRoleToCheck;
        }

        public HasTokenCondition(Type tokenType, ShipRole shipRoleToCheck = ShipRole.CheckingShip)
        {
            AllowedTokensList = new List<Type>() { tokenType };
            ShipRoleToCheck = shipRoleToCheck;
        }

        public HasTokenCondition(List<TokenColors> tokenColors, ShipRole shipRoleToCheck = ShipRole.CheckingShip)
        {
            AllowedTokenColors = tokenColors;
            ShipRoleToCheck = shipRoleToCheck;
        }

        public HasTokenCondition(TokenColors tokenColor, ShipRole shipRoleToCheck = ShipRole.CheckingShip)
        {
            AllowedTokenColors = new List<TokenColors>() { tokenColor };
            ShipRoleToCheck = shipRoleToCheck;
        }

        public override bool Passed(ConditionArgs args)
        {
            GenericShip shipToCheck = GetShipFromRole(args);

            if (AllowedTokensList != null)
            {
                foreach (Type tokenType in AllowedTokensList)
                {
                    if (shipToCheck.Tokens.HasToken(tokenType, '*')) return true;
                }
            }

            if (AllowedTokenColors != null)
            {
                foreach (TokenColors tokenColor in AllowedTokenColors)
                {
                    if (shipToCheck.Tokens.HasTokenByColor(tokenColor)) return true;
                }
            }

            return false;
        }

        private GenericShip GetShipFromRole(ConditionArgs args)
        {
            switch (ShipRoleToCheck)
            {
                case ShipRole.CheckingShip:
                    return args.ShipToCheck;
                case ShipRole.HostShip:
                    return args.ShipAbilityHost;
                default:
                    Debug.LogError("Unknown role");
                    return null;
            }

        }
    }
}
