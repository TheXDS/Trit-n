![Tritón logo](https://raw.githubusercontent.com/TheXDS/Triton/master/Art/Triton%20banner.svg)
[![CodeFactor](https://www.codefactor.io/repository/github/thexds/triton/badge)](https://www.codefactor.io/repository/github/thexds/triton)
[![codecov](https://codecov.io/gh/TheXDS/Triton/branch/master/graph/badge.svg?token=ULEQC09JGW)](https://codecov.io/gh/TheXDS/Triton)
[![Build Triton](https://github.com/TheXDS/Triton/actions/workflows/build.yml/badge.svg)](https://github.com/TheXDS/Triton/actions/workflows/build.yml)
[![Publish Triton](https://github.com/TheXDS/Triton/actions/workflows/publish.yml/badge.svg)](https://github.com/TheXDS/Triton/actions/workflows/publish.yml)
[![Issues](https://img.shields.io/github/issues/TheXDS/Triton)](https://github.com/TheXDS/Triton/issues)
[![MIT](https://img.shields.io/github/license/TheXDS/Triton)](https://mit-license.org)

## Introducción
Tritón es una librería auxiliar que facilita el acceso a la API de gestores
de bases de datos, particularmente Entity Framework Core Provee de servicios,
clases base, generadores dinámicos y otras herramientas misceláneas.

## Releases
Tritón se encuentra disponible en NuGet y en mi repositorio privado de GitHub.

Release | Link
--- | ---
Última versión estable: | [![Versión estable](https://buildstats.info/nuget/TheXDS.Triton)](https://www.nuget.org/packages/TheXDS.Triton/)
Última versión de desarrollo: | [![Versión de desarrollo](https://buildstats.info/nuget/TheXDS.Triton?includePreReleases=true)](https://www.nuget.org/packages/TheXDS.Triton/)

**Package Manager**  
```sh
Install-Package TheXDS.Triton
```

**.NET CLI**  
```sh
dotnet add package TheXDS.Triton
```

**Paket CLI**  
```sh
paket add TheXDS.Triton
```

**Referencia de paquete**  
```xml
<PackageReference Include="TheXDS.Triton" Version="1.4.0" />
```

**Ventana interactiva (CSI)**  
```
#r "nuget: TheXDS.Triton, 1.4.0"
```

#### Repositorio de GitHub
Para obtener los paquetes de Tritón directamente desde GitHub, es necesario
agregar mi repositorio privado. Paar lograr esto, solo es necesario
ejecutar en una terminal:
```sh
nuget sources add -Name "TheXDS GitHub Repo" -Source https://nuget.pkg.github.com/TheXDS/index.json
```

## Compilación
Tritón requiere de un compilador compatible con C# 10, debido a ciertas
características especiales del lenguaje que ayudan a disminuir la
complejidad del código.

Tritón también requiere que [.Net SDK 6.0](https://dotnet.microsoft.com/) esté instalado en el sistema.

### Compilando Tritón
```sh
dotnet build ./src/Triton.sln
```
Los binarios se encontarán en la carpeta `./Build` en la raíz del repositorio.

### Ejecutando pruebas
```sh
dotnet test ./src/Triton.sln
```
#### Reporte de cobertura
Es posible obtener un reporte de la cobertura de código de manera local. Para ello, es necesario instalar 
[`ReportGenerator`](https://github.com/danielpalme/ReportGenerator) , que leerá los resultados de la ejecución de las pruebas, y generará una página web con el resultado de la cobertura.

Para instalar `ReportGenerator` ejecuta:
```sh
dotnet tool install -g dotnet-reportgenerator-globaltool
```
Luego de haber instalado `ReportGenerator`, será posible ejecutar el siguiente comando:
```sh
dotnet test .\src\Triton.sln --collect:"XPlat Code Coverage" --results-directory:.\Build\Tests ; reportgenerator.exe -reports:.\Build\Tests\*\coverage.cobertura.xml -targetdir:.\Build\Coverage\
```
Los resultados de la cobertura se almacenarán en `./Build/Coverage`

## Contribuir
[![Buy Me A Coffee](https://cdn.buymeacoffee.com/buttons/default-orange.png)](https://www.buymeacoffee.com/xdsxpsivx)

Si Tritón te ha sido de utilidad, o te interesa donar para fomentar el
desarrollo del proyecto, siéntete libre de hacer una donación por medio de
[PayPal](https://paypal.me/thexds), [BuyMeACoffee](https://www.buymeacoffee.com/xdsxpsivx)
o ponte en contacto directamente conmigo.

Lamentablemente, no puedo ofrecer otros medios de donación por el momento
debido a que mi país (Honduras) no es soportado por casi ninguna plataforma.
