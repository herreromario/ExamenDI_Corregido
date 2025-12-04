using di.examen._1EV._2025.Backend.Modelos;
using Microsoft.EntityFrameworkCore;

namespace di.examen._1EV._2025.Backend.Repositorios
{
    /// <summary>
    /// Implementación del repositorio para Producto, reutiliza GenericRepository.
    /// </summary>
    public class ProductoRepository : GenericRepository<Producto>, IProductoRepository
    {
        public ProductoRepository(DbContext context) : base(context)
        {
        }

        public async Task<bool> CodigoProductoExisteAsync(string codigoProducto)
        {
            // Validar entrada

            if (string.IsNullOrWhiteSpace(codigoProducto))
                return false;

            // Consultar si existe algún producto con el código dado
            var list = await QueryAsync(
                q => q.Where(p => p.CodigoProducto == codigoProducto).Take(1)
            );

            // Retornar true si se encontró al menos un producto
            return list.Any();
        }
    }
}
