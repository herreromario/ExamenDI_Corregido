using di.examen._1EV._2025.Backend.Modelos;
using di.examen._1EV._2025.Backend.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace di.examen._1EV._2025.Frontend.Dialogos
{
    /// <summary>
    /// Lógica de interacción para DialogoGestionProductos.xaml
    /// </summary>
    public partial class DialogoGestionProductos : Window
    {
        // Declaración de variables de contexto y repositorios

        private JardineriaContext _context;
        private ProductoRepository _productoRepository;
        private GamaProductoRepository _gamaProductoRepository;


        public DialogoGestionProductos()
        {
            InitializeComponent();
            Loaded += DialogoGestionProductos_Loaded;
        }

        private async void DialogoGestionProductos_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicialización del contexto y repositorios
            _context = new JardineriaContext();
            _productoRepository = new ProductoRepository(_context);
            _gamaProductoRepository = new GamaProductoRepository(_context);

            // Cargar datos en los combos
            await CargarCombosAsync();
        }

        private async Task CargarCombosAsync()
        {
            // Cargar gamas de producto
            List<Gamasproducto> gamasProducto;

            // Crear un nuevo contexto para evitar problemas de concurrencia
            using (var context = new JardineriaContext())
            {
                // Crear el repositorio de gamas de producto
                var gamaProductoRepo = new GamaProductoRepository(context);

                // Obtener todas las gamas de producto
                gamasProducto = (List<Gamasproducto>) await gamaProductoRepo.GetAllAsync();
            }

            // Asignar las gamas de producto al ComboBox
            cmbGama.ItemsSource = gamasProducto;
        }

        private void RecogerDatosProducto(Producto producto)
        {
            // Recoger datos del formulario y asignarlos al objeto producto

            producto.CodigoProducto = txtCodigo.Text;
            producto.Nombre = txtNombre.Text;
            producto.Proveedor = txtProveedor.Text;
            producto.PrecioVenta = decimal.Parse(txtPrecio.Text);
            producto.CantidadEnStock = short.Parse(txtCantidad.Text);

            // Obtener la gama seleccionada en el ComboBox
            if (cmbGama.SelectedItem != null)
            {
                var gama = (Gamasproducto)cmbGama.SelectedItem;
                producto.Gama = gama.Gama;
            }
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var context = new JardineriaContext();
                var repo = new ProductoRepository(context);

                if (await repo.CodigoProductoExisteAsync(txtCodigo.Text.Trim()))
                {
                    MessageBox.Show("El código de producto ya existe. Por favor, ingrese un código diferente.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                Producto producto = new Producto();
                RecogerDatosProducto(producto);

                await repo.AddAsync(producto);
                context.SaveChanges();

                DialogResult = true;
                Close();

                MessageBox.Show("Producto guardado correctamente.",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el producto: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
