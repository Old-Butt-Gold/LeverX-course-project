using System.Reflection;
using EER.Domain.DatabaseAbstractions;
using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;

namespace EER.Architecture.Tests;

public class ArchitectureTests
{
    private const string DomainNamespace = "EER.Domain";
    private const string ApplicationNamespace = "EER.Application";
    private const string PersistenceNamespace = "EER.Persistence";
    private const string InfrastructureNamespace = "EER.Infrastructure";
    private const string ApiNamespace = "EER.API";
    private const string Dll = ".dll";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        var assembly = Assembly.LoadFrom(DomainNamespace + Dll);

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                ApplicationNamespace,
                PersistenceNamespace,
                ApiNamespace,
                InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Domain_Should_Not_DependOnExternalLibraries()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(DomainNamespace + Dll);

        var forbiddenReferences = new[] {
            "Microsoft.EntityFrameworkCore",
            "System.Data.SqlClient"
        };

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenReferences)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_Should_Not_DependOnAnyOtherProjectsExceptDomain()
    {
        var assembly = Assembly.LoadFrom(ApplicationNamespace + Dll);

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                ApiNamespace,
                PersistenceNamespace,
                InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_Should_NotReferenceJson()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(ApplicationNamespace + Dll);

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Newtonsoft.Json", "System.Text.Json")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_Should_Not_DependOnApi()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(PersistenceNamespace + Dll);

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            // later also infrastructure if exists
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_Should_Not_DependOnInfrastructure()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(PersistenceNamespace + Dll);

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Persistence_Should_Not_DependOnApplication()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(PersistenceNamespace + Dll);

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Controllers_Should_Not_ReferenceRepositoriesDirectly()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(ApiNamespace + Dll);

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(ControllerBase))
            .ShouldNot()
            .HaveDependencyOn(PersistenceNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Interfaces_Should_StartWithI()
    {
        // Arrange
        var allAssemblies = new[]
        {
            Assembly.LoadFrom(DomainNamespace + Dll),
            Assembly.LoadFrom(ApplicationNamespace + Dll),
            Assembly.LoadFrom(PersistenceNamespace + Dll),
            Assembly.LoadFrom(InfrastructureNamespace + Dll),
        };

        // Act and Assert
        foreach (var assembly in allAssemblies)
        {
            var result = Types.InAssembly(assembly)
                .That()
                .AreInterfaces()
                .Should()
                .HaveNameStartingWith("I")
                .GetResult();

            Assert.True(result.IsSuccessful);
        }
    }

    [Fact]
    public void Repositories_Should_FollowNamingConvention()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(PersistenceNamespace + Dll);

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(IRepository<,>))
            .Should()
            .HaveNameEndingWith("Repository")
            .And()
            .NotBePublic() // Register in Extensions
            .And()
            .ResideInNamespaceEndingWith(".Repositories")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Infrastructure_Should_DependOnlyOnApplicationAndDomain()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(InfrastructureNamespace + Dll);

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(PersistenceNamespace, ApiNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Infrastructure_Should_ContainAllImplementations()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(InfrastructureNamespace + Dll);
        var applicationAssembly = Assembly.LoadFrom(ApplicationNamespace + Dll);

        // Act
        var interfaces = GetUnimplementedInterfaces(applicationAssembly);

        var missingImplementations = new List<string>();
        foreach (var interfaceType in interfaces)
        {
            var implementation = Types.InAssembly(assembly)
                .That().ImplementInterface(interfaceType)
                .GetTypes();

            if (!implementation.Any())
                missingImplementations.Add(interfaceType.Name);
        }

        // Assert
        Assert.True(missingImplementations.Count == 0,
            $"Missing implementations for: {string.Join(", ", missingImplementations)}");
        return;

        static IEnumerable<Type> GetUnimplementedInterfaces(Assembly assembly)
        {
            var allTypes = assembly.GetTypes();

            var interfaces = allTypes
                .Where(t => t.IsInterface)
                .ToList();

            var classes = allTypes
                .Where(t => t is { IsClass: true, IsAbstract: false })
                .ToList();

            foreach (var iface in interfaces)
            {
                var hasImpl = classes.Any(c => c.GetInterfaces().Contains(iface));

                if (!hasImpl)
                    yield return iface;
            }
        }
    }

    [Fact]
    public void Infrastructure_Should_NotReferenceDomainDirectly()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(InfrastructureNamespace + Dll);

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

}
