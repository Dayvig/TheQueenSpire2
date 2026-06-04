using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Formatters;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class BattleDance() : QueenMod2Card(-2,
    CardType.Skill, CardRarity.Rare,
    TargetType.AllEnemies)
{
    
    public List<MainFile.DanceStep> steps = new List<MainFile.DanceStep>
    {
        MainFile.DanceStep.SKILL,
        MainFile.DanceStep.POWER,
        MainFile.DanceStep.ATTACK,
    };
    public int currentStepCount = 0;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10M, ValueProp.Unpowered),
        new DanceVar(steps, currentStepCount)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(QueenMod2Keywords.Dance)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Unplayable
    ];
    
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (!play.Card.Owner.Equals(Owner) || !CardPile.GetCards(Owner, PileType.Hand).Contains(this))
            return Task.CompletedTask;
        DanceVar dance =  (DanceVar)DynamicVars["Dance"];
        switch (dance.danceSteps[dance.place])
        {
            case MainFile.DanceStep.ATTACK:
                if (play.Card.Type.Equals(CardType.Attack))
                {
                    dance.place++;
                    if (dance.place >= dance.danceSteps.Count)
                    {
                        dance.place = 0;
                        return triggerEffect(choiceContext);
                    }
                }
                break;
            case MainFile.DanceStep.SKILL:
                if (play.Card.Type.Equals(CardType.Skill))
                {
                    dance.place++;
                    if (dance.place >= dance.danceSteps.Count)
                    {
                        dance.place = 0;
                        return triggerEffect(choiceContext);
                    }
                }
                break;
            case MainFile.DanceStep.POWER:
                if (play.Card.Type.Equals(CardType.Power))
                {
                    dance.place++;
                    if (dance.place >= dance.danceSteps.Count)
                    {
                        dance.place = 0;
                        return triggerEffect(choiceContext);
                    }
                }                
                break;
        }

        return Task.CompletedTask;
    }

    public async Task triggerEffect(PlayerChoiceContext choiceContext)
    {
        BattleDance dance = this;
        AttackCommand attackCommand = await DamageCmd.Attack(dance.DynamicVars.Damage.IntValue).FromCard(dance).TargetingAllOpponents(dance.CombatState)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4M);
    }


    public List<MainFile.DanceStep> createNewSteps()
    {
        steps.Clear();
        List<MainFile.DanceStep> newSteps = new List<MainFile.DanceStep>();
        int containedPowers = 0;
        int containedSkills = 0;
        int containedAttacks = 0;

        if (RunState == null)
        {
            newSteps.Add(MainFile.DanceStep.ATTACK);
            newSteps.Add(MainFile.DanceStep.SKILL);
            newSteps.Add(MainFile.DanceStep.POWER);
            return newSteps;
        }
        
        float nextRng = RunState.Rng.Niche.NextFloat(0, 1F);
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
        nextRng = RunState.Rng.Niche.NextFloat(0, 1F);
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
                    miscRng = RunState.Rng.Niche.NextFloat(0, 1F);
                    if (miscRng <= 0.5f) { newSteps.Add(MainFile.DanceStep.ATTACK); containedAttacks++; } else { newSteps.Add(MainFile.DanceStep.SKILL); containedSkills++; }
                    break;
                }
                newSteps.Add(MainFile.DanceStep.POWER);
                containedPowers++;
                break;
        }
        nextRng = RunState.Rng.Niche.NextFloat(0, 1F);
        //Third step: 40% Skill, 40% Attack, 20% Power. If a power already exists, power chance reduced to 5%, with skill and attack boosted accordingly.
        //Additionally, if the previous two steps are the same as the third, 75% chance for the third to be the reverse of attack/skill. If two powers are previous, only 5% chance for third to be a power as well.
        switch (nextRng)
        {
            case <= 0.4f:
                miscRng = RunState.Rng.Niche.NextFloat(0, 1F);
                if (containedAttacks == 2){ if (miscRng > 0.75f){newSteps.Add(MainFile.DanceStep.ATTACK); } else { newSteps.Add(MainFile.DanceStep.SKILL); } }
                else
                {
                    newSteps.Add(MainFile.DanceStep.ATTACK);
                }
                break;
            case <= 0.8f:
                miscRng = RunState.Rng.Niche.NextFloat(0, 1F);
                if (containedAttacks == 2){if (miscRng > 0.75f){newSteps.Add(MainFile.DanceStep.SKILL); } else { newSteps.Add(MainFile.DanceStep.ATTACK); } }
                else
                {
                    newSteps.Add(MainFile.DanceStep.SKILL);
                }
                break;
            case > 0.8f:
                if (containedPowers > 0 && nextRng > 0.95f)
                {
                    miscRng = RunState.Rng.Niche.NextFloat(0, 1F);
                    if (containedPowers == 2){if (miscRng >= 0.95f) { newSteps.Add(MainFile.DanceStep.POWER); } 
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
    
    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Equals(this))
        {
            DanceVar dance =  (DanceVar)DynamicVars["Dance"];
            dance.danceSteps = createNewSteps();
            dance.place = 0;
        }
        return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }
    
    public override void AfterCreated()
    {
        base.AfterCreated();
        DanceVar dance =  (DanceVar)DynamicVars["Dance"];
        dance.danceSteps = createNewSteps();
        dance.place = 0;
    }
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add((DanceVar)DynamicVars["Dance"]);
    }
}