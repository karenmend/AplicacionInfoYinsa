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
	public partial class Paros : ContentPage
	{
        List<string> equiposListaNom = new List<string>();
        List<string> causasParoLista = new List<string>();
        string keyE;
        string keyP;
        static string cadenaConexion = @"Data Source=192.168.1.73;Initial Catalog=INFOYINSA;Integrated Security = false;User ID = infoYinsa;Password = yinsa2;";
        public Paros ()
		{
			InitializeComponent ();
            Equipos();
            CausasParos();
            btn_agregar.Clicked += btn_agregarClicked;
        }

        private void CausasParos()
        {
            DataTable dt = new DataTable();
            SqlConnection conexion = new SqlConnection(cadenaConexion);//cadena conexion

            conexion.Open();
            string consulta = "SELECT CausaParoKey,CausaParoDesc FROM CausasParo"; //consulta a la tabla 
            SqlCommand comando = new SqlCommand(consulta, conexion);

            SqlDataAdapter adap = new SqlDataAdapter(comando);
            adap.Fill(dt);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                causasParoLista.Add(dt.Rows[i].ItemArray[1].ToString());
                Debug.WriteLine(dt.Rows[i].ItemArray[1].ToString());
            }

            pck_causaParo.ItemsSource = causasParoLista;
            dt.Clear();
            conexion.Close();
        }
        public void keyCausasParo()
        {
            DataTable dt = new DataTable();
            SqlConnection conexion = new SqlConnection(cadenaConexion);//cadena conexion

            conexion.Open();
            string consulta = "SELECT CausaParoKey FROM CausasParo WHERE CausaParoDesc = '" + pck_causaParo.SelectedItem.ToString() + "'"; //consulta a la tabla 
            SqlCommand comando = new SqlCommand(consulta, conexion);

            SqlDataAdapter adap = new SqlDataAdapter(comando);
            adap.Fill(dt);


            keyP = dt.Rows[0].ItemArray[0].ToString();

            dt.Clear();
            conexion.Close();

        }
        private void btn_agregarClicked(object sender, EventArgs e)
        {
           if(validacion())
            {
                keyEquipos();
                keyCausasParo();
                txtVacios();
                try
                {

                    SqlConnection   Conexion = new SqlConnection(cadenaConexion);//cadena conexion

                    string cmdtxt = "select * from TRABAJO_EQUIPO where Equipo_Key=" + keyE.ToString() + " and Fecha='" + datePck_fecha.Date.ToString("yyyyMMdd") + "'";
                    Conexion.Open();
                    SqlCommand cmd = new SqlCommand(cmdtxt, Conexion);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        Conexion.Close();
                        cmdtxt = "insert into Paros (EquipoKey, Fecha, CausaParoKey, HParo) values (" + keyE.ToString() + ", '" + datePck_fecha.Date.ToString("yyyyMMdd") + "', " + keyP.ToString() +
                            " , '" + txt_horasParo.Text + "')";
                        Conexion.Open();
                        cmd = new SqlCommand(cmdtxt, Conexion);
                        cmd.ExecuteReader();
                        Conexion.Close();

                        cmdtxt = "update TRABAJO_EQUIPO set H_Paro_Justif=(select isnull((select sum(HParo) from Paros a inner join CausasParo b on a.CausaParoKey=b.CausaParoKey where a.EquipoKey=" + keyE.ToString() + " and Fecha='" + datePck_fecha.Date.ToString("yyyyMMdd") + "' and b.ParoJustificado = 'True'),'0'))" +
                            " where Equipo_Key='" + keyE.ToString() + "' and Fecha='" + datePck_fecha.Date.ToString("yyyyMMdd") + "'";
                        Conexion.Open();
                        cmd = new SqlCommand(cmdtxt, Conexion);
                        cmd.ExecuteReader();
                        Conexion.Close();

                        cmdtxt = "update TRABAJO_EQUIPO set H_Paro_No_Justif=(select isnull((select sum(HParo) from Paros a inner join CausasParo b on a.CausaParoKey=b.CausaParoKey where a.EquipoKey=" + keyE.ToString() + " and Fecha='" + datePck_fecha.Date.ToString("yyyyMMdd") + "' and b.ParoJustificado = 'False'),'0'))" +
                            " where Equipo_Key='" + keyE.ToString() + "' and Fecha='" + datePck_fecha.Date.ToString("yyyyMMdd") + "'";
                        Conexion.Open();
                        cmd = new SqlCommand(cmdtxt, Conexion); cmd.ExecuteReader();
                        Conexion.Close();

                        /*CargaGridParo();
                        LimpiarParo();
                        CargaGridTEquipo();*/
                        
                        DisplayAlert("Atencion", "Carga Éxitosa", "OK");
                        Limpiar();
                        //MessageBox.Show("Carga Éxitosa", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Conexion.Close();
                        DisplayAlert("Ateción", "No se puede agregar Paro porque no hay registro en Trabajo de Equipo para la Fecha: " + datePck_fecha.Date.ToString("dd/MM/yyyy"), "OK");
                       
                    }
                }
                catch (Exception Ex)
                {
                    //debug
                    DisplayAlert("ERROR", Ex.ToString(), "OK");
                    throw;
                }
            }
        }

        private void txtVacios()
        {
            if(string.IsNullOrEmpty(txt_horasParo.Text))
            {
                txt_horasParo.Text = "0";
            }
        }

        bool validacion()
        {
            
            if (pck_equipos.SelectedItem == null)
            {
                DisplayAlert("Advertencia", "Necesitas seleccionar un equipo.", "OK");
                return false;
            }
            if(pck_causaParo.SelectedItem == null)
            {
                DisplayAlert("Advertencia", "Necesitas seleccionar una causa de paro.", "OK");
                return false;
            }
            if (!txt_horasParo.Text.ToCharArray().All(Char.IsDigit))
            {
                DisplayAlert("Advertencia", "El formato de Horas en Paro es incorrecto, solo se aceptan numeros.", "OK");
                return false;
            }
            return true;
        }
        public void Limpiar()
        {
            pck_equipos.SelectedIndex = -1;
            pck_causaParo.SelectedIndex = -1;
            txt_horasParo.Text = string.Empty;
           
        }
        private void Equipos()
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
    }
}