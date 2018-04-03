using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class GenericCommand
{
    public string Keyword { get; protected set; }
    public string Description { get; protected set; }

    public abstract void Execute(Dictionary<string, string> parameters);

    protected void ShowHelp()
    {
        Console.ProcessCommand("help " + Keyword);
    }
}
