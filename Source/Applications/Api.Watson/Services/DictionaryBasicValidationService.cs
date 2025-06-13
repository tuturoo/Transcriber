using AspNetCore.Authentication.Basic;

namespace Api.Watson.Services
{
    public sealed class DictionaryBasicValidationService : IBasicUserValidationService
    {
        private readonly IDictionary<string, string> _authenticationData; 

        public DictionaryBasicValidationService(IDictionary<string, string> authenticationData)
        {
            _authenticationData = authenticationData;
        }

        public Task<bool> IsValidAsync(string username, string password)
        {
            if (_authenticationData.TryGetValue(username, out string? value) && value == password)
                return Task.FromResult(true);

            return Task.FromResult(false);
        }
    }
}
