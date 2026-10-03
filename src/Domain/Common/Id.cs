namespace Domain.Common;

public record Id
{
    public Guid Value { get; }

    public Id(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(value));

        Value = value;
    }

    public static Id New() => new(Guid.NewGuid());
    public static Id From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
