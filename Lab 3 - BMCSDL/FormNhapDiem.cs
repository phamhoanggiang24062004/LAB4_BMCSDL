using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_3___BMCSDL
{
    public partial class FormNhapDiem : Form
    {
        private string malop;
        private string tenlop;
        private string connectionString = @"Server=LAPTOP-RBM16H2U\MSSQLSER2022;Database=QLSVNhom;Trusted_Connection=True;";

        public FormNhapDiem(string malop, string tenlop)
        {
            InitializeComponent();
            this.malop = malop;
            this.tenlop = tenlop;

            this.Text = $"Nhập điểm cho lớp: {tenlop} ({malop})";
            InitializeGrid();
            LoadSinhVienTheoLop();
        }

        private DataGridView dgv;

        private void InitializeGrid()
        {
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };
            this.Controls.Add(dgv);
        }

        private void LoadSinhVienTheoLop()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT MASV, HOTEN FROM SINHVIEN WHERE MALOP = @MALOP";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MALOP", malop);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (!dt.Columns.Contains("DIEM"))
                    dt.Columns.Add("DIEM", typeof(double)); // Thêm cột điểm nhập tay

                dgv.DataSource = dt;

                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    col.ReadOnly = col.Name != "DIEM"; // Chỉ cột điểm được nhập
                }

                dgv.Columns["MASV"].HeaderText = "Mã SV";
                dgv.Columns["HOTEN"].HeaderText = "Họ tên";
                dgv.Columns["DIEM"].HeaderText = "Điểm";
            }
        }
    }
}
