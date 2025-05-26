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
    private const string ApiNamespace = "EER.API";
    private const string Dll = ".dll";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(DomainNamespace + Dll);

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, PersistenceNamespace, ApiNamespace)
            // later also infrastructure if exists
            .GetResult();

        // Assert
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
        // Arrange
        var assembly = Assembly.LoadFrom(ApplicationNamespace + Dll);

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApiNamespace, PersistenceNamespace)
            // later also infrastructure if exists
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_Should_NotReferenceNewtonsoftJson()
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
    public void Persistence_Should_Not_DependOnApplication()
    {
        // Arrange
        var assembly = Assembly.LoadFrom(PersistenceNamespace + Dll);

        // Act
        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            // later also infrastructure if exists
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
            .ImplementInterface(typeof(IRepository<>))
            .Should()
            .HaveNameEndingWith("Repository")
            .And()
            .NotBePublic() // Register in Extensions
            .And()
            .ResideInNamespace(PersistenceNamespace + ".Repositories")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

}
