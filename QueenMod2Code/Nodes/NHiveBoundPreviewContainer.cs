using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace QueenMod2.QueenMod2Code.Extensions;

public partial class NHiveBoundPreviewContainer : Control
{
    private int? _forcedMaxColumns;

  public override void _Ready()
  {
    long num1 = (long) this.Connect(Node.SignalName.ChildEnteredTree, Callable.From<Node>((Action<Node>) new Action<Node>(this.ReformatElements)));
    long num2 = (long) this.Connect(Node.SignalName.ChildExitingTree, Callable.From<Node>((Action<Node>) new Action<Node>(this.CheckAnyChildrenPresent)));
  }

  /// <summary>
  /// Enforces a max column count on the grid container.
  /// Call this before adding elements to the container. Once all cards added have disappeared, the count will reset.
  /// </summary>
  public void ForceMaxColumnsUntilEmpty(int maxColumns)
  {
    this._forcedMaxColumns = new int?(maxColumns);
  }

  private void ReformatElements(Node _)
  {
    Vector2 vector2_1 = this.Size / 2f + Vector2.Down * 50f;
    int childCount = this.GetChildCount();
    int val1 = Mathf.FloorToInt((this.GetViewportRect().Size.X * 0.25f) / (NCard.defaultSize.X + (25f * 0.25f)));
    int num1 = Mathf.CeilToInt((float) childCount / (float) val1);
    if (this._forcedMaxColumns.HasValue)
      val1 = Math.Min(val1, this._forcedMaxColumns.Value);
    for (int idx = 0; idx < childCount; ++idx)
    {
      int num2 = idx / val1;
      int num3 = idx % val1;
      int num4 = Math.Min(val1, childCount - num2 * val1);
      float num5 = (float) ((double) -(num1 - 1) * ((double) NCard.defaultSize.Y + 25.0) * 0.5);
      float num6 = (float) ((double) -(num4 - 1) * ((double) NCard.defaultSize.X + 25.0) * 0.5);
      Vector2 vector2_2 = vector2_1 + new Vector2(num6 + (NCard.defaultSize.X + 25f) * (float) num3, num5 + (NCard.defaultSize.Y + 25f) * (float) num2 * 0.25f);
      Node child = this.GetChild(idx);
      if (child is Node2D node2D)
        node2D.Position = vector2_2;
      else
        ((Control) child).Position = vector2_2;
    }
  }

  private void CheckAnyChildrenPresent(Node _)
  {
    if (this.GetChildCount() != 0)
      return;
    this._forcedMaxColumns = new int?();
  }
}
