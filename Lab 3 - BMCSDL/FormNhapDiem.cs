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
        private string mahp;
        private string manv;
        private string password;
        private string connectionString = @"Server=LAPTOP-RBM16H2U\MSSQLSER2022;Database=QLSVNhom;Trusted_Connection=True;";
        private Button btnLuuDiem;

        public FormNhapDiem(string malop, string tenlop, string mahp, string manv, string password)
        {
            InitializeComponent();
            this.malop = malop;
            this.tenlop = tenlop;
            this.mahp = mahp;
            this.manv = manv;
            this.password = password;

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

            btnLuuDiem = new Button
            {
                Text = "Lưu điểm",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.LightGreen
            };
            btnLuuDiem.Click += BtnLuuDiem_Click;

            this.Controls.Add(btnLuuDiem);
            this.Controls.Add(dgv);
        }

        private void LoadSinhVienTheoLop()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Tạo SqlCommand để gọi Stored Procedure
                SqlCommand cmd = new SqlCommand("SP_HIENTHI_DIEMTHI_SINHVIEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Thêm tham số cho Stored Procedure
                cmd.Parameters.AddWithValue("@MALOP", malop);
                cmd.Parameters.AddWithValue("@MANV", manv);

                // Lấy dữ liệu ra DataTable
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (!dt.Columns.Contains("DIEM_MOI"))
                    dt.Columns.Add("DIEM_MOI", typeof(double)); // Thêm cột điểm nhập tay

                dgv.DataSource = dt;

                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    col.ReadOnly = col.Name != "DIEM_MOI"; // Chỉ cột điểm được nhập
                }

                dgv.Columns["MASV"].HeaderText = "Mã SV";
                dgv.Columns["HOTEN"].HeaderText = "Họ tên";
                dgv.Columns["DIEM_MOI"].HeaderText = "Điểm Mới";
            }
        }

        private void BtnLuuDiem_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    string masv = row.Cells["MASV"].Value.ToString();
                    object diemObj = row.Cells["DIEM_MOI"].Value;

                    if (diemObj == DBNull.Value || diemObj == null) continue;

                    int diem;
                    int.TryParse(diemObj.ToString(), out diem);

                    // Gọi Stored Procedure SP_NHAPDIEM_SINHVIEN
                    SqlCommand cmd = new SqlCommand("SP_NHAPDIEM_SINHVIEN", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DIEM", diem); // ép về int vì @DIEM là int
                    cmd.Parameters.AddWithValue("@MASV", masv);
                    cmd.Parameters.AddWithValue("@MAHP", mahp);
                    cmd.Parameters.AddWithValue("@MANV", manv);
                }

                MessageBox.Show("Lưu điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}
