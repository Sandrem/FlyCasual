using Ship;
using System;
using Upgrade;

namespace Tokens
{
    public enum TokenColors
    {
        Green,
        Yellow,
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
                    RuleSets.Edition.Instance.AdaptPilotToRules(pilot);
                    Tooltip = pilot.ImageUrl;
                }
                else if (tooltipHolder is GenericUpgrade)
                {
                    GenericUpgrade upgrade = tooltipHolder as GenericUpgrade;
                    RuleSets.Edition.Instance.AdaptUpgradeToRules(upgrade);
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

}
