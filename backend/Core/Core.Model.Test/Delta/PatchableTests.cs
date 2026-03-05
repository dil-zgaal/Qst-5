using Core.Model.Delta;

namespace Core.Model.Test.Delta;

public class PatchableTests
{
    [Fact]
    public void Patchable_NotGiven_DoesNotApply()
    {
        var patch = Patchable<string>.NotGiven();
        var target = "original";

        patch.Apply(target, (_, v) => target = v!);

        Assert.Equal("original", target);
    }

    [Fact]
    public void Patchable_Clear_SetsToNull()
    {
        var patch = Patchable<string?>.Clear();
        string? target = "original";

        patch.Apply(target, (_, v) => target = v);

        Assert.Null(target);
    }

    [Fact]
    public void Patchable_Set_UpdatesValue()
    {
        var patch = Patchable<string>.Set("new value");
        var target = "original";

        patch.Apply(target, (_, v) => target = v!);

        Assert.Equal("new value", target);
    }

    [Fact]
    public void Patchable_HasCorrectStateFlags()
    {
        var notGiven = Patchable<string>.NotGiven();
        var clear = Patchable<string>.Clear();
        var set = Patchable<string>.Set("value");

        Assert.True(notGiven.IsNotGiven);
        Assert.False(notGiven.IsClear);
        Assert.False(notGiven.IsSet);

        Assert.False(clear.IsNotGiven);
        Assert.True(clear.IsClear);
        Assert.False(clear.IsSet);

        Assert.False(set.IsNotGiven);
        Assert.False(set.IsClear);
        Assert.True(set.IsSet);
    }
}
