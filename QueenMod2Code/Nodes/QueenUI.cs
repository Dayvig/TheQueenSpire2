using Godot;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using QueenMod2.QueenMod2Code.Extensions;

namespace QueenMod2.QueenMod2Code.Nodes;

public partial class QueenUI : NGlobalUi
{
    public NHiveBoundPreviewContainer HiveBoundPreviewContainer { get; private set; }

    public static QueenUI Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        MainFile.Logger.Info("QueenUI Initiated: "+Instance.Name.ToString());
        this.HiveBoundPreviewContainer = this.GetNode<NHiveBoundPreviewContainer>((NodePath) "%HiveBoundPreviewContainer");
        MainFile.Logger.Info("HiveBoundPreviewContainer Initiated: "+HiveBoundPreviewContainer.Name.ToString());
    }

}