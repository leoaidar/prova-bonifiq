using ProvaPub.Utils;
using System;

namespace ProvaPub.Tests
{
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTime _fixedTime;

        public FakeDateTimeProvider(DateTime fixedTime)
        {
            _fixedTime = fixedTime;
        }

        public DateTime UtcNow => _fixedTime; // Sempre retorna o tempo fixo injetado
    }
}
