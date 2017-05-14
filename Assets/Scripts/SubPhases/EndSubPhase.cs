using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSubPhase : GenericSubPhase
{

    public override void StartSubPhase()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Name = "Setup SubPhase";
        Game.UI.AddTestLogEntry(Name);

        NextSubPhase();
    }

    public override void NextSubPhase()
    {
        Game.PhaseManager.CurrentPhase.NextPhase();
    }

}
