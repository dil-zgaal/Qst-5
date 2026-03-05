using Core.Model.Delta;

namespace Core.Model.Test.Delta;

public class PatchableTests
{
    [Fact]
    public void Patchable_NotGiven_DoesNotApply()
    {
        var patch = Patchable<string>.NotGiven();
        var target = "original";

        patch.Patch(v => target = v);

        Assert.Equal("original", target);
    }

    [Fact]
    public void Patchable_Set_UpdatesValue()
    {
        var patch = Patchable<string>.Set("new value");
        var target = "original";

        patch.Patch(v => target = v);

        Assert.Equal("new value", target);
    }

    [Fact]
    public void Patchable_HasCorrectStateFlags()
    {
        var notGiven = Patchable<string>.NotGiven();
        var set = Patchable<string>.Set("value");

        Assert.True(notGiven.IsNotGiven);
        Assert.False(notGiven.IsSet);

        Assert.False(set.IsNotGiven);
        Assert.True(set.IsSet);
    }

    [Fact]
    public void Patchable_Apply_MergesDelta()
    {
        var original = Patchable<string>.Set("original");
        var delta = Patchable<string>.Set("updated");

        original.Apply(delta);

        Assert.Equal("updated", original.Value);
    }

    [Fact]
    public void Patchable_Apply_IgnoresNotGiven()
    {
        var original = Patchable<string>.Set("original");
        var delta = Patchable<string>.NotGiven();

        original.Apply(delta);

        Assert.Equal("original", original.Value);
    }
}

public class PatchableNullableTests
{
    [Fact]
    public void PatchableNullable_NotGiven_DoesNotApply()
    {
        var patch = PatchableNullable<string>.NotGiven();
        string? target = "original";

        patch.Patch(v => target = v);

        Assert.Equal("original", target);
    }

    [Fact]
    public void PatchableNullable_Clear_SetsToNull()
    {
        var patch = PatchableNullable<string>.Clear();
        string? target = "original";

        patch.Patch(v => target = v);

        Assert.Null(target);
    }

    [Fact]
    public void PatchableNullable_Set_UpdatesValue()
    {
        var patch = PatchableNullable<string>.Set("new value");
        string? target = "original";

        patch.Patch(v => target = v);

        Assert.Equal("new value", target);
    }

    [Fact]
    public void PatchableNullable_SetNull_ClearsValue()
    {
        var patch = PatchableNullable<string>.Set(null);

        Assert.True(patch.IsClear);
        Assert.Null(patch.Value);
    }

    [Fact]
    public void PatchableNullable_HasCorrectStateFlags()
    {
        var notGiven = PatchableNullable<string>.NotGiven();
        var clear = PatchableNullable<string>.Clear();
        var set = PatchableNullable<string>.Set("value");

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

    [Fact]
    public void PatchableNullable_Apply_MergesDelta()
    {
        var original = PatchableNullable<string>.Set("original");
        var delta = PatchableNullable<string>.Set("updated");

        original.Apply(delta);

        Assert.Equal("updated", original.Value);
    }

    [Fact]
    public void PatchableNullable_Apply_ClearsValue()
    {
        var original = PatchableNullable<string>.Set("original");
        var delta = PatchableNullable<string>.Clear();

        original.Apply(delta);

        Assert.True(original.IsClear);
        Assert.Null(original.Value);
    }

    [Fact]
    public void PatchableNullable_Apply_IgnoresNotGiven()
    {
        var original = PatchableNullable<string>.Set("original");
        var delta = PatchableNullable<string>.NotGiven();

        original.Apply(delta);

        Assert.Equal("original", original.Value);
    }
}
