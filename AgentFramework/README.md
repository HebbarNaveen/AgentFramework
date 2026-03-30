# CustomAgentFramework

A lightweight .NET agent framework for building and running custom conversational AI agents with extension points for prompts, messaging, and flow orchestration.  
This repository contains a reusable core agent library (`AgentFramework`) plus a sample app (`CustomAgent`) demonstrating local agent execution.

## Table of Contents

- [Features](#features)
- [Repository Structure](#repository-structure)
- [Getting Started](#getting-started)
- [Build and Run](#build-and-run)
- [Usage](#usage)
- [Testing](#testing)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)

## Features

- Agent runtime abstraction layer in `AgentFramework`
- Config-driven behavior with `config.json` and `message.json`
- Custom agent host example in `CustomAgent.cs`
- Push notification sample in `SendCloudMessage.cs` / `StackHawk.cs`
- .NET 6 and .NET 9 build targets with cross-compile solution file

## Repository Structure

- `AgentFramework/` — core library
  - `AgentBuilder.cs`, `IAgentBuilder.cs`, `MCPClient.cs`
- `CustomAgent.csproj` — sample console application
- `AgentFramework.sln` — multi-project solution
- `config.json` — runtime config settings used by sample agent
- `message.json` — sample message payloads
- `README.md` — this document
- `stackhawk_template.yml` — CI/test security policy template

## Getting Started

Prerequisites:
- .NET SDK 6.0+ installed (preferably latest LTS)
- Git

Clone repository:

```bash
git clone https://<your-repo-url>.git
cd CustomAgentFramework
```

## Build and Run

Restore dependencies:

```bash
dotnet restore
```

Build solution:

```bash
dotnet build AgentFramework.sln -c Release
```

Run sample app:

```bash
dotnet run --project CustomAgent.csproj
```

> You may also run from `bin/Release/net6.0/` artifacts after build.

## Usage

1. Edit `config.json` and `message.json` to match your external service and message schema.
2. Extend or customize logic in:
   - `CustomAgent.cs` for startup and orchestration
   - `AgentFramework/` for agent behaviors, builder patterns and API client integration
3. Rebuild and rerun.

## Configuration

- `config.json` — app-level configuration (API endpoints, keys, mode)
- `message.json` — sample payload and agent request templates
- `stackhawk_template.yml` — optional automated security scan template

## Contributing

1. Fork repository
2. Create feature branch
3. Add tests and validation
4. Submit pull request with clear description

Coding style basics:
- C# idiomatic naming and comments
- Null-safe code and dependency injection in framework layer
- Keep agent startup minimal and extensible

## Testing

No dedicated test project is included by default.  
Add `xUnit` or `NUnit` test project and target key logic in `AgentFramework` for:
- agent initialization
- message building
- network client behavior

## Next Steps

- Add step-by-step sample in docs with live response flow
- Document API extension points in `AgentFramework`
- Add unit tests/project and CI pipeline in `.github/workflows`

## License

MIT License — see `LICENSE` (if not present, add as required).
