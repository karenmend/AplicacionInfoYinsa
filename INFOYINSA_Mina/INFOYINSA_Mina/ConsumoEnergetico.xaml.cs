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
	public partial class ConsumoEnergetico : ContentPage
	{
        List<string> fuentesEner = new List<string>();
        List<string> equiposListaNom = new List<string>();
        static string cadenaConexion = @"Data Source=192.168.1.73;Initial Catalog=INFOYINSA;Integrated Security = false;User ID = infoYinsa;Password = yinsa2;";
        string keyE;
        string keyF;
        public ConsumoEnergetico ()
		{
			InitializeComponent ();
            Equipos();
            FuentesEnergia();
            btn_agregar.Clicked += btn_agregarClicked;
            
        }

        private void btn_agregarClicked(object sender, EventArgs e)
        {
            if (Validacion())
            {
                
                    keyEquipos();
                    keyFuentes();
                    try
                    {
                        
                            SqlConnection Conexion = new SqlConnection(cadenaConexion);//cadena conexion

                            string cmdtxt = "select * from TRABAJO_EQUIPO where Equipo_Key=" + keyE.ToString() + " and Fecha='" + datePck_fecha.Date.ToString("yyyyMMdd") + "'";
                            Conexion.Open();
                            SqlCommand cmd = new SqlCommand(cmdtxt, Conexion);
                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {

                                Conexion.Close();
                                cmdtxt = "if exists(select * from ConsumoEquipo where EquipoKey=" + keyE.ToString() + " and Fecha='" + datePck_fecha.Date.ToString("yyyyMMdd") + "' and FuenteEnergKey=" +
                                    keyF.ToString() + ") update ConsumoEquipo set Consumo=" + txt_consumo.Text + " where EquipoKey=" + keyE.ToString() + " and Fecha='" + datePck_fecha.Date.ToString("yyyyMMdd") + "' and FuenteEnergKey=" +
                                    keyF.ToString() + " else insert into ConsumoEquipo (EquipoKey,Fecha,FuenteEnergKey,Consumo) values (" + keyE.ToString() + ", '" + datePck_fecha.Date.ToString("yyyyMMdd") + "', " +
                                    keyF.ToString() + ", " + txt_consumo.Text + ")";
                                //cmdtxt = "insert into ConsumoEquipo (EquipoKey,Fecha,FuenteEnergKey,Consumo) values (" + CmbEquipo.SelectedValue + ", '" + Fecha.ToString("yyyyMMdd") + "', " +
                                //    CmbFuente.SelectedValue + ", " + TxtConsumo.Text + ")";
                                Conexion.Open();
                                cmd = new SqlCommand(cmdtxt, Conexion);
                                cmd.ExecuteReader();
                                Conexion.Close();
                                DisplayAlert("Atencion", "Carga Éxitosa", "OK");
                                Limpiar();


                            }
                            else
                            {
                                Conexion.Close();
                                DisplayAlert("Atención", "No se puede agregar Consumo porque no hay registro en Trabajo de Equipo para la Fecha: " + datePck_fecha.Date.ToString("dd/MM/yyyy"), "OK");

                            }
                        }
                        
                    catch (Exception Ex)
                    {
                        DisplayAlert("ERROR", Ex.Message, "OK");
                        throw;
                    }
                }
            else
            {
                DisplayAlert("Atención", "Falta Consumo ERROR FATALITY", "OK");
            }
        }
        public void txtVacios()
        {
            if (string.IsNullOrWhiteSpace(txt_consumo.Text))
            {
                txt_consumo.Text = "0";
            }
        }
        public void Limpiar()
        {
            pck_equipos.SelectedIndex = -1;
            pck_fuenteEnergetica.SelectedIndex = -1;
            txt_consumo.Text = string.Empty;
        }
        bool Validacion()
        {
            if (pck_equipos.SelectedItem == null)
            {
                DisplayAlert("Advertencia", "Necesitas seleccionar un equipo.", "OK");
                return false;
            }
            if (pck_fuenteEnergetica.SelectedItem == null)
            {
                DisplayAlert("Advertencia", "Necesitas seleccionar una fuente energética.", "OK");
                return false;
            }
            string Str = txt_consumo.Text.Trim();
            double Num;
            bool isNum = double.TryParse(Str, out Num);
            if (!isNum)
            {
                DisplayAlert("Advertencia", "El formato de Consumo es incorrecto, solo se aceptan numeros.", "OK");
                return false;
            }
            return true; 
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
        public void keyFuentes()
        {
            DataTable dt = new DataTable();
            SqlConnection conexion = new SqlConnection(cadenaConexion);//cadena conexion

            conexion.Open();
            string consulta = "SELECT FuenteEnergKey FROM FuentesEnergia WHERE FuenteEnergDesc = '" + pck_fuenteEnergetica.SelectedItem.ToString() + "'"; //consulta a la tabla 
            SqlCommand comando = new SqlCommand(consulta, conexion);

            SqlDataAdapter adap = new SqlDataAdapter(comando);
            adap.Fill(dt);


            keyF = dt.Rows[0].ItemArray[0].ToString();

            dt.Clear();
            conexion.Close();

        }
        public void Equipos()
        {
            DataTable dt = new DataTable();
            SqlConnection conexion = new SqlConnection(cadenaConexion);//cadena conexion

            conexion.Open();
            string consulta = "SELECT Equipo_Key,Nom_Equipo FROM Equipo"; //consulta a la tabla 
            SqlCommand comando = new SqlCommand(consulta, conexion);

            SqlDataAdapter adap = new SqlDataAdapter(comando);
            adap.Fill(dt);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                equiposListaNom.Add(dt.Rows[i].ItemArray[1].ToString());
                Debug.WriteLine(dt.Rows[i].ItemArray[1].ToString());
            }

            pck_equipos.ItemsSource = equiposListaNom;
            dt.Clear();
            conexion.Close();
        }
        private void FuentesEnergia()
        {

            DataTable dt = new DataTable();
            SqlConnection conexion = new SqlConnection(cadenaConexion);
            conexion.Open();
            string cmdtxt = "select FuenteEnergKey, FuenteEnergDesc from FuentesEnergia";
            SqlCommand cmd = new SqlCommand(cmdtxt, conexion);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            for(int i=0; i < dt.Rows.Count; i++)
            {
                fuentesEner.Add(dt.Rows[i].ItemArray[1].ToString());
            }
            pck_fuenteEnergetica.ItemsSource = fuentesEner;
            
            dt.Clear();
            conexion.Close();
        }
    }
}