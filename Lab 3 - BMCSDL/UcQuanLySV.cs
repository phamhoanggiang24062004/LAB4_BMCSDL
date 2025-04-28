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
using System.Security.Cryptography;


namespace Lab_3___BMCSDL
{
    public partial class UcQuanLySV : UserControl
    {
        string connectionString = @"Server=LAPTOP-RBM16H2U\MSSQLSER2022;Database=QLSVNhom;Trusted_Connection=True;";
        private string currentMANV;

        private FlowLayoutPanel flowPanel;

        public UcQuanLySV(string manv)
        {
            InitializeComponent();
            currentMANV = manv;
            InitializeUI();
            LoadSinhVienTheoLop();
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
            };

            this.Controls.Add(flowPanel);
        }

        private void LoadSinhVienTheoLop()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Lấy tất cả lớp do nhân viên quản lý bằng Stored Procedure
                    SqlCommand cmdLop = new SqlCommand("SP_QUANLY_LOPHOC_NHANVIEN", conn);
                    cmdLop.CommandType = CommandType.StoredProcedure;

                    // Thêm tham số
                    cmdLop.Parameters.AddWithValue("@MANV", currentMANV);


                    SqlDataAdapter adapterLop = new SqlDataAdapter(cmdLop);
                    DataTable dtLop = new DataTable();
                    adapterLop.Fill(dtLop);

                    foreach (DataRow row in dtLop.Rows)
                    {
                        string malop = row["MALOP"].ToString();
                        string tenlop = row["TENLOP"].ToString();

                        // Tạo GroupBox cho mỗi lớp
                        GroupBox groupBox = new GroupBox
                        {
                            Text = $"Lớp: {tenlop} ({malop})",
                            Width = 1170,
                            Height = 300,
                            Font = new Font("Segoe UI", 10, FontStyle.Bold),
                            AutoSize = true,
                            AutoSizeMode = AutoSizeMode.GrowAndShrink
                        };

                        // Button toggle
                        Button toggleButton = new Button
                        {
                            Text = "▼", // Hình tam giác chỉ xuống
                            Width = 30,
                            Height = 25,
                            Location = new Point(groupBox.Width - 40, 20),
                            Anchor = AnchorStyles.Top | AnchorStyles.Right,
                            FlatStyle = FlatStyle.Flat
                        };
                        groupBox.Controls.Add(toggleButton);

                        // Panel để chứa DataGridView
                        Panel panelContent = new Panel
                        {
                            Dock = DockStyle.Bottom,
                            Visible = false // Ban đầu ẩn
                        };

                        // Tạo DataGridView và add vào GroupBox
                        DataGridView dgv = new DataGridView
                        {
                            Dock = DockStyle.Top,
                            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                            AllowUserToAddRows = false,
                            AllowUserToDeleteRows = false,
                            ReadOnly = false
                        };

                        Button btnCapNhat = new Button
                        {
                            Text = "Cập nhật thông tin",
                            Width = 150,
                            Height = 35,
                            BackColor = Color.LightBlue,
                            Font = new Font("Segoe UI", 9),
                            Visible = false // Ẩn ban đầu
                        };
                        btnCapNhat.Location = new Point(20, 10); // vị trí tạm, sẽ chỉnh lại bên dưới

                        // Gắn sự kiện gọi hàm cập nhật
                        btnCapNhat.Click += (s, e) => CapNhatDuLieuSinhVien(malop, dgv);

                        panelContent.Controls.Add(dgv);
                        panelContent.Controls.Add(btnCapNhat);
                        groupBox.Controls.Add(panelContent);
                        flowPanel.Controls.Add(groupBox);

                        // Sự kiện click để ẩn/hiện DataGridView
                        toggleButton.Click += (s, e) =>
                        {
                            panelContent.Visible = !panelContent.Visible;
                            toggleButton.Text = panelContent.Visible ? "▲" : "▼";
                            btnCapNhat.Visible = panelContent.Visible;

                            if (panelContent.Visible)
                            {
                                // Tính toán chiều cao vừa đủ cho nội dung
                                int rowHeight = dgv.RowTemplate.Height; // chiều cao trung bình mỗi dòng
                                int rowCount = dgv.Rows.Count;

                                // Tính tổng chiều cao thực tế: dòng + header + padding
                                int dgvHeight = (rowHeight * rowCount) + dgv.ColumnHeadersHeight + 10;

                                // Nếu có scrollbar, cộng thêm một chút
                                if (dgv.DisplayedRowCount(false) < dgv.RowCount)
                                {
                                    dgvHeight += SystemInformation.HorizontalScrollBarHeight;
                                }

                                // Cập nhật chiều cao cho panelContent và groupBox
                                dgv.Height = dgvHeight;
                                btnCapNhat.Top = dgv.Bottom + 10; // đặt nút dưới bảng
                                panelContent.Height = dgvHeight + btnCapNhat.Height + 20;

                                groupBox.Height = panelContent.Height + 60; // 60 là phần header + margin GroupBox
                            }
                            else
                            {
                                groupBox.Height = 60;
                            }
                        };

                        // Load sinh viên của lớp này
                        LoadSinhVienForLop(malop, dgv, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu sinh viên: " + ex.Message);
            }
        }

