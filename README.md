# package-usage

A dotnet global tool to inspect package versions across your projects.

Displays all packages used by your projects, grouped by package name, then lists all the different versions of each package.  
Might help you notice version discrepancies, unnecessary references, outdated packages, or plan ahead for migration to central package management.

Sample output:

```
FluentAssertions
    5.10.3, Contoso.Services.A
     6.3.0, Contoso.Services.B
     6.5.1, Contoso.Services.C
     6.6.0, Contoso.Services.D
     6.7.0, Contoso.Services.E

MassTransit
     7.1.8, Contoso.Services.A
     8.0.1, Contoso.Services.B

Microsoft.AspNetCore.Hosting.Abstractions
    3.1.22, Contoso.Services.A
     6.0.6, Contoso.Services.B
```



## Installation

```cs
dotnet tool install -g package-usage
```

## Usage
Navigate to a directory with your solution, then

```
package-usage [--only-different]
```
or
```
package-usage MySolution.sln [--only-different]
```

Use the optional `--only-different` switch to view only packages with different versions across projects.