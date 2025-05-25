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
    public partial class UcThemNhanVien : UserControl
    {
        string connectionString = @"Server=DESKTOP-P0BQAJD;Database=QLSVNhom;Trusted_Connection=True;";

        private Label lblManv, lblHoten, lblEmail, lblLuong, lblTendn, lblMatkhau;
        private TextBox txtManv, txtHoten, txtEmail, txtLuong, txtTendn, txtMatkhau;
        private Button btnSave;
        private readonly EmployeeKeyGenerator _keyGenerator;

        public UcThemNhanVien()
        {
            InitializeComponent();
            _keyGenerator = new EmployeeKeyGenerator();
            InitializeThemNhanVienUI();
        }

        private void InitializeThemNhanVienUI()
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
                Text = "THÔNG TIN NHÂN VIÊN",
                Location = new Point(0, 10),
                Size = new Size(this.Width, 40),
                Font = titleFont,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Mã NV
            lblManv = new Label
            {
                Text = "Mã NV:",
                Location = new Point(marginLeft, marginTop),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtManv = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Họ tên
            lblHoten = new Label
            {
                Text = "Họ tên:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 1),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtHoten = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 1),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Email
            lblEmail = new Label
            {
                Text = "Email:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 2),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtEmail = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 2),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Lương
            lblLuong = new Label
            {
                Text = "Lương:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 3),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtLuong = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 3),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Tên đăng nhập
            lblTendn = new Label
            {
                Text = "Tên ĐN:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 4),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtTendn = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 4),
                Size = new Size(inputWidth, height),
                Font = inputFont
            };

            // Mật khẩu
            lblMatkhau = new Label
            {
                Text = "Mật khẩu:",
                Location = new Point(marginLeft, marginTop + (height + spacing) * 5),
                Size = new Size(labelWidth, height),
                Font = labelFont
            };
            txtMatkhau = new TextBox
            {
                Location = new Point(marginLeft + labelWidth, marginTop + (height + spacing) * 5),
                Size = new Size(inputWidth, height),
                Font = inputFont
               
            };

         

            // Nút Lưu
            btnSave = new Button
            {
                Text = "Lưu",
                Location = new Point(marginLeft + (labelWidth + inputWidth - 100) / 2, marginTop + (height + spacing) * 6),
                Size = new Size(100, 30),
                Font = inputFont
            };
            btnSave.Click += BtnSave_Click;

            // Thêm vào Control
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblManv);
            this.Controls.Add(txtManv);
            this.Controls.Add(lblTendn);
            this.Controls.Add(txtTendn);
            this.Controls.Add(lblMatkhau);
            this.Controls.Add(txtMatkhau);
            this.Controls.Add(lblHoten);
            this.Controls.Add(txtHoten);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblLuong);
            this.Controls.Add(txtLuong);
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ TextBox
            string manv = txtManv.Text.Trim();
            string hoten = txtHoten.Text.Trim();
            string email = txtEmail.Text.Trim();
            string luongText = txtLuong.Text.Trim();
            string tendn = txtTendn.Text.Trim();
            string matkhau = txtMatkhau.Text.Trim();

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(manv) || string.IsNullOrEmpty(hoten) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(luongText) || string.IsNullOrEmpty(tendn) || string.IsNullOrEmpty(matkhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra định dạng email
            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra lương có phải là số không
            if (!int.TryParse(luongText, out int luong))
            {
                MessageBox.Show("Lương phải là một số hợp lệ!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Đường dẫn lưu trữ khóa
            string publicKeyPath = Path.Combine(_keyGenerator.getKeyStoragePath(), $"{manv}_publickey.xml");
            string privateKeyPath = Path.Combine(_keyGenerator.getKeyStoragePath(), $"{manv}_privatekey.xml");

            if (!File.Exists(publicKeyPath) || !File.Exists(privateKeyPath))
            {
                // Tạo cặp khóa nếu chưa tồn tại
                _keyGenerator.CreateKeyPairForEmployee(manv, matkhau);
            }


            // Mã hóa lương và lấy public key
            byte[] encryptedLuong = null;
            string publicKeyXml = "";

            try
            {
                using (RSA rsa = _keyGenerator.LoadPublicKey(manv))
                {
                    byte[] luongBytes = BitConverter.GetBytes(luong);
                    encryptedLuong = rsa.Encrypt(luongBytes, RSAEncryptionPadding.Pkcs1);
                    publicKeyXml = rsa.ToXmlString(false); // Lấy public key dưới dạng XML

                    // Lưu dữ liệu vào database ngay trong khối using
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SP_INS_PUBLIC_NHANVIEN", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@MANV", manv);
                            cmd.Parameters.AddWithValue("@HOTEN", hoten);
                            cmd.Parameters.AddWithValue("@EMAIL", email);
                            cmd.Parameters.AddWithValue("@LUONG", encryptedLuong ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@TENDN", tendn);

                            // Mã hóa mật khẩu
                            byte[] hashedPassword = null;
                            using (SHA1 sha1 = SHA1.Create())
                            {
                                byte[] passwordBytes = Encoding.UTF8.GetBytes(matkhau);
                                hashedPassword = sha1.ComputeHash(passwordBytes);
                            }
                            cmd.Parameters.AddWithValue("@MK", hashedPassword ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@PUB", publicKeyXml);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Thêm nhân viên thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Xóa dữ liệu trong TextBox sau khi lưu
                    txtManv.Clear();
                    txtHoten.Clear();
                    txtEmail.Clear();
                    txtLuong.Clear();
                    txtTendn.Clear();
                    txtMatkhau.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm nhân viên: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}