        private void LoadSinhVienForLop(string malop, DataGridView dgv, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("SP_QUANLY_SINHVIEN", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MALOP", malop);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dt.Columns.Add("NEW_PASSWORD", typeof(string)); // thêm cột giả cho password được mã hóa 
            dgv.DataSource = dt;

            if (dgv.Columns.Contains("MATKHAU"))
            {
                dgv.Columns["MATKHAU"].Visible = false; // Ẩn cột mật khẩu gốc
            }

            if (dgv.Columns.Contains("MASV"))
            {
                dgv.Columns["MASV"].ReadOnly = true; // không cho sửa Mã sinh viên
            }
        }

        public static byte[] HashPassword(string password)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);
                return hashBytes;
            }
        }

        private void CapNhatDuLieuSinhVien(string malop, DataGridView dgv)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    string masv = row.Cells["MASV"].Value?.ToString();
                    string hoten = row.Cells["HOTEN"].Value?.ToString();
                    string diachi = row.Cells["DIACHI"].Value?.ToString();
                    string tendangnhap = row.Cells["TENDN"].Value?.ToString();

                    // Lấy ngày sinh
                    object ngaysinhObj = row.Cells["NGAYSINH"].Value;
                    DateTime? ngaysinh = null;
                    if (ngaysinhObj != null && DateTime.TryParse(ngaysinhObj.ToString(), out DateTime parsed))
                    {
                        ngaysinh = parsed;
                    }

                    // Lấy mật khẩu mới nếu có
                    string newPassword = row.Cells["NEW_PASSWORD"].Value?.ToString();
                    byte[] finalPassword = (byte[])row.Cells["MATKHAU"].Value;

                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        finalPassword = HashPassword(newPassword);
                    }

                    try
                    {
                        // Kiểm tra các trường bắt buộc
                        if (string.IsNullOrWhiteSpace(masv) || string.IsNullOrWhiteSpace(hoten) ||
                            string.IsNullOrWhiteSpace(tendangnhap) || finalPassword == null)
                        {
                            MessageBox.Show($"Thiếu thông tin [MASV] [TENDN] [HOTEN] bắt buộc ở sinh viên có mã: {masv}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }

                        using (SqlCommand cmd = new SqlCommand("SP_CAPNHAT_SINHVIEN", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Thêm tham số
                            cmd.Parameters.AddWithValue("@MASV", masv);
                            cmd.Parameters.AddWithValue("@HOTEN", hoten);
                            cmd.Parameters.AddWithValue("@NGAYSINH", ngaysinh ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@DIACHI", diachi ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@MALOP", malop ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@TENDN", tendangnhap);
                            cmd.Parameters.AddWithValue("@MATKHAU", finalPassword ?? (object)DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }

                        // Xóa dữ liệu cột NEW_PASSWORD để tránh cập nhật lại
                        row.Cells["NEW_PASSWORD"].Value = null;

                        MessageBox.Show($"Cập nhật sinh viên {masv} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Lỗi SQL khi cập nhật sinh viên {masv}: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
        }
    }
}
