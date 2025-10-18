using CnabProcessor.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnabProcessor.Models.Entity;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int Type { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(11)]
    public string Cpf { get; set; } = string.Empty;

    [Required]
    [StringLength(12)]
    public string Card { get; set; } = string.Empty;

    [Required]
    public TimeSpan Time { get; set; }

    [Required]
    [StringLength(14)]
    public string StoreOwner { get; set; } = string.Empty;

    [Required]
    [StringLength(19)]
    public string StoreName { get; set; } = string.Empty;

    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;

    public string TypeDescription => GetTypeDescription(Type);
    public string Nature => GetNature(Type);
    public string Sign => GetSign(Type);

    private static string GetTypeDescription(int type) => TransactionTypeHelper.GetTransactionTypeDescription(type);

    private static string GetNature(int type) => TransactionTypeHelper.GetNature(type);

    private static string GetSign(int type) => TransactionTypeHelper.GetSign(type);
}
