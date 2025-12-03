using di.examen._1EV._2025.Backend.Modelos;
using di.examen._1EV._2025.Backend.Repositorios;
using Microsoft.EntityFrameworkCore;
using System.Windows;
namespace di.examen._1EV._2025.Frontend.Dialogos
{
    /// <summary>
    /// Lógica de interacción para DialogoGestionEmpleados.xaml
    /// </summary>
    public partial class DialogoGestionEmpleados : Window
    {
        // Declaración de variables de contexto y repositorios
        private JardineriaContext _context;
        private EmpleadoRepository _empleadoRepository;
        private OficinaRepository _oficinaRepository;
        public DialogoGestionEmpleados()
        {
            InitializeComponent();
            Loaded += DialogoGestionEmpleados_Loaded;
        }

        private async void DialogoGestionEmpleados_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicialización del contexto y repositorios
            _context = new JardineriaContext();
            _empleadoRepository = new EmpleadoRepository(_context);
            _oficinaRepository = new OficinaRepository(_context);

            // Cargar datos en los combos
            await CargarCombosAsync();
        }

        private async Task CargarCombosAsync()
        {
            // Cargar oficinas
            List<Oficina> oficinas;
            // Crear un nuevo contexto para evitar problemas de concurrencia
            using (var context = new JardineriaContext())
            {
                // Crear un nuevo repositorio con el nuevo contexto
                var repo = new OficinaRepository(context);
                // Obtener todas las oficinas
                oficinas = (List<Oficina>)await repo.GetAllAsync();
            }

            // Asignar las oficinas al combo
            cmbOficina.ItemsSource = oficinas;

            // Cargar empleados para el combo de jefes
            List<Empleado> empleados;
            // Crear un nuevo contexto para evitar problemas de concurrencia
            using (var context = new JardineriaContext())
            {
                // Crear un nuevo repositorio con el nuevo contexto
                var repo = new EmpleadoRepository(context);
                // Obtener todos los empleados
                empleados = (List<Empleado>)await repo.GetAllAsync();
            }

            // Asignar los empleados al combo
            cmbJefe.ItemsSource = empleados;
        }

        private void RecogerDatosEmpleado(Empleado empleado)
        {
            // Recoger datos del formulario y asignarlos al objeto empleado
            empleado.Nombre = txtNombre.Text;
            empleado.Apellido1 = txtApellido1.Text;
            empleado.Apellido2 = txtApellido2.Text;
            empleado.Email = txtEmail.Text;
            empleado.Extension = txtExtension.Text;
            empleado.Puesto = txtPuesto.Text;


           if (cmbOficina.SelectedItem != null)
           {
                // Obtener la oficina seleccionada y asignar su código al empleado
                var oficina = (Oficina)cmbOficina.SelectedItem;
                empleado.CodigoOficina = oficina.CodigoOficina;
            }

             if (cmbJefe.SelectedItem != null)
            {
                // Obtener el jefe seleccionado y asignar su código al empleado
                var jefe = (Empleado)cmbJefe.SelectedItem;
                empleado.CodigoJefe = jefe.CodigoEmpleado;
            }   
        }

        // Obtener el siguiente código de empleado disponible
        public async Task<int> GetNextCodigoEmpleadoAsync()
        {
            // Obtener el código máximo actual y sumar 1
            int maxCodigo = await _context.Empleados
                                          .MaxAsync(e => (int?)e.CodigoEmpleado) ?? 0;
            return maxCodigo + 1;
        }


        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido1.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtExtension.Text) ||
                cmbOficina.SelectedItem == null)
            {
                MessageBox.Show("Rellena todos los campos obligatorios.",
                                "Aviso",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
            {
                return;
            }

            try
            {
                // Crear un nuevo contexto y repositorio para la operación
                using var context = new JardineriaContext();
                var repo = new EmpleadoRepository(context);

                Empleado nuevoEmpleado = new Empleado();

                RecogerDatosEmpleado(nuevoEmpleado);

                // Asignar el siguiente código de empleado disponible
                nuevoEmpleado.CodigoEmpleado = await GetNextCodigoEmpleadoAsync();

                // Añadir el nuevo empleado a la base de datos
                await repo.AddAsync(nuevoEmpleado);
                context.SaveChanges();

                MessageBox.Show("Empleado guardado correctamente.",
                                "Información",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                // Cerrar el diálogo con resultado positivo
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el empleado: " + ex.Message,
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            // Cerrar el diálogo con resultado negativo
            DialogResult = false;
            Close();
        }

    }
}
