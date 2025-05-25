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
        string connectionString = @"Server=DESKTOP-P0BQAJD;Database=QLSVNhom;Trusted_Connection=True;";

        public UcQuanLyLop()
        {
            InitializeComponent();
            InitializeGrid();
            LoadLopData();
        }

        private void InitializeGrid()
        {
            this.Dock = DockStyle.Fill;                 // Phủ hết phần content 
            this.BackColor = Color.White;

            Panel containerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 50, 0),     // Tạo khoảng cách phải
                BackColor = Color.White
            };

            dgvLop = new DataGridView
            {
                Dock = DockStyle.Top,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ScrollBars = ScrollBars.Vertical        // Cho cuộn dọc nếu dữ liệu dài
            };

            containerPanel.Controls.Add(dgvLop);
            this.Controls.Add(containerPanel);
        }

        // --- Tải dữ liệu lớp học từ cơ sở dữ liệu ---
        private void LoadLopData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_QUANLY_LOPHOC", conn);
                    cmd.CommandType = CommandType.StoredProcedure; 

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    dgvLop.DataSource = table;

                    // Sau khi load xong dữ liệu, tính và cập nhật chiều cao
                    int newHeight = TinhChieuCaoDataGridView(dgvLop);
                    // Giới hạn chiều cao tối đa, 900 - headerPanel.Height - verticalGap
                    int maxHeight = 900 - 60 - 30;
                    dgvLop.Height = Math.Min(newHeight, maxHeight);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu lớp học: " + ex.Message, "Lỗi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int TinhChieuCaoDataGridView(DataGridView dgv)
        {
            int totalHeight = dgv.ColumnHeadersHeight;  // Chiều cao phần header

            foreach (DataGridViewRow row in dgv.Rows)
            {
                totalHeight += row.Height;
            }
            totalHeight += 10;                          // padding nhỏ bên dưới

            return totalHeight;
        }
    }
}
