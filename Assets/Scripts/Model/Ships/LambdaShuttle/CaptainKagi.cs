namespace Ship
{
    namespace LambdaShuttle
    {
        public class CaptainKagi : LambdaShuttle
        {
            public CaptainKagi() : base()
            {
                PilotName = "Captain Kagi";
                PilotSkill = 8;
                Cost = 27;
                IsUnique = true;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed += CanPerformTargetLock;
                OnDestroyed += RemoveCaptainKagiAbility;
            }


            public void CanPerformTargetLock(ref bool result, GenericShip attacker, GenericShip defender)
            {
                bool abilityIsActive = false;
                if (defender.ShipId != this.ShipId)
                {
                    if (defender.Owner.PlayerNo == this.Owner.PlayerNo)
                    {
                        Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(attacker, this);
                        if (positionInfo.Range >= attacker.TargetLockMinRange && positionInfo.Range <= attacker.TargetLockMaxRange)
                        {
                            abilityIsActive = true;                            
                        }
                    }
                }

                if (abilityIsActive)
                {
                    if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
                    {
                        Messages.ShowErrorToHuman("Captain Kagi: You cannot target lock that ship");
                    }
                    result = false;
                }                
            }

            private void RemoveCaptainKagiAbility(GenericShip ship)
            {
                RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed -= CanPerformTargetLock;
                OnDestroyed -= RemoveCaptainKagiAbility;
            }
        }
    }
}
