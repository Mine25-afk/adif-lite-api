// Cliente o proveedor de un negocio.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("socios")]
[Index(nameof(NegocioId))]
public class Socio : IPerteneceANegocio
{
    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("nombre"), MaxLength(120)] public string Nombre { get; set; } = string.Empty;
    [Column("dni"), MaxLength(30)] public string Dni { get; set; } = string.Empty;
    [Column("telefono"), MaxLength(30)] public string Telefono { get; set; } = string.Empty;
    [Column("notas")] public string Notas { get; set; } = string.Empty;
    [Column("es_proveedor")] public bool EsProveedor { get; set; }
    [Column("favorito")] public bool Favorito { get; set; }

    public static Socio NuevoCliente(int negocioId, string nombre, string? telefono)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new DomainException("El nombre del cliente es obligatorio.");
        return new Socio { NegocioId = negocioId, Nombre = nombre.Trim(), Telefono = telefono?.Trim() ?? string.Empty };
    }
}
