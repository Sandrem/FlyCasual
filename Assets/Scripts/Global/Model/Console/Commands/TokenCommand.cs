using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;

namespace CommandsList
{
    public class TokenCommand : GenericCommand
    {
        private Dictionary<string, Type> stringToType = new Dictionary<string, Type>()
        {
            { "focus",          typeof(FocusToken)          },
            { "evade",          typeof(EvadeToken)          },
            { "stress",         typeof(StressToken)         },
            { "targetlock",     typeof(BlueTargetLockToken) },
            { "ion",            typeof(IonToken)            },
            { "tractorbeam",    typeof(TractorBeamToken)    },
            { "jam",            typeof(JamToken)            },
            { "reinforceaft",   typeof(ReinforceAftToken)   },
            { "reinforcefore",  typeof(ReinforceForeToken)  },
            { "cloak",          typeof(CloakToken)          },
        };

        public TokenCommand()
        {
            Keyword = "token";
            Description =   "token assign id:<shipId> type:<type> [target:<targetShipId>]- assing token to ship\n" +
                            "where type: focus, evade, stress, targetlock, ion, tractorbeam, jam, reinforceaft, reinforcefore, cloak\n" +
                            "(target is used only for targetlock type)";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("assign"))
            {
                Console.ProcessCommand("help " + Keyword);
                return;
            }

            int shipId = -1;
            if (parameters.ContainsKey("id")) int.TryParse(parameters["id"], out shipId);

            int targetShipId = -1;
            if (parameters.ContainsKey("target")) int.TryParse(parameters["target"], out targetShipId);

            Type tokenType = null;
            string typeString = (parameters.ContainsKey("type")) ? parameters["type"] : null;
            if (typeString != null)
            {
                if (stringToType.ContainsKey(typeString)) tokenType = stringToType[typeString];
            }

            if (shipId != -1 && tokenType != null)
            {
                AssignToken(shipId, tokenType, targetShipId);
            }
            else
            {
                Console.ProcessCommand("help " + Keyword);
            }
        }

        private void AssignToken(int shipId, Type tokenType, int targetShipId)
        {
            GenericShip ship = Roster.AllShips.FirstOrDefault(n => n.Key == "ShipId:" + shipId).Value;

            if (ship != null)
            {
                if (tokenType != typeof(BlueTargetLockToken))
                {
                    GenericToken token = (GenericToken)System.Activator.CreateInstance(tokenType, ship);
                    ship.Tokens.AssignToken(token, ShowMessage);
                }
                else
                {
                    if (targetShipId != -1)
                    {
                        GenericShip targetShip = Roster.AllShips.FirstOrDefault(n => n.Key == "ShipId:" + targetShipId).Value;
                        if (targetShip != null)
                        {
                            Actions.AcquireTargetLock(ship, targetShip, ShowMessage, ShowErrorMessage);
                        }
                        else
                        {
                            ShowHelp();
                        }
                    }
                    else
                    {
                        ShowHelp();
                    }
                }
            }
        }

        private void ShowMessage()
        {
            Console.Write("Token command is resolved", LogTypes.Everything, true);
        }

        private void ShowErrorMessage()
        {
            Console.Write("Token command is not resolved - error occured", LogTypes.Everything, true, "red");
        }
    }
}
