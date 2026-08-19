using System.ComponentModel.DataAnnotations;

namespace OnlineAccountingApp.Infrasructure.Options;

  public class RedisOptions
  {
    [Required]
    public string ConnectionString { get; set; }
  }