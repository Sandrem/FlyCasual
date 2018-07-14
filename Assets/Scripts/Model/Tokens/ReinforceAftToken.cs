using Ship;

namespace Tokens
{

    public class ReinforceAftToken : GenericReinforceToken
    {
        public ReinforceAftToken(GenericShip host): base(host)
        {
            Name = "Reinforce Aft Token";
            Facing = Arcs.ArcFacing.Rear180;
        }
    }

}
