using System.ComponentModel.DataAnnotations;
using CnabProcessor.Helpers;

namespace CnabProcessor.Models.Entity;

public class Store
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(14)]
    public string Owner { get; set; } = string.Empty;
    
    [Required]
    [StringLength(19)]
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    
    public decimal Balance => Transactions.Sum(t => 
        TransactionTypeHelper.GetEffectiveAmount(t.Type, t.Amount));
}
