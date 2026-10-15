using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Adif.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "abonos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    venta_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    compra_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    monto = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    metodo = table.Column<int>(type: "integer", nullable: false),
                    comentario = table.Column<string>(type: "text", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_abonos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "adif_tbCodigos",
                columns: table => new
                {
                    codg_codigo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    codg_usado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adif_tbCodigos", x => x.codg_codigo);
                });

            migrationBuilder.CreateTable(
                name: "adif_tbLogs",
                columns: table => new
                {
                    user_username = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    logs_fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    logs_telefono = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    logs_accion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adif_tbLogs", x => new { x.user_username, x.logs_fecha, x.logs_telefono });
                });

            migrationBuilder.CreateTable(
                name: "adif_tbUsuarios",
                columns: table => new
                {
                    user_username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    logs_telefono = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    user_bloqueado = table.Column<bool>(type: "boolean", nullable: false),
                    user_google = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, defaultValue: ""),
                    user_mensualidad = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    user_cantidad = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adif_tbUsuarios", x => new { x.user_username, x.logs_telefono });
                });

            migrationBuilder.CreateTable(
                name: "caja_movimientos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    monto = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    concepto = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false, defaultValue: ""),
                    usuario = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false, defaultValue: ""),
                    saldo_resultante = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_caja_movimientos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false, defaultValue: ""),
                    icono = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "compras",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    socios_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    fecha_vence = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    total = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    es_credito = table.Column<bool>(type: "boolean", nullable: false),
                    saldo = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    metodo = table.Column<int>(type: "integer", nullable: false),
                    comentario = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    estado = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compras", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "compras_detalles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    compra_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    producto_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    precio = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    cantidad = table.Column<decimal>(type: "numeric(12,3)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compras_detalles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "especialidades",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    especialidad = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_especialidades", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "eventos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    titulo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false, defaultValue: ""),
                    descripcion = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    hora = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: ""),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gastos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    categoria = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false, defaultValue: ""),
                    descripcion = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    monto = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    metodo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gastos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "negocios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false, defaultValue: ""),
                    propietario = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, defaultValue: ""),
                    telefono = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: ""),
                    direccion = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    rtn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: ""),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_negocios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "objetos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    objeto = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false, defaultValue: ""),
                    estado = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_objetos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false, defaultValue: ""),
                    precio = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    costo = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    stock = table.Column<decimal>(type: "numeric(12,3)", nullable: false, defaultValue: 0m),
                    cateid = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    notas = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    img_blob = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, defaultValue: ""),
                    espeid = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    codigo_barra = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: ""),
                    unidad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: ""),
                    precio_mayoreo = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    stock_minimo = table.Column<decimal>(type: "numeric(12,3)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "socios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false, defaultValue: ""),
                    dni = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: ""),
                    telefono = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: ""),
                    notas = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    es_proveedor = table.Column<bool>(type: "boolean", nullable: false),
                    favorito = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_socios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    usuario = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false, defaultValue: ""),
                    bloqueado = table.Column<bool>(type: "boolean", nullable: false),
                    google = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, defaultValue: ""),
                    mensualidad = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: ""),
                    cantidad = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ventas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    socios_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    fecha_vence = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    total = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    es_credito = table.Column<bool>(type: "boolean", nullable: false),
                    saldo = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    metodo = table.Column<int>(type: "integer", nullable: false),
                    comentario = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    isv = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ventas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "negocio_whatsapp",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false),
                    phone_number_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: ""),
                    display_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: ""),
                    waba_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: ""),
                    webhook_secret = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false, defaultValue: ""),
                    kapso_api_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false, defaultValue: ""),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_negocio_whatsapp", x => x.id);
                    table.ForeignKey(
                        name: "FK_negocio_whatsapp_negocios_negocio_id",
                        column: x => x.negocio_id,
                        principalTable: "negocios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "whatsapp_operadores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false),
                    telefono = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: ""),
                    nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false, defaultValue: ""),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_whatsapp_operadores", x => x.id);
                    table.ForeignKey(
                        name: "FK_whatsapp_operadores_negocios_negocio_id",
                        column: x => x.negocio_id,
                        principalTable: "negocios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ventas_detalles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    negocio_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    venta_id = table.Column<int>(type: "integer", nullable: false),
                    producto_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    precio = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    costo = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    cantidad = table.Column<decimal>(type: "numeric(12,3)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ventas_detalles", x => x.id);
                    table.ForeignKey(
                        name: "FK_ventas_detalles_ventas_venta_id",
                        column: x => x.venta_id,
                        principalTable: "ventas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_abonos_compra_id",
                table: "abonos",
                column: "compra_id");

            migrationBuilder.CreateIndex(
                name: "IX_abonos_negocio_id",
                table: "abonos",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_abonos_venta_id",
                table: "abonos",
                column: "venta_id");

            migrationBuilder.CreateIndex(
                name: "IX_adif_tbUsuarios_user_google",
                table: "adif_tbUsuarios",
                column: "user_google");

            migrationBuilder.CreateIndex(
                name: "IX_caja_movimientos_negocio_id",
                table: "caja_movimientos",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_categorias_negocio_id",
                table: "categorias",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_compras_negocio_id",
                table: "compras",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_compras_detalles_compra_id",
                table: "compras_detalles",
                column: "compra_id");

            migrationBuilder.CreateIndex(
                name: "IX_compras_detalles_negocio_id",
                table: "compras_detalles",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_especialidades_negocio_id",
                table: "especialidades",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_eventos_negocio_id",
                table: "eventos",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_gastos_negocio_id",
                table: "gastos",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_negocio_whatsapp_negocio_id",
                table: "negocio_whatsapp",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_negocio_whatsapp_phone_number_id",
                table: "negocio_whatsapp",
                column: "phone_number_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_negocios_propietario",
                table: "negocios",
                column: "propietario");

            migrationBuilder.CreateIndex(
                name: "IX_objetos_negocio_id",
                table: "objetos",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_productos_negocio_id",
                table: "productos",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_productos_negocio_id_codigo_barra",
                table: "productos",
                columns: new[] { "negocio_id", "codigo_barra" });

            migrationBuilder.CreateIndex(
                name: "IX_socios_negocio_id",
                table: "socios",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_negocio_id",
                table: "usuarios",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_negocio_id",
                table: "ventas",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_negocio_id_socios_id",
                table: "ventas",
                columns: new[] { "negocio_id", "socios_id" });

            migrationBuilder.CreateIndex(
                name: "IX_ventas_detalles_negocio_id",
                table: "ventas_detalles",
                column: "negocio_id");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_detalles_venta_id",
                table: "ventas_detalles",
                column: "venta_id");

            migrationBuilder.CreateIndex(
                name: "IX_whatsapp_operadores_negocio_id_telefono",
                table: "whatsapp_operadores",
                columns: new[] { "negocio_id", "telefono" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "abonos");

            migrationBuilder.DropTable(
                name: "adif_tbCodigos");

            migrationBuilder.DropTable(
                name: "adif_tbLogs");

            migrationBuilder.DropTable(
                name: "adif_tbUsuarios");

            migrationBuilder.DropTable(
                name: "caja_movimientos");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "compras");

            migrationBuilder.DropTable(
                name: "compras_detalles");

            migrationBuilder.DropTable(
                name: "especialidades");

            migrationBuilder.DropTable(
                name: "eventos");

            migrationBuilder.DropTable(
                name: "gastos");

            migrationBuilder.DropTable(
                name: "negocio_whatsapp");

            migrationBuilder.DropTable(
                name: "objetos");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "socios");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "ventas_detalles");

            migrationBuilder.DropTable(
                name: "whatsapp_operadores");

            migrationBuilder.DropTable(
                name: "ventas");

            migrationBuilder.DropTable(
                name: "negocios");
        }
    }
}
