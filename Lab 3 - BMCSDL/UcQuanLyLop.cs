using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Lab_3___BMCSDL
{
    public partial class UcQuanLyLop : UserControl
    {
        private DataGridView dgvLop;
        string connectionString = @"Server=LAPTOP-RBM16H2U\MSSQLSER2022;Database=QLSVNhom;Trusted_Connection=True;";

        public UcQuanLyLop()
        {
            InitializeComponent();
            InitializeGrid();
            LoadLopData();
        }

        private void InitializeGrid()
        {
            this.Dock = DockStyle.Fill;             // Phủ hết phần content 
            this.BackColor = Color.White;

            Panel containerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 50, 0), // Tạo khoảng cách phải
                BackColor = Color.White
            };

            dgvLop = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            containerPanel.Controls.Add(dgvLop);
            this.Controls.Add(containerPanel);
        }

        private void LoadLopData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT MALOP, TENLOP, MANV FROM LOP";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    dgvLop.DataSource = table;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu lớp học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
