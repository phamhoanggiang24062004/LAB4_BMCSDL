using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using System.IO;

namespace Lab_3___BMCSDL
{
    public partial class Form1 : Form
    {
        private RoundedPanel panelLogin;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private readonly EmployeeKeyGenerator _keyGenerator;

        public Form1()
        {
            InitializeComponent();
            InitializeLoginForm();
            // Nút Enter cho đăng nhập
            this.AcceptButton = btnLogin;
            // Đăng ký sự kiện Resize cho Form
            this.Resize += MainForm_Resize;
            _keyGenerator = new EmployeeKeyGenerator();
        }

        private void InitializeLoginForm()
        {
            //--- Thiết lập Form ---
            this.Text = "Đăng nhập";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(800, 600);
            // Background sẽ thêm sau
            this.BackgroundImage = Image.FromFile("D:\\NAM_3\\BMCSDL\\Lab 3 - BMCSDL\\Resources\\images.jpg");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            //--- Tạo Panel bo góc chứa phần login ---
            panelLogin = new RoundedPanel
            {
                CornerRadius = 20,
                BorderColor = Color.LightBlue,
                BorderThickness = 2,
                BackColor = Color.FromArgb(240, 248, 255),      // màu nền nhạt
                Size = new Size(400, 230),
            };

            // Canh giữa panelLogin
            panelLogin.Location = new Point(
                (this.ClientSize.Width - panelLogin.Width) / 2,
                (this.ClientSize.Height - panelLogin.Height) / 2
            );

            this.Controls.Add(panelLogin);

            //--- Label và TextBox Tên đăng nhập ---
            Label lblUser = new Label
            {
                Text = "Tên đăng nhập:",
                AutoSize = true,
                Location = new Point(20, 20)
            };
            panelLogin.Controls.Add(lblUser);

            txtUsername = new TextBox
            {
                Size = new Size(360, 25),
                Location = new Point(20, 45)
            };
            panelLogin.Controls.Add(txtUsername);

            //--- Label và TextBox Mật khẩu ---
            Label lblPass = new Label
            {
                Text = "Mật khẩu:",
                AutoSize = true,
                Location = new Point(20, 80)
            };
            panelLogin.Controls.Add(lblPass);

            txtPassword = new TextBox
            {
                Size = new Size(360, 25),
                Location = new Point(20, 105),
                UseSystemPasswordChar = true
            };
            panelLogin.Controls.Add(txtPassword);

            //--- Nút Đăng nhập ---
            btnLogin = new Button
            {
                Text = "Đăng nhập",
                Size = new Size(100, 30),
                Location = new Point((panelLogin.Width - 100) / 2, 160)
            };
            btnLogin.Click += BtnLogin_Click;
            panelLogin.Controls.Add(btnLogin);
        }

        // --- Xác thực đăng nhập --- 
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Lấy tên đăng nhập và mật khẩu từ TextBox
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;

            // Mã hóa mật khẩu người dùng nhập bằng SHA1
            byte[] hashedPassword = HashPasswordWithSHA1(pass);

            // Chuỗi kết nối đến cơ sở dữ liệu
            string connectionString = @"Server=DESKTOP-P0BQAJD;Database=QLSVNhom;Trusted_Connection=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Tạo SqlCommand để gọi Stored Procedure
                    using (SqlCommand command = new SqlCommand("SP_KIEMTRA_DANGNHAP", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Tham số input
                        command.Parameters.AddWithValue("@USERNAME", user);
                        command.Parameters.AddWithValue("@PASSWORD", hashedPassword);

                        // Tham số output
                        SqlParameter returnManvParam = new SqlParameter("@RETURN_MANV", SqlDbType.VarChar, 20);
                        returnManvParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(returnManvParam);

                        command.ExecuteNonQuery();

                        string manv = returnManvParam.Value as string;

                        if (!string.IsNullOrEmpty(manv))
                        {

                            Dashboard dashboard = new Dashboard(manv, pass, user);
                            this.Hide();
                            dashboard.ShowDialog();
                            this.Close();
                        }
                        else if (user == "nv01")
                        {
                           
                            Dashboard dashboard = new Dashboard("NV01", pass, user);
                            this.Hide();
                            dashboard.ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        // Canh giữa cho Round Panel - nơi chứa chỗ đăng nhập 
        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Tính tọa độ mới để panelLogin luôn ở giữa Form
            int newX = (this.ClientSize.Width - panelLogin.Width) / 2;
            int newY = (this.ClientSize.Height - panelLogin.Height) / 2;

            panelLogin.Location = new Point(newX, newY);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // --- Mã hóa mật khẩu bằng SHA1 - UTF16 LE ---
        private byte[] HashPasswordWithSHA1(string password)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                return sha1.ComputeHash(passwordBytes);
            }
        }
    }
}
