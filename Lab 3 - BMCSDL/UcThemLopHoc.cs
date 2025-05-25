using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Lab_3___BMCSDL
{
    public partial class UcThemLopHoc : UserControl
    {
        string connectionString = @"Server=DESKTOP-P0BQAJD;Database=QLSVNhom;Trusted_Connection=True;";

        private Label lblMalop, lblTenlop, lblManhanvien;
        private TextBox txtMalop, txtTenlop, txtManhanvien;
        private Button btnSave;
        

        public UcThemLopHoc()
        {
            InitializeComponent();
            InitializeThemLopHocUI();
        }

        private void InitializeThemLopHocUI()
        {
            this.BackColor = Color.White;
            this.Size = new Size(600, 400); 

            int labelWidth = 100;
            int inputWidth = 250;
            int height = 30;
            int spacing = 30;
            int marginLeft = 30;
            int marginTop = 90;

            Font labelFont = new Font("Segoe UI", 10, FontStyle.Bold);
            Font inputFont = new Font("Segoe UI", 10, FontStyle.Regular);
            Font titleFont = new Font("Segoe UI", 14, FontStyle.Bold);

            // Tiêu đề
            Label lblTitle = new Label
            {
                Text = "THÔNG TIN LỚP HỌC",
                Location = new Point(0, 10),
                Size = new Size(this.Width, 40),
                Font = titleFont,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Mã lớp
            lblMalop = new Label
            {
                Text = "Mã lớp:",
                Location = new Point(marginLeft, marginTop),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtMalop = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Tên lớp
            lblTenlop = new Label
            {
                Text = "Tên lớp:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 1),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtTenlop = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 1),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Mã nhân viên
            lblManhanvien = new Label
            {
                Text = "Mã NV:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 2),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtManhanvien = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 2),
                Size = new Size(inputWidth, height),
                Font = inputFont,
                
            };


            // Nút Lưu
            btnSave = new Button
            {
                Text = "Lưu",
                Location = new Point(marginLeft + (labelWidth + inputWidth - 100) / 2, marginTop + (height + spacing) * 3),
                Size = new Size(100, 30),
                Font = inputFont
            };
            btnSave.Click += BtnSave_Click;

            // Thêm vào Control
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblMalop);
            this.Controls.Add(txtMalop);
            this.Controls.Add(lblTenlop);
            this.Controls.Add(txtTenlop);
            this.Controls.Add(lblManhanvien);
            this.Controls.Add(txtManhanvien);
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string malop = txtMalop.Text.Trim();
            string tenlop = txtTenlop.Text.Trim();
            string manv = txtManhanvien.Text.Trim();

            // Kiểm tra đầu vào
            if (string.IsNullOrEmpty(malop) || string.IsNullOrEmpty(tenlop) || string.IsNullOrEmpty(manv))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin lớp học!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_INS_LOPHOC", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MALOP", malop);
                        cmd.Parameters.AddWithValue("@TENLOP", tenlop);
                        cmd.Parameters.AddWithValue("@MANV", manv);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Thêm lớp học thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Xóa dữ liệu sau khi lưu
                txtMalop.Clear();
                txtTenlop.Clear();
                txtManhanvien.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm lớp học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}