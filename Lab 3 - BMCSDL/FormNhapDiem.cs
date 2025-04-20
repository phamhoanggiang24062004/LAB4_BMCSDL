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
        private string connectionString = @"Server=LAPTOP-RBM16H2U\MSSQLSER2022;Database=QLSVNhom;Trusted_Connection=True;";
        private Button btnLuuDiem;

        public FormNhapDiem(string malop, string tenlop, string mahp)
        {
            InitializeComponent();
            this.malop = malop;
            this.tenlop = tenlop;
            this.mahp = mahp;

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

        private void BtnLuuDiem_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    string masv = row.Cells["MASV"].Value.ToString();
                    object diemObj = row.Cells["DIEM"].Value;

                    if (diemObj == DBNull.Value || diemObj == null) continue;

                    double diem;
                    if (!double.TryParse(diemObj.ToString(), out diem)) continue;

                    // Kiểm tra đã có điểm chưa
                    string checkQuery = "SELECT COUNT(*) FROM BANGDIEM WHERE MASV = @MASV AND MAHP = @MAHP";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@MASV", masv);
                    checkCmd.Parameters.AddWithValue("@MAHP", mahp);
                    int count = (int)checkCmd.ExecuteScalar();

                    string query;
                    if (count > 0)
                    {
                        // Cập nhật điểm
                        query = "UPDATE BANGDIEM SET DIEMTHI = @DIEMTHI WHERE MASV = @MASV AND MAHP = @MAHP";
                    }
                    else
                    {
                        // Chèn điểm mới
                        query = "INSERT INTO BANGDIEM (MASV, MAHP, DIEMTHI) VALUES (@MASV, @MAHP, @DIEMTHI)";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MASV", masv);
                    cmd.Parameters.AddWithValue("@MAHP", mahp);
                    cmd.Parameters.AddWithValue("@DIEMTHI", diem);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Lưu điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}
