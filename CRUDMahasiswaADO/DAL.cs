using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    internal class DAL
    {

        static string connectionString = "Data Source=LAPTOP-P0RT1FO1;Initial Catalog=DBAkademikADO; User ID=sa;Password=Kadirojo7";

        public string GetConnectionString()
        {
            string connection = $"Data Source={GetLocalIPAddress()};Initial Catalog=DBAkademikADO; User ID=sa;Password=Kadirojo7;";
            return connectionString;
        }

        SqlConnection conn = new SqlConnection(connectionString);

        SqlDataAdapter da;
        DataTable dtMahasiswa;
        DataTable dtProdi;

        public static string GetLocalIPAddress()
        {
            string localIP = string.Empty;
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting local IP address: " + ex.Message);
            }
            return localIP;
        }

        public int CountMhs()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(outputParam);

            cmd.ExecuteNonQuery();

            return Convert.ToInt32(outputParam.Value);
        }

        public DataTable GetMhs()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            da = new SqlDataAdapter(cmd);

            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);

            return dtMahasiswa;
        }

        public void InsertMhs(string nim, string nama, string alamat, string jenisKelamin, DateTime tanggalLahir, string kodeProdi, byte[] foto)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                SqlCommand command = new SqlCommand("sp_InsertMahasiswa", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = trans; // <-- WAJIB: command harus terdaftar ke transaksi yang sama

                command.Parameters.AddWithValue("@PNIM", nim);
                command.Parameters.AddWithValue("@PNama", nama);
                command.Parameters.AddWithValue("@PAlamat", alamat);
                command.Parameters.AddWithValue("@PTanggalLahir", tanggalLahir);
                command.Parameters.AddWithValue("@PJenisKelamin", jenisKelamin);
                command.Parameters.AddWithValue("@PKodeProdi", kodeProdi);
                SqlParameter paramFoto = command.Parameters.Add("@PFoto", SqlDbType.VarBinary, -1);
                paramFoto.Value = (object)foto ?? DBNull.Value;

                command.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw; // <-- lempar ulang agar Form1.cs tahu insert gagal dan TIDAK menampilkan pesan "berhasil"
            }
            finally
            {
                conn.Close();
            }
        }

        public void UpdateMhs(string nim, string nama, string alamat, string jenisKelamin, DateTime tanggalLahir, string kodeProdi, byte[] foto)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand command = new SqlCommand("sp_UpdateMahasiswa", conn);

            command.Parameters.AddWithValue("@PNIM", nim);
            command.Parameters.AddWithValue("@PNama", nama);
            command.Parameters.AddWithValue("@PAlamat", alamat);
            command.Parameters.AddWithValue("@PJenisKelamin", jenisKelamin);
            command.Parameters.AddWithValue("@PTanggalLahir", tanggalLahir);
            command.Parameters.AddWithValue("@PKodeProdi", kodeProdi);
            command.Parameters.AddWithValue("@Pfoto", foto);

            command.CommandType = CommandType.StoredProcedure;

            command.ExecuteNonQuery();
        }

        public void DeleteMhs(string nim)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn);
            cmd.Parameters.AddWithValue("@NIM", nim);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.ExecuteNonQuery();
        }
