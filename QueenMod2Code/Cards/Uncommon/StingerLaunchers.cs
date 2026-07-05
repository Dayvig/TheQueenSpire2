using GodotPlugins.Game;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class StingerLaunchers() : QueenMod2Card(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3M, ValueProp.Move),
        new RepeatVar(3)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        QueenMod2Keywords.Hivebound
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        StingerLaunchers stingers = this;
        AttackCommand attackCommand = await DamageCmd.Attack(stingers.DynamicVars.Damage.IntValue).FromCard(stingers, play).WithHitCount(stingers.DynamicVars.Repeat.IntValue).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }
    
    public override Task AfterAutoPrePlayPhaseEnteredLate(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (CardPile.GetCards(this.Owner, PileType.Draw).Contains(this))
        {
            return triggerHiveBoundEffect(choiceContext);
        }
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1M);
    }

    public async Task triggerHiveBoundEffect(PlayerChoiceContext choiceContext)
    {
        StingerLaunchers stingers = this;
        //CardCmd.Preview(stingers, 0.4f, CardPreviewStyle.GridLayout);
        AttackCommand attackCommand = await DamageCmd.Attack(stingers.DynamicVars.Damage.IntValue).FromCard(stingers, null).TargetingRandomOpponents(stingers.CombatState)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }
}