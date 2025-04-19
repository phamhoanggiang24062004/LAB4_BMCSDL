using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class RoundedPanel : Panel
{
    public int CornerRadius { get; set; } = 15;          // Bán kính bo góc
    public Color BorderColor { get; set; } = Color.Gray; // Màu viền
    public int BorderThickness { get; set; } = 2;        // Độ dày viền

    public RoundedPanel()
    {
        // Tăng hiệu năng vẽ
        this.DoubleBuffered = true;
        // Khi thay đổi kích thước, tự invalidate để vẽ lại
        this.Resize += (s, e) => this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
        using (GraphicsPath path = GetRoundedRectPath(rect, CornerRadius))
        {
            // Set region để panel có hình bo góc
            this.Region = new Region(path);
            // Vẽ viền
            using (Pen pen = new Pen(BorderColor, BorderThickness))
                e.Graphics.DrawPath(pen, path);
        }
    }

    private GraphicsPath GetRoundedRectPath(Rectangle r, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int d = radius * 2;
        // Top-left
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        // Top-right
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        // Bottom-right
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        // Bottom-left
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
