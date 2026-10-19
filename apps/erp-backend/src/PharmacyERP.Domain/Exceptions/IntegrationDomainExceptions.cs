
namespace PharmacyERP.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}
public class IntegrationDomainException : DomainException
{
    public IntegrationDomainException(string message) : base(message) { }
}

public class InvalidIdempotencyHashException : IntegrationDomainException
{
    public InvalidIdempotencyHashException(string hash)
        : base($"Invalid payload hash '{hash}'. The payload hash must be exactly 64 characters (SHA-256 Hex string).")
    {
    }
}

public class MissingSourcePromptIdException : IntegrationDomainException
{
    public MissingSourcePromptIdException()
        : base("SourcePromptId is strictly required when the ActorType is AI_ASSISTED. AI actions must be traceable to their source prompt.")
    {
    }
}