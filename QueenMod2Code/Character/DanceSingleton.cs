using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Runs;

namespace QueenMod2.QueenMod2Code.Character;

public class DanceSingleton() : CustomSingletonModel(true, true)
{
    private const float attackChance = 0.45f;
    private const float skillChance = 0.45f;
    private const float powerChance = 0.1f;
    private const float duplicateRerollChance = 0.25f;
    private const float duplicatePowerChance = 0.025f; 
    
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
        //First step: 45% Skill, 45% attack, 10% Power
        switch (nextRng)
        {
            case <= attackChance:
                newSteps.Add(MainFile.DanceStep.ATTACK);
                containedAttacks++;
                break;
            case <= attackChance + skillChance:
                newSteps.Add(MainFile.DanceStep.SKILL);
                containedSkills++;
                break;
            case > attackChance + skillChance:
                newSteps.Add(MainFile.DanceStep.POWER);
                containedPowers++;
                break;
        }
        nextRng = runState.Rng.Niche.NextFloat(0, 1F);
        //Second step: 45% Skill, 45% Attack, 10% Power. If a power already exists, power chance reduced to 2.5%, with skill and attack boosted accordingly.
        switch (nextRng)
        {
            case <= attackChance:
                newSteps.Add(MainFile.DanceStep.ATTACK);
                containedAttacks++;
                break;
            case <= attackChance + skillChance:
                newSteps.Add(MainFile.DanceStep.SKILL);
                containedSkills++;
                break;
            case > attackChance + skillChance:
                if (containedPowers > 0)
                {
                    miscRng = runState.Rng.Niche.NextFloat(0, 1F);
                    if (miscRng <= duplicatePowerChance)
                    {
                        newSteps.Add(MainFile.DanceStep.POWER);
                        containedPowers++;
                    } else if (miscRng <= ((1 - duplicatePowerChance) / 2))
                    {
                        newSteps.Add(MainFile.DanceStep.SKILL); containedSkills++;
                    }
                    else
                    {
                        newSteps.Add(MainFile.DanceStep.ATTACK); containedAttacks++;
                    }
                }
                else
                {
                    newSteps.Add(MainFile.DanceStep.POWER);
                    containedPowers++;
                }
                break;
        }
        nextRng = runState.Rng.Niche.NextFloat(0, 1F);
        //Third step: 45% Skill, 45% Attack, 10% Power. If a power already exists, power chance reduced to 2.5%, with skill and attack boosted accordingly.
        //Additionally, if the previous two steps are the same as the third, 75% chance for the third to be the reverse of attack/skill. If two powers are previous, 0% chance for third to be a power as well.
        switch (nextRng)
        {
            case <= attackChance:
                miscRng = runState.Rng.Niche.NextFloat(0, 1F);
                if (containedAttacks == 2){ 
                    if (miscRng > 0.75f){newSteps.Add(MainFile.DanceStep.ATTACK); } 
                    else { newSteps.Add(MainFile.DanceStep.SKILL); } 
                }
                else
                {
                    newSteps.Add(MainFile.DanceStep.ATTACK);
                }
                break;
            case <= attackChance + skillChance:
                miscRng = runState.Rng.Niche.NextFloat(0, 1F);
                if (containedSkills == 2){
                    if (miscRng <= duplicateRerollChance){newSteps.Add(MainFile.DanceStep.SKILL); } 
                    else { newSteps.Add(MainFile.DanceStep.ATTACK); } 
                }
                else
                {
                    newSteps.Add(MainFile.DanceStep.SKILL);
                }
                break;
            case > attackChance + skillChance:
                if (containedPowers > 0)
                {
                    miscRng = runState.Rng.Niche.NextFloat(0, 1F);
                    if (containedPowers > 1)
                    {
                        miscRng += duplicatePowerChance;
                    }
                    if (miscRng < duplicatePowerChance)
                    {
                        newSteps.Add(MainFile.DanceStep.POWER);
                        containedPowers++;
                    } else if (miscRng <= ((1 - duplicatePowerChance) / 2))
                    {
                        newSteps.Add(MainFile.DanceStep.SKILL); containedSkills++;
                    }
                    else
                    {
                        newSteps.Add(MainFile.DanceStep.ATTACK); containedAttacks++;
                    }
                }
                else
                {
                    newSteps.Add(MainFile.DanceStep.POWER);
                    containedPowers++;
                }
                break;
        }

        return newSteps;
    }
}