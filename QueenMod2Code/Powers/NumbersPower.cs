using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using QueenMod2.QueenMod2Code.Cards.Uncommon;
using QueenMod2.QueenMod2Code.Extensions;

namespace QueenMod2.QueenMod2Code.Powers;

public class NumbersPower : CustomTemporaryPowerModelWrapper<StrengthInNumbers, StrengthPower>
{
    public override AbstractModel OriginModel
    {
        get => ModelDb.GetById<AbstractModel>(ModelDb.GetId<StrengthInNumbers>());
    }
    
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}
