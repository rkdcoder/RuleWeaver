# 🕸️ RuleWeaver

<p align="center">
  <img src="https://raw.githubusercontent.com/rkdcoder/RuleWeaver/master/src/RuleWeaver/Media/icon.png" width="128" alt="RuleWeaver logo" />
</p>

[![NuGet](https://img.shields.io/nuget/v/RuleWeaver.svg)](https://www.nuget.org/packages/RuleWeaver)
[![Build & Publish](https://github.com/rkdcoder/RuleWeaver/actions/workflows/main.yml/badge.svg)](https://github.com/rkdcoder/RuleWeaver/actions/workflows/main.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

**Validation without the boilerplate.** A high-performance, configuration-driven, .NET 10–agnostic validation engine.

---

## 🚀 Why RuleWeaver?

Tired of creating repetitive `Validator` classes for every DTO? **RuleWeaver** eliminates the need for “robotic” validation coding.

- **Zero Validation Classes:** Define rules directly in `appsettings.json`.
- **Enterprise-Grade Performance:** Singleton cache of execution plans. JSON parsing happens only once; subsequent requests run entirely in memory (Zero I/O).
- **Deterministic:** Structured and frontend-friendly error responses (Array of Objects).
- **Extensible:** Create custom rules (e.g. `IsCpf`, `IsCnpj`) in your API and RuleWeaver discovers them automatically.
- **Safe Dependency Injection:** Architecture that respects scopes (Singleton Cache + Scoped Engine), allowing rules that access databases.

---

## 📦 Installation

Install via NuGet Package Manager Console:

```bash
Install-Package RuleWeaver
Or via .NET CLI:

dotnet add package RuleWeaver
⚡ Quick Start
1. Register in Program.cs
In your Program.cs, register the service.
The parameter typeof(Program).Assembly allows RuleWeaver to discover custom rules in your project.

using RuleWeaver.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Registers the Cache (Singleton) and the Engine (Scoped)
builder.Services.AddRuleWeaver(typeof(Program).Assembly);

builder.Services.AddControllers();
// ...
2. Configure the Rules (appsettings.json)
Use the RuleWeaver section to map your classes and properties.

{
  "RuleWeaver": {
    "CustomerRequest": {
      "Name": [
        {
          "RuleName": "Required",
          "RuleParameter": null,
          "RuleErrorMessage": "Name is required."
        },
        {
          "RuleName": "MinLength",
          "RuleParameter": "3"
        }
      ],
      "Age": [
        {
          "RuleName": "MinValue",
          "RuleParameter": "18",
          "RuleErrorMessage": "Not allowed for under 18."
        }
      ],
      "Email": [
        { "RuleName": "Required" },
        { "RuleName": "Email", "RuleErrorMessage": "Invalid email." }
      ]
    }
  }
}
3. Use in the Controller
Add the [WeaveValidation] attribute to your Action.
The engine will validate the object before entering the method.

using Microsoft.AspNetCore.Mvc;
using RuleWeaver.Attributes;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    [HttpPost]
    [WeaveValidation] // <--- The magic happens here
    public IActionResult Create([FromBody] CustomerRequest request)
    {
        // If execution reaches here, the request is already validated!
        return Ok();
    }
}
🛠️ Available Rules (Built-in)
The package already includes the following native rules:

Rule	Parameter (Ex)	Description
Required	—	Checks if the value is not null or empty.
MinLength	5	Minimum length for Strings or Lists.
MaxLength	100	Maximum length for Strings or Lists.
MinValue	18	Minimum numeric value.
MaxValue	99	Maximum numeric value.
Email	—	Validates email format (Regex).
Regex	[0-9]	Validates against a custom regular expression.
🧩 Creating Custom Rules
Need to validate CPF, CNPJ, or query the database?
Create a class implementing IValidationRule in your API.

using RuleWeaver.Abstractions;

public class CpfRule : IValidationRule
{
    public string Name => "IsCpf"; // Name used in JSON

    public bool Validate(object? value, string[] args, out string errorMessage)
    {
        errorMessage = "";
        if (value is null) return true; // Let Required handle nulls

        // ... Your CPF validation logic ...
        if (!CpfValido(value.ToString()))
        {
            errorMessage = "Invalid CPF.";
            return false;
        }
        return true;
    }
}
In JSON:

"Document": [
  { "RuleName": "IsCpf", "RuleErrorMessage": "Invalid document." }
]
📡 Response Format (API)
RuleWeaver returns a 400 Bad Request with a deterministic and frontend-friendly JSON body.

{
  "message": "Validation errors occurred.",
  "errors": [
    {
      "property": "Age",
      "messages": [
        "Not allowed for under 18."
      ]
    },
    {
      "property": "Password",
      "messages": [
        "Length must be at least 8 characters.",
        "Needs number"
      ]
    }
  ]
}
🏛️ Architecture and Performance
RuleWeaver was designed for scalability:

Layer 1 (Singleton Cache): On startup, RuleWeaver reads appsettings.json, parses it, and compiles an Execution Plan. This happens only once.

Layer 2 (Scoped Engine): On each request, the engine simply consults the in-memory plan (O(1)) and executes the rules.

Result: Virtually zero I/O and reflection cost after the initial warm-up.

🤝 Contribution
Contributions are welcome! Feel free to open Issues or submit Pull Requests.

📄 License
This project is licensed under the MIT License.
```
