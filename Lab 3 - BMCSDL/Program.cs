namespace Lab_3___BMCSDL
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());


            // 1. Hiển thị Form1 (LoginForm) ở chế độ modal
            //using (var login = new Form1())
            //{
            //    // ShowDialog trả về DialogResult tùy bạn set trong Form1
            //    if (login.ShowDialog() == DialogResult.OK)
            //    {
            //        // 2. Nếu login thành công (DialogResult.OK) → khởi chạy MainForm
            //        Application.Run(new Dashboard());
            //    }
            //    // Ngược lại: thoát luôn, không chạy MainForm
            //}
        }
    }
}