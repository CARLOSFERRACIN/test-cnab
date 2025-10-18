namespace CnabProcessor.Helpers;

public static class TransactionTypeHelper
{
    /// <summary>
    /// Determines if a transaction type is a debit transaction
    /// </summary>
    /// <param name="type">Transaction type</param>
    /// <returns>True if it's a debit transaction, false otherwise</returns>
    public static bool IsDebitTransaction(int type)
    {
        // Baseado no padrão CNAB:
        // Tipos de débito: 2 (Débito), 3 (Financiamento), 9 (Desconto)
        // Tipos de crédito: 1 (Débito), 4 (Crédito), 5 (Recebimento Empréstimo), 6 (Vendas), 7 (Recebimento TED), 8 (Recebimento DOC)
        return type switch
        {
            2 or 3 or 9 => true,  // Débito
            1 or 4 or 5 or 6 or 7 or 8 => false,  // Crédito
            _ => false
        };
    }

    /// <summary>
    /// Calculates the effective amount for a transaction based on its type
    /// </summary>
    /// <param name="type">Transaction type</param>
    /// <param name="amount">Original amount</param>
    /// <returns>Effective amount (positive for credit, negative for debit)</returns>
    public static decimal GetEffectiveAmount(int type, decimal amount)
    {
        return IsDebitTransaction(type) ? -Math.Abs(amount) : Math.Abs(amount);
    }

    /// <summary>
    /// Gets the transaction type description
    /// </summary>
    /// <param name="type">Transaction type</param>
    /// <returns>Human-readable description</returns>
    public static string GetTransactionTypeDescription(int type)
    {
        return type switch
        {
            1 => "Debit",
            2 => "Boleto",
            3 => "Financing",
            4 => "Credit",
            5 => "Loan Receipt",
            6 => "Sales",
            7 => "TED Receipt",
            8 => "DOC Receipt",
            9 => "Rent",
            _ => $"Type {type}"
        };
    }

    /// <summary>
    /// Gets the transaction nature (In/Out)
    /// </summary>
    /// <param name="type">Transaction type</param>
    /// <returns>Nature of the transaction</returns>
    public static string GetNature(int type)
    {
        return type switch
        {
            1 or 4 or 5 or 6 or 7 or 8 => "In",
            2 or 3 or 9 => "Out",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Gets the transaction sign (+/-)
    /// </summary>
    /// <param name="type">Transaction type</param>
    /// <returns>Sign of the transaction</returns>
    public static string GetSign(int type)
    {
        return type switch
        {
            1 or 4 or 5 or 6 or 7 or 8 => "+",
            2 or 3 or 9 => "-",
            _ => "?"
        };
    }
}
