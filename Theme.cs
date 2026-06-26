using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DQ5SaveEditor;

/// <summary>
/// Visual theme modelled on the Dragon Quest V DS "command window": a deep navy
/// blue fill with a bright rounded light-blue frame, white text, and gold accents
/// for the active cursor. Centralises the palette and the window/border painting so
/// every panel, grid and button shares the same look.
/// </summary>
internal static class Theme
{
    // ── Palette ───────────────────────────────────────────────────────────────
    public static readonly Color Backdrop    = Color.FromArgb(6, 13, 38);    // form background (darkest)
    public static readonly Color WindowTop    = Color.FromArgb(30, 56, 120);  // window gradient — top
    public static readonly Color WindowBottom = Color.FromArgb(12, 26, 74);   // window gradient — bottom
    public static readonly Color Window       = Color.FromArgb(17, 34, 86);   // flat window fill
    public static readonly Color RowAlt       = Color.FromArgb(23, 44, 104);  // alternating grid row
    public static readonly Color Frame        = Color.FromArgb(224, 234, 255); // bright light-blue frame
    public static readonly Color FrameDim     = Color.FromArgb(92, 126, 206);  // secondary line / gridlines
    public static readonly Color Text         = Color.White;
    public static readonly Color SubText      = Color.FromArgb(186, 216, 255); // muted blue-white
    public static readonly Color Gold         = Color.FromArgb(255, 210, 72);  // cursor / accent
    public static readonly Color MonsterName  = Color.FromArgb(170, 210, 255);
    public static readonly Color InfoGreen    = Color.FromArgb(150, 220, 160);

    public const string FontName = "Segoe UI";

    // ── Rounded-rectangle helper ──────────────────────────────────────────────
    public static GraphicsPath Rounded(Rectangle r, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0 || r.Width <= 0 || r.Height <= 0)
        {
            path.AddRectangle(r);
            path.CloseFigure();
            return path;
        }

        int d = radius * 2;
        d = Math.Min(d, Math.Min(r.Width, r.Height));
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    /// <summary>
    /// Paint a DQ command window (gradient fill + bright rounded frame) into rect.
    /// Used by container Paint handlers; children are inset via Padding so the
    /// rounded corners stay visible.
    /// </summary>
    public static void PaintWindow(Graphics g, Rectangle rect, int radius = 12)
    {
        if (rect.Width <= 2 || rect.Height <= 2)
        {
            return;
        }

        g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        using var path = Rounded(r, radius);
        using (var fill = new LinearGradientBrush(
                   new Rectangle(r.X, r.Y, r.Width, Math.Max(1, r.Height)),
                   WindowTop, WindowBottom, LinearGradientMode.Vertical))
        {
            g.FillPath(fill, path);
        }

        // Subtle inner line then the bright outer frame — mimics the game's double edge.
        using (var inner = new Pen(FrameDim, 1f))
        {
            using var ip = Rounded(new Rectangle(r.X + 2, r.Y + 2, r.Width - 4, r.Height - 4), Math.Max(1, radius - 2));
            g.DrawPath(inner, ip);
        }
        using var pen = new Pen(Frame, 2f);
        g.DrawPath(pen, path);
    }

    // ── Control styling helpers ───────────────────────────────────────────────
    public static void StyleButton(Button b, bool primary = false)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.BackColor = primary ? Color.FromArgb(22, 46, 108) : Window;
        b.ForeColor = primary ? Gold : Text;
        b.FlatAppearance.BorderColor = primary ? Gold : Frame;
        b.FlatAppearance.BorderSize = 2;
        b.FlatAppearance.MouseOverBackColor = Color.FromArgb(36, 66, 134);
        b.FlatAppearance.MouseDownBackColor = Color.FromArgb(52, 88, 166);
        b.Font = new Font(FontName, b.Font.Size, primary ? FontStyle.Bold : FontStyle.Regular);
        b.Cursor = Cursors.Hand;
    }

    public static void StyleGrid(DataGridView g)
    {
        g.BackgroundColor = Window;
        g.BorderStyle = BorderStyle.None;
        g.DefaultCellStyle.BackColor = Window;
        g.DefaultCellStyle.ForeColor = Text;
        g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(42, 74, 148);
        g.DefaultCellStyle.SelectionForeColor = Gold;
        g.DefaultCellStyle.Padding = new Padding(2, 1, 2, 1);
        g.AlternatingRowsDefaultCellStyle.BackColor = RowAlt;
        g.AlternatingRowsDefaultCellStyle.ForeColor = Text;
        g.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(42, 74, 148);
        g.AlternatingRowsDefaultCellStyle.SelectionForeColor = Gold;
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(10, 22, 62);
        g.ColumnHeadersDefaultCellStyle.ForeColor = SubText;
        g.ColumnHeadersDefaultCellStyle.Font = new Font(FontName, 9.5f, FontStyle.Bold);
        g.ColumnHeadersHeight = 30;
        g.EnableHeadersVisualStyles = false;
        g.GridColor = FrameDim;
        g.RowHeadersVisible = false;
        g.RowTemplate.Height = 26;
        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
    }

    /// <summary>Attach a DQ-window Paint border to a container; insets children via Padding.</summary>
    public static void FrameContainer(Control c, int radius = 12, int pad = 7)
    {
        c.BackColor = Backdrop;
        c.Padding = new Padding(pad);
        c.Paint += (s, e) => PaintWindow(e.Graphics, ((Control)s!).ClientRectangle, radius);
    }
}

// ── Navy menu / context-menu renderer ─────────────────────────────────────────
internal sealed class DqColorTable : ProfessionalColorTable
{
    public override Color MenuStripGradientBegin => Theme.Backdrop;
    public override Color MenuStripGradientEnd   => Theme.Backdrop;
    public override Color ToolStripDropDownBackground => Theme.Window;
    public override Color ImageMarginGradientBegin => Theme.Window;
    public override Color ImageMarginGradientMiddle => Theme.Window;
    public override Color ImageMarginGradientEnd => Theme.Window;
    public override Color MenuItemSelected => Color.FromArgb(42, 74, 148);
    public override Color MenuItemSelectedGradientBegin => Color.FromArgb(42, 74, 148);
    public override Color MenuItemSelectedGradientEnd => Color.FromArgb(42, 74, 148);
    public override Color MenuItemBorder => Theme.Frame;
    public override Color MenuBorder => Theme.Frame;
    public override Color MenuItemPressedGradientBegin => Theme.Window;
    public override Color MenuItemPressedGradientEnd => Theme.Window;
    public override Color SeparatorDark => Theme.FrameDim;
    public override Color SeparatorLight => Theme.FrameDim;
}

internal sealed class DqMenuRenderer : ToolStripProfessionalRenderer
{
    public DqMenuRenderer() : base(new DqColorTable()) { }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        e.TextColor = Theme.Text;
        base.OnRenderItemText(e);
    }
}
