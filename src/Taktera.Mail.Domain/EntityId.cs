namespace Taktera.Mail.Domain;

internal static class EntityId
{
    public static Guid Require(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("A local identifier must not be empty.", parameterName);

        return value;
    }
}
