using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Runs;

namespace QueenMod2.QueenMod2Code.Character;

public class DanceSingleton() : CustomSingletonModel(true, true)
{
    public static List<MainFile.DanceStep> createNewSteps(List<MainFile.DanceStep> steps, IRunState runState)
    {
        steps.Clear();
        List<MainFile.DanceStep> newSteps = new List<MainFile.DanceStep>();
        int containedPowers = 0;
        int containedSkills = 0;
        int containedAttacks = 0;

        if (runState == null)
        {
            newSteps.Add(MainFile.DanceStep.ATTACK);
            newSteps.Add(MainFile.DanceStep.SKILL);
            newSteps.Add(MainFile.DanceStep.POWER);
            return newSteps;
        }
        
        float nextRng = runState.Rng.Niche.NextFloat(0, 1F);
        float miscRng;
        //First step: 40% Skill, 40% attack, 20% Power
        switch (nextRng)
        {
            case <=0.4f:
                newSteps.Add(MainFile.DanceStep.ATTACK);
                containedAttacks++;
                break;
            case <= 0.8f:
                newSteps.Add(MainFile.DanceStep.SKILL);
                containedSkills++;
                break;
            case > 0.8f:
                newSteps.Add(MainFile.DanceStep.POWER);
                containedPowers++;
                break;
        }
        nextRng = runState.Rng.Niche.NextFloat(0, 1F);
        //Second step: 40% Skill, 40% Attack, 20% Power. If a power already exists, power chance reduced to 5%, with skill and attack boosted accordingly.
        switch (nextRng)
        {
            case <= 0.4f:
                newSteps.Add(MainFile.DanceStep.ATTACK);
                containedAttacks++;
                break;
            case <= 0.8f:
                newSteps.Add(MainFile.DanceStep.SKILL);
                containedSkills++;
                break;
            case > 0.8f:
                if (containedPowers > 0 && nextRng > 0.95f)
                {
                    miscRng = runState.Rng.Niche.NextFloat(0, 1F);
                    if (miscRng <= 0.05f) { newSteps.Add(MainFile.DanceStep.ATTACK); containedAttacks++; } else { newSteps.Add(MainFile.DanceStep.SKILL); containedSkills++; }
                    break;
                }
                newSteps.Add(MainFile.DanceStep.POWER);
                containedPowers++;
                break;
        }
        nextRng = runState.Rng.Niche.NextFloat(0, 1F);
        //Third step: 40% Skill, 40% Attack, 20% Power. If a power already exists, power chance reduced to 5%, with skill and attack boosted accordingly.
        //Additionally, if the previous two steps are the same as the third, 75% chance for the third to be the reverse of attack/skill. If two powers are previous, only 5% chance for third to be a power as well.
        switch (nextRng)
        {
            case <= 0.4f:
                miscRng = runState.Rng.Niche.NextFloat(0, 1F);
                if (containedAttacks == 2){ if (miscRng > 0.75f){newSteps.Add(MainFile.DanceStep.ATTACK); } else { newSteps.Add(MainFile.DanceStep.SKILL); } }
                else
                {
                    newSteps.Add(MainFile.DanceStep.ATTACK);
                }
                break;
            case <= 0.8f:
                miscRng = runState.Rng.Niche.NextFloat(0, 1F);
                if (containedAttacks == 2){if (miscRng > 0.75f){newSteps.Add(MainFile.DanceStep.SKILL); } else { newSteps.Add(MainFile.DanceStep.ATTACK); } }
                else
                {
                    newSteps.Add(MainFile.DanceStep.SKILL);
                }
                break;
            case > 0.8f:
                if (containedPowers > 0 && nextRng > 0.95f)
                {
                    miscRng = runState.Rng.Niche.NextFloat(0, 1F);
                    if (containedPowers == 2){if (miscRng <= 0.05f) { newSteps.Add(MainFile.DanceStep.POWER); } 
                        else if (miscRng > 0.475f){ newSteps.Add(MainFile.DanceStep.SKILL); } 
                        else {newSteps.Add(MainFile.DanceStep.ATTACK);} 
                    }
                    else
                    {
                        newSteps.Add(MainFile.DanceStep.POWER);
                    }
                    break;
                }
                newSteps.Add(MainFile.DanceStep.POWER);
                break;
        }

        return newSteps;
    }
}