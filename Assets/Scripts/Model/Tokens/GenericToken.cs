using Ship;

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

        public GenericToken(GenericShip host)
        {
            Host = host;
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
