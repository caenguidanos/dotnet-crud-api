namespace Common.Test.Domain.ValueObject;

using Common.Domain.ValueObject;

public class Primitive
{
    [Test]
    public void GivenStringPrimitiveWithValidation_WhenCreateInstance_ThenThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new StringPrimitiveWithValidation("1234"));
    }

    [Test]
    public void GivenStringPrimitiveWithValidation_WhenCreateInstance_ThenNotThrowArgumentOutOfRangeException()
    {
        Assert.DoesNotThrow(() => new StringPrimitiveWithValidation("123456"));
    }

    [Test]
    public void GivenStringPrimitiveWithValidation_WhenGetValue_ThenReturnsSameValue()
    {
        var primitive = new StringPrimitiveWithValidation("123499");

        string actual = primitive.GetValue();

        Assert.That(actual, Is.EqualTo("123499"));
    }

    private sealed class StringPrimitiveWithValidation : ValueObject<string>
    {
        public StringPrimitiveWithValidation(string value)
            : base(value)
        {
        }

        public override string Validate(string value)
        {
            int maxLength = 10;
            int minLength = 5;

            if (value.Length < minLength || value.Length > maxLength)
            {
                throw new ArgumentOutOfRangeException(value);
            }

            return value;
        }
    }
}