using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace QueenMod2.QueenMod2Code.Formatters;

public class DanceVar : DynamicVar
{
    public const string defaultName = "Dance";
    public List<MainFile.DanceStep> danceSteps = new List<MainFile.DanceStep>();
    public int place = 0;
    
    public DanceVar(List<MainFile.DanceStep> steps, int place)
        : base("Dance", (Decimal) (int) 0M)
    {
        danceSteps = steps;
        this.place = place;
        MainFile.Logger.Info("Setting Dance default Constructor");
    }
    
    public DanceVar(string name, Decimal amount)
        : base(name, amount)
    {
    }
}