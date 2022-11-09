# package-usage

A dotnet global tool to inspect package versions across your projects.

Displays all packages used by your projects, grouped by package name, then lists all the different versions of each package.  
Might help you notice version discrepancies, unnecessary references, outdated packages, or plan ahead for migration to central package management.

Sample output:

```
FluentAssertions
    5.10.3, Contoso.Product.Api
     6.3.0, Contoso.Product.Database
     6.5.1, Contoso.Product.Tests
     6.6.0, Contoso.Product.Manager
     6.7.0, Contoso.Product.Common

MassTransit
     7.1.8, Contoso.Product.Api
     8.0.1, Contoso.Product.Manager

Microsoft.AspNetCore.Hosting.Abstractions
    3.1.22, Contoso.Product.Common
     6.0.6, Contoso.Product.Api
```

## Installation

```console
dotnet tool install -g PackageUsage
```

## Usage
Inside a directory with your solution:

```console
package-usage [--only-different]
```
or specify path to solution:
```console
package-usage MySolution.sln [--only-different]
```

Use the optional `--only-different` switch to view only packages with different versions across projects. (Skip packages with same version everywhere).