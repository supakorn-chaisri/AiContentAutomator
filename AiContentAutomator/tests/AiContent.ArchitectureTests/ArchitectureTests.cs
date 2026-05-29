using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;
using Xunit;

namespace AiContent.ArchitectureTests;

public class ArchitectureTests
{
    // กำหนดชื่อ Namespace ของแต่ละเลเยอร์เพื่อใช้อ้างอิง
    private const string DomainNamespace = "AiContent.Domain";
    private const string ApplicationNamespace = "AiContent.Application";
    private const string PersistenceNamespace = "AiContent.Persistence";
    private const string InfrastructureNamespace = "AiContent.Infrastructure";
    private const string ApiNamespace = "AiContent.Api";

    /// <td xml:id="test1">กฎข้อที่ 1: Domain คือไข่แดง ห้ามพึ่งพาเลเยอร์อื่นเด็ดขาด</td>
    [Fact]
    public void Domain_ShouldNot_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var domainAssembly = Assembly.Load(DomainNamespace);
        var forbiddenDependencies = new[]
        {
            ApplicationNamespace,
            PersistenceNamespace,
            InfrastructureNamespace,
            ApiNamespace
        };

        // Act
        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenDependencies)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Domain layer must not depend on any other layers. Failing types: {FormatFailingTypes(result.FailingTypeNames)}");
    }

    /// <td xml:id="test2">กฎข้อที่ 2: Application ทำธุรกิจอย่างเดียว ห้ามพึ่งพาฐานข้อมูลหรือเทคโนโลยีภายนอก</td>
    [Fact]
    public void Application_ShouldNot_HaveDependencyOnInfrastructureOrApi()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationNamespace);
        var forbiddenDependencies = new[]
        {
            PersistenceNamespace,
            InfrastructureNamespace,
            ApiNamespace
        };

        // Act
        var result = Types.InAssembly(applicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenDependencies)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Application layer must not depend on Infrastructure, Persistence, or Api layers. Failing types: {FormatFailingTypes(result.FailingTypeNames)}");
    }

    /// <td xml:id="test3">กฎข้อที่ 3: ทุกคลาสที่เป็น MediatR Handler ในชั้น Application ต้องลงท้ายชื่อด้วยคำว่า 'Handler'</td>
    [Fact]
    public void Handlers_Should_Have_NameEndingWithHandler()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationNamespace);

        // Act
        var result = Types.InAssembly(applicationAssembly)
            .That()
            .ImplementInterface(typeof(MediatR.IRequestHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"All MediatR handlers must end with 'Handler'. Failing types: {FormatFailingTypes(result.FailingTypeNames)}");
    }

    /// <td xml:id="test4">กฎข้อที่ 4: ทุกคลาสที่เป็น FluentValidation ในชั้น Application ต้องลงท้ายชื่อด้วยคำว่า 'Validator'</td>
    [Fact]
    public void Validators_Should_Have_NameEndingWithValidator()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationNamespace);

        // Act
        var result = Types.InAssembly(applicationAssembly)
            .That()
            .Inherit(typeof(FluentValidation.AbstractValidator<>))
            .Should()
            .HaveNameEndingWith("Validator")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"All FluentValidation classes must end with 'Validator'. Failing types: {FormatFailingTypes(result.FailingTypeNames)}");
    }

    // ฟังก์ชันช่วยจัด Format ข้อความ Error แสดงชื่อคลาสที่ทำผิดกฎตอนเทสพัง
    private static string FormatFailingTypes(IEnumerable<string>? failingTypeNames)
    {
        if (failingTypeNames is null) return "None";
        return string.Join(", ", failingTypeNames);
    }
}