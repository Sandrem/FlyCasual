using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DamageDeckCardFE;
using DamageDeckCardSE;
using Ship;
using Players;

namespace CommandsList
{
    public class CritCardCommand : GenericCommand
    {
        private Dictionary<string, Type> stringToTypeFE = new Dictionary<string, Type>()
        {
            { "blindedpilot",       typeof(DamageDeckCardFE.BlindedPilot)        },
            { "consolefire",        typeof(DamageDeckCardFE.ConsoleFire)         },
            { "damagedcockpit",     typeof(DamageDeckCardFE.DamagedCockpit)      },
            { "damagedengine",      typeof(DamageDeckCardFE.DamagedEngine)       },
            { "damagedsensorarray", typeof(DamageDeckCardFE.DamagedSensorArray)  },
            { "directhit",          typeof(DamageDeckCardFE.DirectHit)           },
            { "loosestabilizer",    typeof(DamageDeckCardFE.LooseStabilizer)     },
            { "majorexplosion",     typeof(DamageDeckCardFE.MajorExplosion)      },
            { "majorhullbreach",    typeof(DamageDeckCardFE.MajorHullBreach)     },
            { "shakenpilot",        typeof(DamageDeckCardFE.ShakenPilot)         },
            { "structuraldamage",   typeof(DamageDeckCardFE.StructuralDamage)    },
            { "stunnedpilot",       typeof(DamageDeckCardFE.StunnedPilot)        },
            { "thrustcontrolfire",  typeof(DamageDeckCardFE.ThrustControlFire)   },
            { "weaponsfailure",     typeof(DamageDeckCardFE.WeaponsFailure)      }
        };
        private Dictionary<string, Type> stringToTypeSE = new Dictionary<string, Type>()
        {
            { "blindedpilot",       typeof(DamageDeckCardSE.BlindedPilot)        },
            { "consolefire",        typeof(DamageDeckCardSE.ConsoleFire)         },
            { "damagedengine",      typeof(DamageDeckCardSE.DamagedEngine)       },
            { "damagedsensorarray", typeof(DamageDeckCardSE.DamagedSensorArray)  },            
            { "directhit",          typeof(DamageDeckCardSE.DirectHit)           },
            { "disabledpowerregulator", typeof(DamageDeckCardSE.DisabledPowerRegulator)},
            { "fuelleak",           typeof(DamageDeckCardSE.FuelLeak)            },
            { "hullbreach",         typeof(DamageDeckCardSE.HullBreach)          },
            { "loosestabilizer",    typeof(DamageDeckCardSE.LooseStabilizer)     },
            { "panickedpilot",      typeof(DamageDeckCardSE.PanickedPilot)       },
            { "structuraldamage",   typeof(DamageDeckCardSE.StructuralDamage)    },
            { "stunnedpilot",       typeof(DamageDeckCardSE.StunnedPilot)        },            
            { "weaponsfailure",     typeof(DamageDeckCardSE.WeaponsFailure)      },
            { "woundedpilot",       typeof(DamageDeckCardSE.WoundedPilot)        }
        };

        private Dictionary<string, Type> stringToType
        {
            get
            {
                return Editions.Edition.Current is Editions.FirstEdition
                    ? stringToTypeFE
                    : stringToTypeSE;
            }
        }

        public CritCardCommand()
        {
            Keyword = "faceupcrit";
            var critList = stringToType.Keys.ToArray();
            var critListString = string.Join(", ", critList);
            Description =   "Deals faceup damage card to ship\n" +
                            "faceupcrit id:<shipId> type:<critCardName>\n" +
                            "where critcardname: " + critListString;

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            int shipId = -1;
            if (parameters.ContainsKey("id")) int.TryParse(parameters["id"], out shipId);

            Type critType = null;
            string typeString = (parameters.ContainsKey("type")) ? parameters["type"] : null;
            if (typeString != null)
            {
                if (stringToType.ContainsKey(typeString)) critType = stringToType[typeString];
            }

            if (shipId != -1 && typeString != null)
            {
                RegisterDealFaceupCrit(shipId, critType);
            }
            else
            {
                ShowHelp();
            }
        }

        private void RegisterDealFaceupCrit(int shipId, Type critType)
        {
            GenericShip ship = Roster.AllShips.FirstOrDefault(n => n.Key == "ShipId:" + shipId).Value;

            if (ship != null)
            {
                GenericDamageCard critCard = (GenericDamageCard)System.Activator.CreateInstance(critType);
                DamageDecks.GetDamageDeck(ship.Owner.PlayerNo).PutOnTop(critCard);

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Console: dead faceup crit card",
                    TriggerType = TriggerTypes.OnAbilityDirect,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = delegate
                    {
                        ship.SufferHullDamage(
                            true,
                            new DamageSourceEventArgs
                            {
                                Source = "Console",
                                DamageType = DamageTypes.Console
                            }
                        );
                    },
                });

                Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, ShowMessage);
            }
            else
            {
                ShowHelp();
            }
        }

        private void ShowMessage()
        {
            Console.Write("FaceupCrit command is resolved", LogTypes.Everything, true);
        }

    }
}
