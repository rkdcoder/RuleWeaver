using Microsoft.Extensions.Configuration;
using RuleWeaver.Abstractions;
using RuleWeaver.Core;
using RuleWeaver.Rules;

namespace RuleWeaver.Tests
{
    public class ValidationEngineTests
    {
        private class UserProfile
        {
            public int Age { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        private class SilentFailRule : IValidationRule
        {
            public string Name => "SilentFail";
            public bool Validate(object? value, string[] args, out string errorMessage)
            {
                errorMessage = "";
                return false;
            }
        }

        private ValidationEngine BuildEngine(Dictionary<string, string> configData)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData!)
                .Build();

            var cache = new ValidationCache(configuration);

            var rules = new List<IValidationRule>
            {
                new RequiredRule(),
                new MinValueRule(),
                new MinLengthRule(),
                new RegexRule(),
                new SilentFailRule()
            };

            return new ValidationEngine(cache, rules);
        }

        [Fact]
        public void Validate_Nivel1_UsaMensagemDoJSON_Override()
        {
            var config = new Dictionary<string, string> {
                {"RuleWeaver:UserProfile:Age:0:RuleName", "MinValue"},
                {"RuleWeaver:UserProfile:Age:0:RuleParameter", "18"},
                {"RuleWeaver:UserProfile:Age:0:RuleErrorMessage", "You are too young."}
            };

            var engine = BuildEngine(config);
            var model = new UserProfile { Age = 10 };

            var result = engine.Validate(model);

            var errorDetail = result.FirstOrDefault(x => x.Property == "Age");
            Assert.NotNull(errorDetail);
            Assert.Contains("You are too young.", errorDetail.Messages);
        }

        [Fact]
        public void Validate_Nivel2_UsaMensagemPadraoIngles()
        {
            var config = new Dictionary<string, string> {
                {"RuleWeaver:UserProfile:Age:0:RuleName", "MinValue"},
                {"RuleWeaver:UserProfile:Age:0:RuleParameter", "18"}
            };

            var engine = BuildEngine(config);
            var model = new UserProfile { Age = 10 };

            var result = engine.Validate(model);

            var errorDetail = result.FirstOrDefault(x => x.Property == "Age");
            Assert.NotNull(errorDetail);
            Assert.Contains("Value must be at least 18.", errorDetail.Messages);
        }

        [Fact]
        public void Validate_Nivel3_UsaFallbackGenerico()
        {
            var config = new Dictionary<string, string> {
                {"RuleWeaver:UserProfile:Username:0:RuleName", "SilentFail"}
            };

            var engine = BuildEngine(config);
            var model = new UserProfile { Username = "Test" };

            var result = engine.Validate(model);

            var errorDetail = result.FirstOrDefault(x => x.Property == "Username");
            Assert.NotNull(errorDetail);
            Assert.Contains("Error in SilentFail", errorDetail.Messages);
        }

        [Fact]
        public void Validate_DeveRetornarEstruturaDeArrayDeterministica()
        {
            var config = new Dictionary<string, string> {
                {"RuleWeaver:UserProfile:Password:0:RuleName", "MinLength"},
                {"RuleWeaver:UserProfile:Password:0:RuleParameter", "8"},

                {"RuleWeaver:UserProfile:Password:1:RuleName", "Regex"},
                {"RuleWeaver:UserProfile:Password:1:RuleParameter", "[0-9]"},
                {"RuleWeaver:UserProfile:Password:1:RuleErrorMessage", "Needs number"}
            };

            var engine = BuildEngine(config);
            var model = new UserProfile { Password = "abc" };

            var result = engine.Validate(model);

            Assert.Single(result);

            var errorDetail = result.First();
            Assert.Equal("Password", errorDetail.Property);
            Assert.Equal(2, errorDetail.Messages.Count);

            Assert.Contains("Length must be at least 8 characters.", errorDetail.Messages);
            Assert.Contains("Needs number", errorDetail.Messages);
        }

        [Fact]
        public void Validate_NaoDeveRetornarNada_SeValidacaoPassar()
        {
            var config = new Dictionary<string, string> {
                {"RuleWeaver:UserProfile:Age:0:RuleName", "MinValue"},
                {"RuleWeaver:UserProfile:Age:0:RuleParameter", "18"}
            };

            var engine = BuildEngine(config);
            var model = new UserProfile { Age = 25 };

            var result = engine.Validate(model);

            Assert.Empty(result);
        }
    }
}