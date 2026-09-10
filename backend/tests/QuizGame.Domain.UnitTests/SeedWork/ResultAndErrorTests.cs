using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.UnitTests.SeedWork;

public sealed class ResultAndErrorTests
{
    [Fact]
    public void Error_Failure_Should_CreateErrorWithFailureType()
    {
        // Arrange
        // Act
        Error error = Error.Failure("Some.Code", "Some description");

        // Assert
        error.Type.ShouldBe(ErrorType.Failure);
        error.Code.ShouldBe("Some.Code");
        error.Description.ShouldBe("Some description");
    }

    [Fact]
    public void Result_Success_Should_ReturnSuccessfulResultWithNoError()
    {
        // Arrange
        // Act
        Result result = Result.Success();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void Result_Failure_Should_ReturnFailedResultWithGivenError()
    {
        // Arrange
        Error error = Error.Validation("Some.Code", "Some description");

        // Act
        Result result = Result.Failure(error);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void ResultOfT_Value_Should_ReturnValue_When_ResultIsSuccessful()
    {
        // Arrange
        Result<int> result = Result.Success(42);

        // Act
        int value = result.Value;

        // Assert
        value.ShouldBe(42);
    }

    [Fact]
    public void ResultOfT_Value_Should_ThrowInvalidOperationException_When_ResultIsFailure()
    {
        // Arrange
        Result<int> result = Result.Failure<int>(Error.Validation("Some.Code", "Some description"));

        // Act
        Action act = () => _ = result.Value;

        // Assert
        Should.Throw<InvalidOperationException>(act);
    }

    [Fact]
    public void ResultOfT_ImplicitOperator_Should_WrapValueAsSuccessfulResult()
    {
        // Arrange
        // Act
        Result<string> result = "payload";

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("payload");
    }
}
