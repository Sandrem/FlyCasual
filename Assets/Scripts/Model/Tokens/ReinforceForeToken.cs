using Ship;

namespace Tokens
{

    public class ReinforceForeToken : GenericReinforceToken
    {
        public ReinforceForeToken(GenericShip host) : base(host)
        {
            Name = "Reinforce Fore Token";
            Facing = Arcs.ArcFacing.Front180;
        }
    }

}
