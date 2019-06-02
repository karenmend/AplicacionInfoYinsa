using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

namespace INFOYINSA_Mina
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TEquipo : ContentPage
	{
        List<string> equiposListaNom = new List<string>();
        List<string> equiposListaKey = new List<string>();
        string keyE;
        static string cadenaConexion = @"Data Source=192.168.1.73;Initial Catalog=INFOYINSA;Integrated Security = false;User ID = infoYinsa;Password = yinsa2;";

        public TEquipo ()
		{
			InitializeComponent ();
            Equipos();

            btn_agregar.Clicked += btn_agregarClicked; 
            

        }

        private void btn_agregarClicked(object sender, EventArgs e)
        {
            if (validacion())
            {
                keyEquipos();
                //datePck_fecha.ToString();
                //lbl_equipos.Text = datePck_fecha.Date.ToString();
                
                try
                {
                    txtVacios();
                   
                    SqlConnection conexion = new SqlConnection(cadenaConexion);//cadena conexion

                    
                    string cmdtxt = "if Exists (select * from TRABAJO_EQUIPO where Equipo_Key=" + keyE.ToString() + " and Fecha='" + datePck_fecha.Date.ToString("yyyyMMdd") + "')" +
                        " update TRABAJO_EQUIPO set HT_Equipo='" + txt_horasTrabajadas.Text + "', Bachadas='" + txt_bachadas.Text + "', MERMASPT='" + txt_mermaPT.Text + "' where Equipo_Key=" + keyE.ToString() +
                        " and Fecha='" + datePck_fecha.Date.ToString("yyyyMMdd") + "' else insert into TRABAJO_EQUIPO (Equipo_Key, Fecha, HT_Equipo, Bachadas, MERMASPT, H_Paro_Justif, H_Paro_No_Justif) values (" + keyE.ToString() +
                        ", '" + datePck_fecha.Date.ToString("yyyyMMdd") + "', '" + txt_horasTrabajadas.Text + "', '" + txt_bachadas.Text + "', '" + txt_mermaPT.Text + "',0 ,0)";
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(cmdtxt, conexion);
                    cmd.ExecuteReader();
                    conexion.Close();

                    //LimpiarHorasEquipo();
                    DisplayAlert("Atencion", "Carga Éxitosa", "OK");
                    Limpiar();
                }
                catch (Exception ex)
                {
                    DisplayAlert("Atención", ex.ToString(), "OK");
                    throw;
                }



            }
        }

        private void txtVacios()
        {
            if (string.IsNullOrWhiteSpace(txt_bachadas.Text))
            {
                txt_bachadas.Text = "0";
            }
            if(string.IsNullOrWhiteSpace(txt_horasTrabajadas.Text))
            {
                txt_horasTrabajadas.Text = "0";
            }
            if(string.IsNullOrWhiteSpace(txt_mermaPT.Text))
            {
                txt_mermaPT.Text = "0";
            }
        }

        public void Limpiar()
        {
            pck_equipos.SelectedIndex = -1;
            txt_bachadas.Text = string.Empty;
            txt_horasTrabajadas.Text = string.Empty;
            txt_mermaPT.Text = string.Empty;
        }
        public void keyEquipos()
        {
            DataTable dt = new DataTable();
            SqlConnection conexion = new SqlConnection(cadenaConexion);//cadena conexion

            conexion.Open();
            string consulta = "SELECT Equipo_Key FROM Equipo WHERE Nom_Equipo = '" + pck_equipos.SelectedItem.ToString() + "'"; //consulta a la tabla 
            SqlCommand comando = new SqlCommand(consulta, conexion);

            SqlDataAdapter adap = new SqlDataAdapter(comando);
            adap.Fill(dt);

            
            keyE = dt.Rows[0].ItemArray[0].ToString();

            dt.Clear();
            conexion.Close();
         
        }
        bool validacion()
        {
            if(pck_equipos.SelectedItem == null)
            {
                DisplayAlert("Advertencia", "Necesitas seleccionar un equipo.", "OK");
                return false;
            }
            if (!txt_horasTrabajadas.Text.ToCharArray().All(Char.IsDigit))
            {
                DisplayAlert("Advertencia", "El formato de Horas Trabajadas es incorrecto, solo se aceptan numeros.", "OK");
                return false;
            }
            else if (!txt_bachadas.Text.ToCharArray().All(Char.IsDigit))
            {
                DisplayAlert("Advertencia", "El formato de Bachadas es incorrecto, solo se aceptan numeros.", "OK");
                return false;
            }
            else if (!txt_mermaPT.Text.ToCharArray().All(Char.IsDigit))
            {
                DisplayAlert("Advertencia", "El formato de Merpa PT es incorrecto, solo se aceptan numeros.", "OK");
                return false;
            }
            else if(string.IsNullOrWhiteSpace(txt_bachadas.Text) && string.IsNullOrWhiteSpace(txt_horasTrabajadas.Text) && string.IsNullOrWhiteSpace(txt_mermaPT.Text))
            {
                DisplayAlert("Advertencia", "Faltan campos por llenar.", "OK");
                return false;
            }
            return true;
        }
        public void  Equipos()
        {
            DataTable dt = new DataTable();
            SqlConnection conexion = new SqlConnection(cadenaConexion);//cadena conexion
           
            conexion.Open();
            string consulta = "SELECT Equipo_Key,Nom_Equipo FROM Equipo"; //consulta a la tabla 
            SqlCommand comando = new SqlCommand(consulta, conexion);

            SqlDataAdapter adap = new SqlDataAdapter(comando);
            adap.Fill(dt);
         
            
            for(int i=0; i<dt.Rows.Count; i++)
            {
                equiposListaKey.Add(dt.Rows[i].ItemArray[0].ToString());
                equiposListaNom.Add(dt.Rows[i].ItemArray[1].ToString());
                Debug.WriteLine(dt.Rows[i].ItemArray[1].ToString());
            }
            
           pck_equipos.ItemsSource = equiposListaNom;
            dt.Clear();
           conexion.Close();
        }
    }
    
}