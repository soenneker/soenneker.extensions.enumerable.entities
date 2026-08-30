[![](https://img.shields.io/nuget/v/soenneker.extensions.enumerable.entities.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.enumerable.entities/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.enumerable.entities/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.enumerable.entities/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.enumerable.entities.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.enumerable.entities/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.enumerable.entities/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.enumerable.entities/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Enumerable.Entities

Projects and searches the complete `Id` values of `IEntity` sequences.

## Installation

```bash
dotnet add package Soenneker.Extensions.Enumerable.Entities
```

## Project IDs

```csharp
using Soenneker.Entities.Entity.Abstract;
using Soenneker.Extensions.Enumerable.Entities;

IEnumerable<IEntity> entities = GetEntities();
List<string> ids = entities.ToIds();
```

`ToIds()` immediately creates a new list in source order. Duplicate IDs and compound IDs such as `partition:document` are preserved exactly; this package does not split them. A null source returns an empty list. Known collection counts are used only for capacity, and the source is visited once.

## Find an ID

```csharp
bool found = entities.ContainsId("customer-42:order-100");
```

`ContainsId()` performs an ordinal, case-sensitive comparison and stops at the first match. It returns `false` when the source is null or the requested ID is null or empty. Entity elements themselves must be non-null.

To extract the partition and document portions of an individual compound entity ID, use `Soenneker.Extensions.Entities`.
