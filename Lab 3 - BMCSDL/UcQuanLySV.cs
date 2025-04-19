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
    public partial class UcQuanLySV : UserControl
    {
        string connectionString = @"Server=LAPTOP-RBM16H2U\MSSQLSER2022;Database=QLSVNhom;Trusted_Connection=True;";
        private string currentMANV = "NV01";  // lấy từ đăng nhập thực tế

        private FlowLayoutPanel flowPanel;

        public UcQuanLySV()
        {
            InitializeComponent();
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
                WrapContents = false
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

                    // Lấy tất cả lớp do nhân viên quản lý
                    string queryLop = "SELECT MALOP, TENLOP FROM LOP WHERE MANV = @MANV";
                    SqlCommand cmdLop = new SqlCommand(queryLop, conn);
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
                            Width = 900,
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
                            Height = 220,
                            Visible = false // Ban đầu ẩn
                        };

                        // Tạo DataGridView và add vào GroupBox
                        DataGridView dgv = new DataGridView
                        {
                            Dock = DockStyle.Fill,
                            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                            AllowUserToAddRows = false,
                            AllowUserToDeleteRows = false,
                            ReadOnly = false
                        };

                        panelContent.Controls.Add(dgv);
                        groupBox.Controls.Add(panelContent);
                        flowPanel.Controls.Add(groupBox);

                        // Sự kiện click để ẩn/hiện DataGridView
                        toggleButton.Click += (s, e) =>
                        {
                            panelContent.Visible = !panelContent.Visible;
                            toggleButton.Text = panelContent.Visible ? "▲" : "▼";
                            groupBox.Height = panelContent.Visible ? 300 : 60;
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
            string querySV = @"SELECT MASV, HOTEN, NGAYSINH, DIACHI FROM SINHVIEN WHERE MALOP = @MALOP";
            SqlCommand cmd = new SqlCommand(querySV, conn);
            cmd.Parameters.AddWithValue("@MALOP", malop);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dgv.DataSource = dt;

            // Cấu hình cho phép sửa các cột phù hợp
            dgv.Columns["MASV"].ReadOnly = true; // không cho sửa mã sinh viên
        }
    }
}
