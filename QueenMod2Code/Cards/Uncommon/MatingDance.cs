using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Character;
using QueenMod2.QueenMod2Code.Formatters;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class MatingDance() : QueenMod2Card(-2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{

    public List<MainFile.DanceStep> steps = new List<MainFile.DanceStep>
    {
        MainFile.DanceStep.SKILL,
        MainFile.DanceStep.POWER,
        MainFile.DanceStep.ATTACK,
    };
    public int currentStepCount = 0;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("Swarm", 4M),
        new DanceVar(steps, currentStepCount)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower(ModelDb.Power<Swarm>()),
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
        await PowerCmd.Apply<Swarm>(choiceContext, Owner.Creature, DynamicVars["Swarm"].BaseValue, Owner.Creature, this, false);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Swarm"].UpgradeValueBy(2M);
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