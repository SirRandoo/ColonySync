# StreamKit.Common

This project provides shared resources for the StreamKit solution, containing
models, contracts, enums, settings, and utilities used across the mod and
server components. It's designed to support both `StreamKit.Mod` and
`StreamKit.Server` with reusable, platform-agnostic code, facilitating
communication and data consistency between the mod and server.

## Purpose

- Centralize shared code and configurations, promoting code reuse and reducing
  redundancy.
- Established common contracts and models to maintain data structures across
  the solution.
- Define core enumerations, settings, and utility functions that can be used
  by various components.
- Abstract dependencies to support the adaptability of different runtime
  environments (e.g., SQLite for the mod, PostgreSQL for the server).

## Directory Structure

```text
StreamKit.Common
|
+-- Contracts  # Interfaces and data transfer objects (DTOs) shared across mod and server
+-- Enums      # Enumerations used across the solution
+-- Models     # Core domain models not specific to mod or server
+-- Settings   # Shared settings and configuration objects
+-- Utilities  # Helper classes and common utilities
```

## Folder Explanations

- **Contracts** <br/>
  Contains interfaces and DTOs that define how data is structured and shared
  between mod and server components. These contracts establish a consistent
  format for inter-component data exchange.
- **Enums** <br/>
  Houses shared enumerations used throughout the project, ensuring type safety
  and consistent usage of values between components.
- **Models** <br/>
  Contains core domain models used by both the mod and server defining core
  entities without infrastructure-specific dependencies. Models here should be
  simple representations, free from specific data access or UI logic.
- **Settings** <br/>
  This directory includes application-wide settings. These settings allow mod
  and server components to use shared configurations where appropriate.
- **Utilities** <br/>
  A collection of helper classes and utility functions commonly used across the
  mod and server. Utilities may include file path management, validation
  helpers, and logging setup.

# Usage Guidelines

1. **Adding New Enums** <br/>
   Place any new enums that may be shared by both mod and server components
   into the `Enums` folder, ensuring they're neutral and don't contain
   implementation-specific logic.
2. **Defining New Contracts or DTOs** <br/>
   When creating new data contracts for shared interactions, add them under
   `Contracts` and ensure they are well-documented for ease of use by both mod
   and server developers.
3. **Settings Management** <br/>
   `Settings` houses configurations that both mod and server components might
   rely on. For component-specific settings, avoid placing them here unless
   they are necessary for cross-component interaction.
4. **Utilities** <br/>
   Helper functions and classes that facilitate shared logic should be defined
   in the `Utilities` folder. Avoid adding component-specific logic here to
   maintain the neutrality of `StreamKit.Common`.

# Future Considerations

As the solutions grows, `StreamKit.Common` may also include shared components,
like validation schemas or notification interfaces, to accommodate expanding
communication needs between the mod and server components.

# Contribution Guidelines

Please document any new files added to `StreamKit.Common` to ensure that their
purpose and intended usage are clear for other developers. Test any added
utilities or settings changes across both mod and server components to confirm
they're universally applicable.
