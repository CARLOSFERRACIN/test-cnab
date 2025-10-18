using CnabProcessor.Helpers;
using FluentAssertions;
using Xunit;

namespace CnabProcessor.Tests.Helpers;

public class TransactionTypeHelperTests
{
    #region IsDebitTransaction Tests

    [Theory]
    [InlineData(2)] // Boleto
    [InlineData(3)] // Financing
    [InlineData(9)] // Rent
    public void IsDebitTransaction_ValidDebitTypes_ShouldReturnTrue(int type)
    {
        // Act
        var result = TransactionTypeHelper.IsDebitTransaction(type);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)] // Debit
    [InlineData(4)] // Credit
    [InlineData(5)] // Loan Receipt
    [InlineData(6)] // Sales
    [InlineData(7)] // TED Receipt
    [InlineData(8)] // DOC Receipt
    public void IsDebitTransaction_ValidCreditTypes_ShouldReturnFalse(int type)
    {
        // Act
        var result = TransactionTypeHelper.IsDebitTransaction(type);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(99)]
    [InlineData(-1)]
    public void IsDebitTransaction_InvalidTypes_ShouldReturnFalse(int type)
    {
        // Act
        var result = TransactionTypeHelper.IsDebitTransaction(type);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetEffectiveAmount Tests

    [Theory]
    [InlineData(2, 100.50, -100.50)] // Boleto (debit)
    [InlineData(3, 250.75, -250.75)] // Financing (debit)
    [InlineData(9, 50.00, -50.00)]   // Rent (debit)
    public void GetEffectiveAmount_DebitTypes_ShouldReturnNegativeAmount(int type, decimal amount, decimal expected)
    {
        // Act
        var result = TransactionTypeHelper.GetEffectiveAmount(type, amount);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 100.50, 100.50)]  // Debit (credit)
    [InlineData(4, 250.75, 250.75)] // Credit (credit)
    [InlineData(5, 50.00, 50.00)]   // Loan Receipt (credit)
    [InlineData(6, 75.25, 75.25)]   // Sales (credit)
    [InlineData(7, 200.00, 200.00)] // TED Receipt (credit)
    [InlineData(8, 150.50, 150.50)] // DOC Receipt (credit)
    public void GetEffectiveAmount_CreditTypes_ShouldReturnPositiveAmount(int type, decimal amount, decimal expected)
    {
        // Act
        var result = TransactionTypeHelper.GetEffectiveAmount(type, amount);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(2, -100.50, -100.50)] // Boleto with negative amount
    [InlineData(1, -100.50, 100.50)] // Debit with negative amount
    public void GetEffectiveAmount_NegativeAmounts_ShouldReturnCorrectSign(int type, decimal amount, decimal expected)
    {
        // Act
        var result = TransactionTypeHelper.GetEffectiveAmount(type, amount);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 100.50, 100.50)]
    [InlineData(10, 100.50, 100.50)]
    [InlineData(99, 100.50, 100.50)]
    public void GetEffectiveAmount_InvalidTypes_ShouldReturnPositiveAmount(int type, decimal amount, decimal expected)
    {
        // Act
        var result = TransactionTypeHelper.GetEffectiveAmount(type, amount);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region GetTransactionTypeDescription Tests

    [Theory]
    [InlineData(1, "Debit")]
    [InlineData(2, "Boleto")]
    [InlineData(3, "Financing")]
    [InlineData(4, "Credit")]
    [InlineData(5, "Loan Receipt")]
    [InlineData(6, "Sales")]
    [InlineData(7, "TED Receipt")]
    [InlineData(8, "DOC Receipt")]
    [InlineData(9, "Rent")]
    public void GetTransactionTypeDescription_ValidTypes_ShouldReturnCorrectDescription(int type, string expected)
    {
        // Act
        var result = TransactionTypeHelper.GetTransactionTypeDescription(type);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, "Type 0")]
    [InlineData(10, "Type 10")]
    [InlineData(99, "Type 99")]
    [InlineData(-1, "Type -1")]
    public void GetTransactionTypeDescription_InvalidTypes_ShouldReturnTypeWithNumber(int type, string expected)
    {
        // Act
        var result = TransactionTypeHelper.GetTransactionTypeDescription(type);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region GetNature Tests

    [Theory]
    [InlineData(1, "In")]  // Debit
    [InlineData(4, "In")]  // Credit
    [InlineData(5, "In")]  // Loan Receipt
    [InlineData(6, "In")]  // Sales
    [InlineData(7, "In")]  // TED Receipt
    [InlineData(8, "In")]  // DOC Receipt
    public void GetNature_CreditTypes_ShouldReturnIn(int type, string expected)
    {
        // Act
        var result = TransactionTypeHelper.GetNature(type);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(2, "Out")] // Boleto
    [InlineData(3, "Out")] // Financing
    [InlineData(9, "Out")] // Rent
    public void GetNature_DebitTypes_ShouldReturnOut(int type, string expected)
    {
        // Act
        var result = TransactionTypeHelper.GetNature(type);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, "Unknown")]
    [InlineData(10, "Unknown")]
    [InlineData(99, "Unknown")]
    [InlineData(-1, "Unknown")]
    public void GetNature_InvalidTypes_ShouldReturnUnknown(int type, string expected)
    {
        // Act
        var result = TransactionTypeHelper.GetNature(type);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region GetSign Tests

    [Theory]
    [InlineData(1, "+")]  // Debit
    [InlineData(4, "+")]  // Credit
    [InlineData(5, "+")]  // Loan Receipt
    [InlineData(6, "+")]  // Sales
    [InlineData(7, "+")]  // TED Receipt
    [InlineData(8, "+")]  // DOC Receipt
    public void GetSign_CreditTypes_ShouldReturnPlus(int type, string expected)
    {
        // Act
        var result = TransactionTypeHelper.GetSign(type);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(2, "-")] // Boleto
    [InlineData(3, "-")] // Financing
    [InlineData(9, "-")] // Rent
    public void GetSign_DebitTypes_ShouldReturnMinus(int type, string expected)
    {
        // Act
        var result = TransactionTypeHelper.GetSign(type);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, "?")]
    [InlineData(10, "?")]
    [InlineData(99, "?")]
    [InlineData(-1, "?")]
    public void GetSign_InvalidTypes_ShouldReturnQuestionMark(int type, string expected)
    {
        // Act
        var result = TransactionTypeHelper.GetSign(type);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void AllMethods_ShouldBeConsistent()
    {
        // Arrange
        var testTypes = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        foreach (var type in testTypes)
        {
            // Act
            var isDebit = TransactionTypeHelper.IsDebitTransaction(type);
            var nature = TransactionTypeHelper.GetNature(type);
            var sign = TransactionTypeHelper.GetSign(type);

            // Assert
            if (isDebit)
            {
                nature.Should().Be("Out");
                sign.Should().Be("-");
            }
            else
            {
                nature.Should().Be("In");
                sign.Should().Be("+");
            }
        }
    }

    [Fact]
    public void GetEffectiveAmount_ShouldBeConsistentWithIsDebitTransaction()
    {
        // Arrange
        var testCases = new[]
        {
            (Type: 1, Amount: 100.50m, ExpectedEffective: 100.50m),
            (Type: 2, Amount: 100.50m, ExpectedEffective: -100.50m),
            (Type: 3, Amount: 250.75m, ExpectedEffective: -250.75m),
            (Type: 4, Amount: 50.00m, ExpectedEffective: 50.00m),
            (Type: 5, Amount: 75.25m, ExpectedEffective: 75.25m),
            (Type: 6, Amount: 200.00m, ExpectedEffective: 200.00m),
            (Type: 7, Amount: 150.50m, ExpectedEffective: 150.50m),
            (Type: 8, Amount: 300.00m, ExpectedEffective: 300.00m),
            (Type: 9, Amount: 125.75m, ExpectedEffective: -125.75m)
        };

        foreach (var (type, amount, expectedEffective) in testCases)
        {
            // Act
            var isDebit = TransactionTypeHelper.IsDebitTransaction(type);
            var effectiveAmount = TransactionTypeHelper.GetEffectiveAmount(type, amount);

            // Assert
            if (isDebit)
            {
                effectiveAmount.Should().BeNegative();
                effectiveAmount.Should().Be(-Math.Abs(amount));
            }
            else
            {
                effectiveAmount.Should().BePositive();
                effectiveAmount.Should().Be(Math.Abs(amount));
            }

            effectiveAmount.Should().Be(expectedEffective);
        }
    }

    #endregion

    #region Edge Cases

    [Theory]
    [InlineData(0)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void AllMethods_EdgeCaseValues_ShouldNotThrow(int type)
    {
        // Act & Assert
        var isDebit = () => TransactionTypeHelper.IsDebitTransaction(type);
        var effectiveAmount = () => TransactionTypeHelper.GetEffectiveAmount(type, 100.50m);
        var description = () => TransactionTypeHelper.GetTransactionTypeDescription(type);
        var nature = () => TransactionTypeHelper.GetNature(type);
        var sign = () => TransactionTypeHelper.GetSign(type);

        isDebit.Should().NotThrow();
        effectiveAmount.Should().NotThrow();
        description.Should().NotThrow();
        nature.Should().NotThrow();
        sign.Should().NotThrow();
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(999999.99)]
    [InlineData(-999999.99)]
    [InlineData(0)]
    public void GetEffectiveAmount_EdgeCaseAmounts_ShouldHandleCorrectly(decimal amount)
    {
        // Arrange
        var creditType = 1; // Debit (credit nature)
        var debitType = 2;  // Boleto (debit nature)

        // Act
        var creditResult = TransactionTypeHelper.GetEffectiveAmount(creditType, amount);
        var debitResult = TransactionTypeHelper.GetEffectiveAmount(debitType, amount);

        // Assert
        creditResult.Should().Be(Math.Abs(amount));
        debitResult.Should().Be(-Math.Abs(amount));
    }

    #endregion
}
