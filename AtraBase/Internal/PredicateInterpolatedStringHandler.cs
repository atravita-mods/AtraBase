namespace AtraBase.Internal;

#if NET6_0_OR_GREATER

using System.Runtime.CompilerServices;

[InterpolatedStringHandler]
public ref struct PredicateInterpolatedStringHandler
{
    private DefaultInterpolatedStringHandler _handler;

    public PredicateInterpolatedStringHandler(int literateLength, int formattedCount, bool predicate, out bool handlerIsValid)
    {
        if (!predicate)
        {
            handlerIsValid = false;
            this._handler = default;
            return;
        }

        handlerIsValid = true;
        this._handler = new DefaultInterpolatedStringHandler(literateLength, formattedCount);
    }

    public void AppendLiteral(string s) => this._handler.AppendLiteral(s);

    public void AppendFormatted<T>(T t) => this._handler.AppendFormatted(t);

    public override string ToString() => this._handler.ToStringAndClear();
}

#endif