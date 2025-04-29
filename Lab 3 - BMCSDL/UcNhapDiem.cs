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
    public partial class UcNhapDiem : UserControl
    {
        string connectionString = @"Server=LAPTOP-RBM16H2U\MSSQLSER2022;Database=QLSVNhom;Trusted_Connection=True;";
        private string currentMANV;
        private string password;
        private DataGridView dgvLop;

        public UcNhapDiem(string manv, string pass)
        {
            InitializeComponent();
            currentMANV = manv;
            password = pass;
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
                Padding = new Padding(0, 0, 50, 0),
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
                ScrollBars = ScrollBars.Vertical    // Cho cuộn dọc nếu dữ liệu dài
            };

            dgvLop.CellDoubleClick += DgvLop_CellDoubleClick;

            containerPanel.Controls.Add(dgvLop);
            this.Controls.Add(containerPanel);
        }

        // --- Hiển thị danh sách lớp học ---
        private void LoadLopData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Gọi Stored Procedure thay vì tự viết query
                    SqlCommand cmdLop = new SqlCommand("SP_QUANLY_LOPHOC_NHANVIEN", conn);
                    cmdLop.CommandType = CommandType.StoredProcedure;

                    cmdLop.Parameters.AddWithValue("@MANV", currentMANV);

                    SqlDataAdapter adapterLop = new SqlDataAdapter(cmdLop);
                    DataTable table = new DataTable();
                    adapterLop.Fill(table);

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

        // --- Tính chiều cao DataGridView ---
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

        // --- Double click vào dòng lớp để nhập điểm ---
        private void DgvLop_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvLop.Rows[e.RowIndex];
                string malop = row.Cells["MALOP"].Value.ToString();
                string tenlop = row.Cells["TENLOP"].Value.ToString();

                string mahp = GetMAHP_From_TENLOP(tenlop);

                if (string.IsNullOrEmpty(mahp))
                {
                    MessageBox.Show("Không tìm thấy học phần phù hợp với lớp.", "Lỗi", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                FormNhapDiem frm = new FormNhapDiem(malop, tenlop, mahp, currentMANV, password);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();               // dùng ShowDialog để buộc người dùng nhập xong mới quay về
            }
        }

        // --- Lấy mã học phần từ tên lớp ---
        private string GetMAHP_From_TENLOP(string tenlop)
        {
            string mahp = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Gọi stored procedure thay vì viết trực tiếp câu lệnh SQL
                using (SqlCommand cmd = new SqlCommand("SP_TIM_HOCPHAN_THEOTEN", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Thêm tham số đầu vào cho stored procedure
                    cmd.Parameters.AddWithValue("@TenLop", tenlop);

                    // Thực hiện truy vấn, sử dụng ExecuteReader để lấy nhiều cột
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Cột MAHP nằm ở vị trí 0 (theo định nghĩa trong procedure)
                            mahp = reader["MAHP"].ToString();
                        }
                    }
                }
            }

            return mahp;
        }

    }
}
