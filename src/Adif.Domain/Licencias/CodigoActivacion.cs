// Código de activación de un solo uso.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;

namespace Adif.Domain.Entities;

[Table("adif_tbCodigos")]
public class CodigoActivacion
{
    [Key, Column("codg_codigo"), MaxLength(60)] public string Codigo { get; set; } = string.Empty;
    [Column("codg_usado")] public bool Usado { get; set; }

    public void Canjear()
    {
        if (Usado) throw new DomainException("El código ya fue utilizado.");
        Usado = true;
    }
}
