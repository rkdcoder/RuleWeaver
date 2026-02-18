using Microsoft.Extensions.Configuration;
using RuleWeaver.Abstractions;
using RuleWeaver.Core;
using RuleWeaver.Rules;

namespace RuleWeaver.Tests
{
    public class ValidationEngineTests
    {
        // --- DTOs simple ---
        private class UserProfile
        {
            public int Age { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        // --- DTOs Nested ---
        private class Address
        {
            public string? Street { get; set; }
            public string? ZipCode { get; set; }
        }

        private class Customer
        {
            public string? Name { get; set; }
            public Address? BillingAddress { get; set; }
        }


        private class SilentFailRule : IValidationRule
        {
            public string Name => "SilentFail";
            public ValueTask<RuleResult> ValidateAsync(object? value, string[] args)
            {
                return new ValueTask<RuleResult>(RuleResult.Failure(""));
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
        public async Task Validate_Nivel1_UsaMensagemDoJSON_Override()
        {
            var config = new Dictionary<string, string> {
                {"RuleWeaver:UserProfile:Age:0:RuleName", "MinValue"},
                {"RuleWeaver:UserProfile:Age:0:RuleParameter", "18"},
                {"RuleWeaver:UserProfile:Age:0:RuleErrorMessage", "You are too young."}
            };

            var engine = BuildEngine(config);
            var model = new UserProfile { Age = 10 };

            var result = await engine.ValidateAsync(model);

            var errorDetail = result.FirstOrDefault(x => x.Property == "Age");
            Assert.NotNull(errorDetail);

            var failure = errorDetail.Violations.First();
            Assert.Equal("MinValue", failure.Rule);
            Assert.Equal("You are too young.", failure.Message);
        }

        [Fact]
        public async Task Validate_Nivel2_UsaMensagemPadraoIngles()
        {
            var config = new Dictionary<string, string> {
                {"RuleWeaver:UserProfile:Age:0:RuleName", "MinValue"},
                {"RuleWeaver:UserProfile:Age:0:RuleParameter", "18"}
            };

            var engine = BuildEngine(config);
            var model = new UserProfile { Age = 10 };

            var result = await engine.ValidateAsync(model);

            var errorDetail = result.FirstOrDefault(x => x.Property == "Age");
            Assert.NotNull(errorDetail);

            var failure = errorDetail.Violations.First();
            Assert.Equal("MinValue", failure.Rule);
            Assert.Contains("Value must be at least 18.", failure.Message);
        }

        [Fact]
        public async Task Validate_Nivel3_UsaFallbackGenerico()
        {
            var config = new Dictionary<string, string> {
                {"RuleWeaver:UserProfile:Username:0:RuleName", "SilentFail"}
            };

            var engine = BuildEngine(config);
            var model = new UserProfile { Username = "Test" };

            var result = await engine.ValidateAsync(model);

            var errorDetail = result.FirstOrDefault(x => x.Property == "Username");
            Assert.NotNull(errorDetail);

            var failure = errorDetail.Violations.First();
            Assert.Equal("SilentFail", failure.Rule);
            Assert.Equal("Error in SilentFail", failure.Message);
        }

        [Fact]
        public async Task Validate_DeveRetornarEstruturaDeArrayDeterministica()
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

            var result = await engine.ValidateAsync(model);

            Assert.Single(result);

            var errorDetail = result.First();
            Assert.Equal("Password", errorDetail.Property);
            Assert.Equal(2, errorDetail.Violations.Count);

            Assert.Equal("MinLength", errorDetail.Violations[0].Rule);
            Assert.Contains("Length must be at least 8 characters.", errorDetail.Violations[0].Message);

            Assert.Equal("Regex", errorDetail.Violations[1].Rule);
            Assert.Equal("Needs number", errorDetail.Violations[1].Message);
        }

        [Fact]
        public async Task Validate_NaoDeveRetornarNada_SeValidacaoPassar()
        {
            var config = new Dictionary<string, string> {
                {"RuleWeaver:UserProfile:Age:0:RuleName", "MinValue"},
                {"RuleWeaver:UserProfile:Age:0:RuleParameter", "18"}
            };

            var engine = BuildEngine(config);
            var model = new UserProfile { Age = 25 };

            var result = await engine.ValidateAsync(model);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Validate_NestedObject_DeveAchatarErrosComDotNotation()
        {
            var config = new Dictionary<string, string> {
                {"RuleWeaver:Customer:BillingAddress:0:RuleName", "Nested"},

                {"RuleWeaver:Address:Street:0:RuleName", "Required"},
                {"RuleWeaver:Address:Street:0:RuleErrorMessage", "Street is mandatory"}
            };

            var engine = BuildEngine(config);

            var model = new Customer
            {
                Name = "Rodrigo",
                BillingAddress = new Address { Street = "" }
            };

            var result = await engine.ValidateAsync(model);

            Assert.Single(result); 

            var errorDetail = result.First();

            Assert.Equal("BillingAddress.Street", errorDetail.Property);

            var failure = errorDetail.Violations.First();
            Assert.Equal("Required", failure.Rule);
            Assert.Equal("Street is mandatory", failure.Message);
        }
    }
}