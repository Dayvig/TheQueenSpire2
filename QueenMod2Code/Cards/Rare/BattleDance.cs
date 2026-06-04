using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Character;
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
    
    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Equals(this))
        {
            DanceVar dance =  (DanceVar)DynamicVars["Dance"];
            dance.danceSteps = DanceSingleton.createNewSteps(dance.danceSteps, RunState);
            dance.place = 0;
        }
        return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }
    
    public override void AfterCreated()
    {
        base.AfterCreated();
        DanceVar dance =  (DanceVar)DynamicVars["Dance"];
        dance.danceSteps = DanceSingleton.createNewSteps(dance.danceSteps, RunState);
        dance.place = 0;
    }
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add((DanceVar)DynamicVars["Dance"]);
    }
}