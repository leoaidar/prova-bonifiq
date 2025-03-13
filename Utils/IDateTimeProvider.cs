using System;

namespace ProvaPub.Utils
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
