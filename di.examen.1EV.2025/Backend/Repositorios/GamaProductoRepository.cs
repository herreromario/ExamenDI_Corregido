using di.examen._1EV._2025.Backend.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace di.examen._1EV._2025.Backend.Repositorios
{
    public class GamaProductoRepository : GenericRepository<Gamasproducto>, IGamaProductoRepository
    {
        public GamaProductoRepository(JardineriaContext context) : base(context)
        {
        }
    }
}
