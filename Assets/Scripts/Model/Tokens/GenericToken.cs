using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Tokens
{
    public enum TokenColors
    {
        Green,
        Orange,
        Red,
        Blue,
        Empty
    }

    public class GenericToken
    {
        public string Name;
        public GenericShip Host;
        public bool Temporary = true;
        public ActionsList.GenericAction Action = null;
        public bool CanBeUsed = true;
        public string Tooltip;
        public TokenColors TokenColor = TokenColors.Empty;
        public Type TooltipType;
        public int PriorityUI { get; protected set; }

        public GenericToken(GenericShip host)
        {
            Host = host;
        }

        public void InitializeTooltip()
        {
            if (TooltipType != null)
            {
                Object tooltipHolder = Activator.CreateInstance(TooltipType);
                if (tooltipHolder is GenericShip)
                {
                    GenericShip pilot = tooltipHolder as GenericShip;
                    Editions.Edition.Current.AdaptPilotToRules(pilot);
                    Tooltip = pilot.ImageUrl;
                }
                else if (tooltipHolder is GenericUpgrade)
                {
                    GenericUpgrade upgrade = tooltipHolder as GenericUpgrade;
                    Editions.Edition.Current.AdaptUpgradeToRules(upgrade);
                    Tooltip = upgrade.ImageUrl;
                }
            }
        }

        public virtual void WhenAssigned() { }

        public virtual void WhenRemoved() { }

        public virtual ActionsList.GenericAction GetAvailableEffects()
        {
            ActionsList.GenericAction result = null;
            if ((Action != null) && (CanBeUsed))
            {
                result = Action;
            }
            return result;
        }

    }

    //Consider two tokens to be equal if they belong to the same ship and are of the same type
    //Warning: Not sufficient to compare target locks!
    public class TokenComparer : IEqualityComparer<GenericToken>
    {
        public bool Equals(GenericToken x, GenericToken y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Name == y.Name && x.Host == y.Host;
        }

        public int GetHashCode(GenericToken token)
        {
            if (Object.ReferenceEquals(token, null)) return 0;

            int hashName = token.Name == null ? 0 : token.Name.GetHashCode();

            int hashHostShip = token.Host == null ? 0 : token.Host.ShipId.GetHashCode();

            return hashName ^ hashHostShip;
        }
    }
}


