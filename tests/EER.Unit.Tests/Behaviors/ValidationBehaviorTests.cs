using EER.Application.Behaviors;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace EER.Unit.Tests.Behaviors;

public class ValidationBehaviorTests
{
    private readonly Mock<IValidator<TestRequest>> _validatorMock1 = new();
    private readonly Mock<IValidator<TestRequest>> _validatorMock2 = new();
    private readonly ValidationBehavior<TestRequest, TestResponse> _behavior;
    private readonly Mock<RequestHandlerDelegate<TestResponse>> _nextMock = new();

    public ValidationBehaviorTests()
    {
        var validators = new List<IValidator<TestRequest>>
        {
            _validatorMock1.Object, _validatorMock2.Object
        };
        _behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
    }

    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        // Arrange
        var noValidatorsBehavior =
            new ValidationBehavior<TestRequest, TestResponse>(new List<IValidator<TestRequest>>());
        var response = new TestResponse();
        _nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await noValidatorsBehavior.Handle(new TestRequest(), _nextMock.Object, CancellationToken.None);

        // Assert
        Assert.Same(response, result);
        _nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_CallsNext()
    {
        // Arrange
        var request = new TestRequest();
        var response = new TestResponse();

        _validatorMock1.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _validatorMock2.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        Assert.Same(response, result);
        _nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = new TestRequest();
        var failures = new List<ValidationFailure>
        {
            new("Property1", "Error1"),
            new("Property2", "Error2")
        };

        _validatorMock1.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        _validatorMock2.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => _behavior.Handle(request, _nextMock.Object, CancellationToken.None));

        Assert.Equal(2, ex.Errors.Count());
        _nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    public record TestRequest : IRequest<TestResponse>;

    public record TestResponse;
}
