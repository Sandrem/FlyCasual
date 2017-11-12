using Ship;

namespace Tokens
{

    public class ReinforceForeToken : GenericToken
    {
        public ReinforceForeToken(GenericShip host) : base(host)
        {
            Name = "Reinforce Fore Token";
            Action = new ActionsList.ReinforceForeAction() { Host = this.Host };
        }
    }

}
