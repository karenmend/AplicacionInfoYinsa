using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;


namespace INFOYINSA_Mina
{
    public partial class MainPage : ContentPage
    {
        Entry txt_usuarioTx = new Entry();
        Entry txt_contrasenaTx = new Entry();
        static string cadenaConexion = @"Data Source=192.168.1.73;Initial Catalog=INFOYINSA;Integrated Security = false;User ID = infoYinsa;Password = yinsa2;";
        public MainPage()
        {
            InitializeComponent();
          
            btn_login.Clicked += Btn_login_Clicked;
        }

        private void Btn_login_Clicked(object sender, EventArgs e)
        {
           

           inicializarTexts();
           Login();


        }

        private void Login()
        {
           
            try
            {
                SqlConnection Conexion = new SqlConnection(cadenaConexion);
                Conexion.Open();

                SqlDataReader myReader = null;

                string Usuarios = "select Nombre,Contraseña,PCalidad,PCliente_Interno,PSeguridad,P5s,PTrabajo_Equipo,PRecursos,PMantenimiento, PTIndicadores, PLaboratorio from Usuarios where Nombre='" + txt_usuario.Text + "' and Contraseña='" + txt_contrasena.Text + "'";
                SqlCommand myCommand = new SqlCommand(Usuarios, Conexion);
                SqlDataAdapter Da = new SqlDataAdapter(myCommand);
                DataTable Dt = new DataTable();
                Da.Fill(Dt);
                myReader = myCommand.ExecuteReader();

                if (myReader.HasRows)
                {

                    App.Current.MainPage = new NavigationPage(new paginaPestañas());
                    Conexion.Close();
                }
                else
                {
                    
                    //MessageBox.Show("Usuario y/o Contraseña Incorrecta", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DisplayAlert("Atencion", "Usuario y/o Contraseña Incorrecta", "OK");
                    Conexion.Close();
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Atencion", ex.Message, "OK");
                throw;
                //MessageBox.Show(ex.Message);
               
            }
        }
        private void inicializarTexts()
        {
            txt_usuarioTx = txt_usuario;
            txt_contrasenaTx = txt_contrasena;
           
        }
    }
}
