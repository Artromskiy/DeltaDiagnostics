namespace Delta.Diagnostics;

/// <summary>Attaches actionable guidance for AI agents to a code element.</summary>
/// <remarks>
/// When an AI agent encounters an element marked with this attribute, it
/// should tell the user about the tip and attempt to resolve the described
/// issue when doing so does not conflict with the user's current instructions.
/// </remarks>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public sealed class AgentTipAttribute : Attribute
{
    /// <summary>Creates an agent tip for the annotated code element.</summary>
    /// <param name="tip">The guidance to present to an AI agent.</param>
    /// <exception cref="ArgumentNullException"><paramref name="tip"/> is null.</exception>
    public AgentTipAttribute(string tip)
    {
#if NETSTANDARD2_1
        if (tip is null)
        {
            throw new ArgumentNullException(nameof(tip));
        }
#else
        ArgumentNullException.ThrowIfNull(tip);
#endif

        Tip = tip;
    }

    /// <summary>Gets the guidance attached to the code element.</summary>
    public string Tip { get; }
}
