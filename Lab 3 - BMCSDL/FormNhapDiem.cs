using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
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
        private string connectionString = @"Server=DESKTOP-P0BQAJD;Database=QLSVNhom;Trusted_Connection=True;";
        private Button btnLuuDiem;
        private readonly EmployeeKeyGenerator _keyGenerator;

        public FormNhapDiem(string malop, string tenlop, string mahp, string manv, string password)
        {
            InitializeComponent();
            this.malop = malop;
            this.tenlop = tenlop;
            this.mahp = mahp;
            this.manv = manv;
            this.password = password;
            _keyGenerator = new EmployeeKeyGenerator();
            this.Text = $"Nhập điểm cho lớp: {malop} - {tenlop} - {mahp} của nhân viên {manv}";
            InitializeGrid();
            LoadSinhVienTheoLop();
        }

        private DataGridView dgv;

        private void InitializeGrid()
        {
            // --- Khởi tạo DataGridView --- 
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };

            // --- Nút Lưu điểm ---
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

        // --- Hiển thị điểm thi sinh viên theo lớp ---
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
                cmd.Parameters.AddWithValue("@MAHP", mahp);

                // Lấy dữ liệu ra DataTable
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Thêm cột mới để hiển thị điểm đã giải mã
                if (!dt.Columns.Contains("DIEM_GIAIMA"))
                    dt.Columns.Add("DIEM_GIAIMA", typeof(int));

                // Giải mã điểm và gán vào cột DIEM_GIAIMA
                foreach (DataRow row in dt.Rows)
                {
                    if (row["DIEMTHI"] != DBNull.Value)
                    {
                        try
                        {
                            byte[] encryptedDiem = (byte[])row["DIEMTHI"];
                            using (RSA rsa = _keyGenerator.LoadPrivateKey(manv, password))
                            {
                                byte[] decryptedDiem = rsa.Decrypt(encryptedDiem, RSAEncryptionPadding.Pkcs1);
                                int diem = BitConverter.ToInt32(decryptedDiem, 0);
                                row["DIEM_GIAIMA"] = diem;
                            }
                        }
                        catch
                        {
                            // Giải mã lỗi, để null
                            row["DIEM_GIAIMA"] = DBNull.Value;
                        }
                    }
                    else
                    {
                        row["DIEM_GIAIMA"] = DBNull.Value;
                    }
                }

                // Xoá cột byte[] để tránh lỗi DataGridView khi hiển thị
                dt.Columns.Remove("DIEMTHI");

                // Thêm cột điểm mới nhập tay
                if (!dt.Columns.Contains("DIEM_MOI"))
                    dt.Columns.Add("DIEM_MOI", typeof(double));

                // Gán dữ liệu vào DataGridView
                dgv.DataSource = dt;

                // Chỉ cho phép nhập điểm mới
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    col.ReadOnly = col.Name != "DIEM_MOI";
                }

                // Đổi tên tiêu đề cột
                dgv.Columns["MASV"].HeaderText = "Mã SV";
                dgv.Columns["HOTEN"].HeaderText = "Họ tên";
                dgv.Columns["DIEM_GIAIMA"].HeaderText = "Điểm Thi";
                dgv.Columns["DIEM_MOI"].HeaderText = "Điểm Mới";
            }
        }



        // --- Lưu điểm ---
        private void BtnLuuDiem_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Duyệt qua từng dòng trong DataGridView
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    string masv = row.Cells["MASV"].Value.ToString();
                    object diemObj = row.Cells["DIEM_MOI"].Value;

                    if (diemObj == DBNull.Value || diemObj == null) continue;

                    int diem;
                    int.TryParse(diemObj.ToString(), out diem);


                    // Mã hoá điểm thi
                    byte[] encryptedDiem = null;
                    using (RSA rsa = _keyGenerator.LoadPublicKey(manv))
                    {
                        byte[] diemBytes = BitConverter.GetBytes(diem);
                        encryptedDiem = rsa.Encrypt(diemBytes, RSAEncryptionPadding.Pkcs1);
                    }


                    // Gọi Stored Procedure SP_NHAPDIEM_SINHVIEN
                    using (SqlCommand cmd = new SqlCommand("SP_NHAPDIEM_SINHVIEN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ENC_DIEM", encryptedDiem);
                        cmd.Parameters.AddWithValue("@MASV", masv);
                        cmd.Parameters.AddWithValue("@MAHP", mahp);

                        cmd.ExecuteNonQuery();                  // Thực thi stored procedure
                    }
                }

                MessageBox.Show("Lưu điểm thành công!", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}
