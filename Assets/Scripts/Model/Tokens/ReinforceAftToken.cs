using Ship;

namespace Tokens
{

    public class ReinforceAftToken : GenericReinforceToken
    {
        public ReinforceAftToken(GenericShip host): base(host)
        {
            Name = ImageName = "Reinforce Aft Token";
            Facing = Arcs.ArcFacing.FullRear;
        }
    }

}
