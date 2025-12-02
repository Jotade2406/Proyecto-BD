using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Runtime.InteropServices;
using System.IO;

namespace GuarderiaVisual
{
    // =============================================================
    // 1. TEMA "NEON PRO"
    // =============================================================
    public static class Tema
    {
        public static Color Background = Color.FromArgb(20, 11, 31);
        public static Color SidebarBg = Color.FromArgb(28, 18, 45);
        public static Color NeonPurple = Color.FromArgb(147, 51, 234);
        public static Color NeonPink = Color.FromArgb(236, 72, 153);
        public static Color NeonCyan = Color.FromArgb(6, 182, 212);
        public static Color NeonGold = Color.FromArgb(255, 180, 0);
        public static Color Danger = Color.FromArgb(239, 68, 68);
        public static Color Success = Color.FromArgb(34, 197, 94);
        public static Color TextWhite = Color.FromArgb(245, 245, 245);
        public static Color TextGray = Color.FromArgb(160, 160, 180);
        public static Color CardDark = Color.FromArgb(40, 25, 60);
        public static Color BorderNeon = Color.FromArgb(139, 92, 246);
    }

    // =============================================================
    // 2. FORMULARIO PRINCIPAL
    // =============================================================
    public partial class Form1 : Form
    {
        // --- VARIABLE REPOSITORIO (GLOBAL PARA EL FORM) ---
        private GuarderiaRepository _repo = new GuarderiaRepository();

        private Panel panelContenedor;
        private BotonMenu btnEstudiantes, btnTutores, btnMenus, btnComedor, btnServicios, btnTienda, btnPagos, btnNomina;
        private UserControl vistaActual;
        private Panel panelSplash; private Timer timerCarga; private int progreso = 0; private Random rnd = new Random();

        public Form1()
        {
            this.Text = "Mi Pequeño Mundo";
            this.Size = new Size(1380, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Tema.Background;
            this.DoubleBuffered = true;

            ConstruirInterfaz();
            ConstruirSplash();
        }

        private void ConstruirSplash()
        {
            panelSplash = new Panel { Dock = DockStyle.Fill, BackColor = Tema.Background };
            this.Controls.Add(panelSplash); panelSplash.BringToFront();
            PictureBox pb = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.StretchImage };
            if (File.Exists("fondo.jpg")) pb.Image = Image.FromFile("fondo.jpg");
            panelSplash.Controls.Add(pb);
            Panel overlay = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(220, 20, 11, 31) }; pb.Controls.Add(overlay);
            Label l1 = new Label { Text = "MI PEQUEÑO MUNDO", Font = new Font("Segoe UI", 36, FontStyle.Bold), ForeColor = Tema.TextWhite, AutoSize = true, Location = new Point(50, 200), BackColor = Color.Transparent };
            Label l2 = new Label { Text = "Cargando Sistema demasiado insano...", Font = new Font("Segoe UI", 14), ForeColor = Tema.NeonPurple, AutoSize = true, Location = new Point(55, 270), BackColor = Color.Transparent };
            Panel barra = new Panel { Height = 8, Width = 0, BackColor = Tema.NeonCyan, Top = this.Height - 8, Left = 0 }; overlay.Controls.Add(barra); overlay.Controls.Add(l1); overlay.Controls.Add(l2);
            timerCarga = new Timer { Interval = 30 };
            timerCarga.Tick += (s, e) => {
                progreso += rnd.Next(1, 5); if (progreso > 100) progreso = 100;
                barra.Width = (int)((this.Width * progreso) / 100.0);
                if (progreso == 100) { timerCarga.Stop(); panelSplash.Dispose(); }
            };
            timerCarga.Start();
        }

        private void ConstruirInterfaz()
        {
            Panel sidebar = new Panel { Dock = DockStyle.Left, Width = 280, BackColor = Tema.SidebarBg, Padding = new Padding(0) };
            this.Controls.Add(sidebar);

            Panel logoPanel = new Panel { Dock = DockStyle.Top, Height = 160, BackColor = Color.Transparent, Padding = new Padding(10) };
            Label lblBalloon = new Label { Text = "🎈", Font = new Font("Segoe UI Emoji", 32), ForeColor = Tema.TextWhite, AutoSize = false, Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter, Height = 60 };
            Label lblTitle = new Label { Text = "MI PEQUEÑO\nMUNDO", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Tema.TextWhite, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter };
            logoPanel.Controls.Add(lblTitle); logoPanel.Controls.Add(lblBalloon); sidebar.Controls.Add(logoPanel);

            btnNomina = new BotonMenu("💼  NÓMINA EXPERTOS"); btnNomina.Click += (s, e) => Navegar("NOMINA"); sidebar.Controls.Add(btnNomina);
            btnPagos = new BotonMenu("💎  CAJA Y PAGOS"); btnPagos.Click += (s, e) => Navegar("PAGOS"); sidebar.Controls.Add(btnPagos);
            btnTienda = new BotonMenu("📦  TIENDA / STOCK"); btnTienda.Click += (s, e) => Navegar("TIENDA"); sidebar.Controls.Add(btnTienda);
            btnServicios = new BotonMenu("💊  SERVICIOS EXTRA"); btnServicios.Click += (s, e) => Navegar("SERVICIOS"); sidebar.Controls.Add(btnServicios);
            btnComedor = new BotonMenu("🍽  COMEDOR"); btnComedor.Click += (s, e) => Navegar("COMEDOR"); sidebar.Controls.Add(btnComedor);
            btnMenus = new BotonMenu("🍎  MENÚS & COCINA"); btnMenus.Click += (s, e) => Navegar("MENUS"); sidebar.Controls.Add(btnMenus);
            btnTutores = new BotonMenu("👥  TUTORES"); btnTutores.Click += (s, e) => Navegar("TUTORES"); sidebar.Controls.Add(btnTutores);
            btnEstudiantes = new BotonMenu("🎓  ESTUDIANTES"); btnEstudiantes.Click += (s, e) => Navegar("ESTUDIANTES"); sidebar.Controls.Add(btnEstudiantes);

            sidebar.Controls.Add(new Label { Text = "v20.0 Final", ForeColor = Color.Gray, Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleCenter, Height = 30, Font = new Font("Segoe UI", 7) });

            Panel header = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Tema.Background };
            header.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0xA1, 0x2, 0); } };
            this.Controls.Add(header);
            BotonControlVentana btnClose = new BotonControlVentana("✕", Tema.Danger); btnClose.Click += (s, e) => Application.Exit(); header.Controls.Add(btnClose);
            BotonControlVentana btnMin = new BotonControlVentana("—", Tema.TextGray); btnMin.Click += (s, e) => this.WindowState = FormWindowState.Minimized; header.Controls.Add(btnMin);

            panelContenedor = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30), BackColor = Tema.Background };
            this.Controls.Add(panelContenedor); panelContenedor.BringToFront();
            Navegar("ESTUDIANTES");
        }

        private void Navegar(string vista)
        {
            panelContenedor.Controls.Clear();
            btnEstudiantes.Desactivar(); btnTutores.Desactivar(); btnMenus.Desactivar(); btnComedor.Desactivar(); btnServicios.Desactivar(); btnPagos.Desactivar(); btnTienda.Desactivar(); btnNomina.Desactivar();
            switch (vista)
            {
                // AQUÍ SE USA _repo. AHORA SÍ EXISTE EN ESTE CONTEXTO.
                case "ESTUDIANTES": vistaActual = new VistaEstudiantes(_repo); btnEstudiantes.Activar(); break;
                case "TUTORES": vistaActual = new VistaTutores(_repo); btnTutores.Activar(); break;
                case "MENUS": vistaActual = new VistaMenus(_repo); btnMenus.Activar(); break;
                case "COMEDOR": vistaActual = new VistaComedor(_repo); btnComedor.Activar(); break;
                case "SERVICIOS": vistaActual = new VistaServicios(_repo); btnServicios.Activar(); break;
                case "TIENDA": vistaActual = new VistaTienda(_repo); btnTienda.Activar(); break;
                case "PAGOS": vistaActual = new VistaPagos(_repo); btnPagos.Activar(); break;
                case "NOMINA": vistaActual = new VistaNomina(_repo); btnNomina.Activar(); break;
            }
            if (vistaActual != null) { vistaActual.Dock = DockStyle.Fill; panelContenedor.Controls.Add(vistaActual); }
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")] private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")] private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
    }

    // =============================================================
    // 3. UI KIT 
    // =============================================================
    public class Tarjeta : Panel { public Color BorderColor { get; set; } = Tema.BorderNeon; public Tarjeta() { BackColor = Color.Transparent; Padding = new Padding(20); DoubleBuffered = true; } protected override void OnPaint(PaintEventArgs e) { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using (GraphicsPath path = UI.RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), 25)) { using (SolidBrush b = new SolidBrush(Tema.CardDark)) { e.Graphics.FillPath(b, path); } using (Pen p = new Pen(BorderColor, 1.5f)) { e.Graphics.DrawPath(p, path); } } } }
    public class BotonModerno : Button { Color c1, c2; public BotonModerno(string t, Color s, Color e) { Text = t; c1 = s; c2 = e; Size = new Size(300, 50); FlatStyle = FlatStyle.Flat; FlatAppearance.BorderSize = 0; ForeColor = Color.White; Font = new Font("Segoe UI", 11, FontStyle.Bold); Cursor = Cursors.Hand; } protected override void OnPaint(PaintEventArgs e) { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using (GraphicsPath p = UI.RoundedRect(new Rectangle(0, 0, Width, Height), 15)) { Region = new Region(p); using (LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, c1, c2, 0F)) { e.Graphics.FillPath(brush, p); } TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter); } } }
    public class InputModerno : Panel { public TextBox txt; public InputModerno(string ph) { Size = new Size(350, 55); BackColor = Color.Transparent; Padding = new Padding(15, 15, 10, 10); txt = new TextBox { BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 11), BackColor = Tema.SidebarBg, Width = 320, ForeColor = Color.White }; Controls.Add(txt); txt.Enter += (s, e) => { Refresh(); }; txt.Leave += (s, e) => { Refresh(); }; } protected override void OnPaint(PaintEventArgs e) { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; Color borderColor = txt.Focused ? Tema.NeonPink : Tema.BorderNeon; using (GraphicsPath p = UI.RoundedRect(new Rectangle(1, 1, Width - 2, Height - 2), 18)) { using (SolidBrush b = new SolidBrush(Tema.SidebarBg)) e.Graphics.FillPath(b, p); using (Pen pen = new Pen(borderColor, txt.Focused ? 2 : 1)) e.Graphics.DrawPath(pen, p); } } }
    public class BotonMenu : Button { private Panel indicador; public BotonMenu(string t) { Text = t; Dock = DockStyle.Top; Height = 60; FlatStyle = FlatStyle.Flat; FlatAppearance.BorderSize = 0; Font = new Font("Segoe UI", 10, FontStyle.Bold); TextAlign = ContentAlignment.MiddleLeft; Padding = new Padding(40, 0, 0, 0); Cursor = Cursors.Hand; ForeColor = Tema.TextGray; BackColor = Color.Transparent; indicador = new Panel { Width = 4, Dock = DockStyle.Left, BackColor = Color.Transparent, Visible = false }; this.Controls.Add(indicador); } public void Activar() { ForeColor = Color.White; BackColor = Color.FromArgb(40, 255, 255, 255); indicador.BackColor = Tema.NeonPink; indicador.Visible = true; } public void Desactivar() { ForeColor = Tema.TextGray; BackColor = Color.Transparent; indicador.Visible = false; } }
    public class BotonControlVentana : Button { public BotonControlVentana(string t, Color h) { Text = t; Dock = DockStyle.Right; Width = 50; FlatStyle = FlatStyle.Flat; FlatAppearance.BorderSize = 0; BackColor = Color.Transparent; Font = new Font("Arial", 11); FlatAppearance.MouseOverBackColor = h; ForeColor = Color.White; } }
    public static class UI { public static GraphicsPath RoundedRect(Rectangle r, int d) { GraphicsPath p = new GraphicsPath(); p.AddArc(r.X, r.Y, d, d, 180, 90); p.AddArc(r.X + r.Width - d, r.Y, d, d, 270, 90); p.AddArc(r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90); p.AddArc(r.X, r.Y + r.Height - d, d, d, 90, 90); p.CloseFigure(); return p; } public static InputModerno AddInput(FlowLayoutPanel p, string label) { p.Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Tema.TextGray, AutoSize = true, Margin = new Padding(5, 10, 0, 5) }); InputModerno inp = new InputModerno(label); p.Controls.Add(inp); return inp; } public static DateTimePicker AddDate(FlowLayoutPanel p, string label) { p.Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Tema.TextGray, AutoSize = true, Margin = new Padding(5, 10, 0, 5) }); DateTimePicker dt = new DateTimePicker { Width = 350, Font = new Font("Segoe UI", 11), Height = 45, Format = DateTimePickerFormat.Short, CalendarForeColor = Tema.Background, CalendarMonthBackground = Tema.TextWhite }; p.Controls.Add(dt); return dt; } public static DataGridView CreateGrid() { var d = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Tema.SidebarBg, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None, RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, GridColor = Tema.CardDark }; d.EnableHeadersVisualStyles = false; d.ColumnHeadersHeight = 50; d.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Tema.CardDark, ForeColor = Tema.NeonCyan, Font = new Font("Segoe UI", 10, FontStyle.Bold), SelectionBackColor = Tema.CardDark, SelectionForeColor = Tema.NeonCyan, Alignment = DataGridViewContentAlignment.MiddleLeft }; d.DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 10), ForeColor = Color.White, Padding = new Padding(10), SelectionBackColor = Tema.NeonPurple, SelectionForeColor = Color.White, BackColor = Tema.SidebarBg }; d.RowTemplate.Height = 50; return d; } }

    // =============================================================
    // 4. VISTAS
    // =============================================================
    public class VistaEstudiantes : UserControl
    {
        GuarderiaRepository _r; DataGridView _g; InputModerno _mat, _nom, _costo, _pagador, _ingrediente; DateTimePicker _nac, _ing;
        Panel _panelIzq;
        public VistaEstudiantes(GuarderiaRepository r) { _r = r; BackColor = Tema.Background; Init(); }
        void Init() { TableLayoutPanel t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(0) }; t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 450)); t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); Controls.Add(t); Tarjeta form = new Tarjeta { Dock = DockStyle.Fill, Padding = new Padding(20) }; Panel w = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 20, 0) }; w.Controls.Add(form); t.Controls.Add(w, 0, 0); _panelIzq = new Panel { Dock = DockStyle.Fill }; form.Controls.Add(_panelIzq); MostrarRegistro(); Tarjeta grid = new Tarjeta { Dock = DockStyle.Fill, Padding = new Padding(20) }; t.Controls.Add(grid, 1, 0); grid.Controls.Add(new Label { Text = "Directorio de Alumnos", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Tema.TextWhite, Dock = DockStyle.Top, Height = 50 }); _g = UI.CreateGrid(); _g.CellClick += (s, e) => { if (e.RowIndex >= 0) { var i = (Niño)_g.Rows[e.RowIndex].DataBoundItem; _mat.txt.Text = i.Matricula.ToString(); _nom.txt.Text = i.Nombre; _costo.txt.Text = i.CostoFijoMensual.ToString(); _pagador.txt.Text = i.CIPagador; _nac.Value = i.FechaNacimiento; _ing.Value = i.FechaIngreso; } }; grid.Controls.Add(_g); Load(); }
        void MostrarRegistro() { _panelIzq.Controls.Clear(); FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, WrapContents = false }; _panelIzq.Controls.Add(f); f.Controls.Add(new Label { Text = "Registrar Alumno", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Tema.TextWhite, AutoSize = true, Margin = new Padding(0, 0, 0, 20) }); _mat = UI.AddInput(f, "Matrícula"); _nom = UI.AddInput(f, "Nombre Completo"); _nac = UI.AddDate(f, "Nacimiento"); _ing = UI.AddDate(f, "Ingreso"); _costo = UI.AddInput(f, "Mensualidad ($)"); _pagador = UI.AddInput(f, "CI Apoderado"); f.Controls.Add(new Panel { Height = 20 }); BotonModerno b1 = new BotonModerno("Guardar Datos", Tema.NeonPurple, Tema.NeonPink); b1.Click += (s, e) => Save(); f.Controls.Add(b1); f.Controls.Add(new Panel { Height = 10 }); BotonModerno b2 = new BotonModerno("Eliminar", Color.FromArgb(50, 0, 0), Tema.Danger); b2.Click += (s, e) => Del(); f.Controls.Add(b2); f.Controls.Add(new Panel { Height = 10 }); BotonModerno b3 = new BotonModerno("⚠️ Alergias / Datos Médicos", Color.Orange, Color.DarkOrange); b3.Click += (s, e) => MostrarAlergias(); f.Controls.Add(b3); }
        void MostrarAlergias() { if (string.IsNullOrEmpty(_mat.txt.Text)) { MessageBox.Show("Selecciona un niño primero."); return; } _panelIzq.Controls.Clear(); FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, WrapContents = false }; _panelIzq.Controls.Add(f); Panel head = new Panel { Width = 350, Height = 40 }; BotonControlVentana btnBack = new BotonControlVentana("◀", Color.Gray); btnBack.Dock = DockStyle.Left; btnBack.Click += (s, e) => MostrarRegistro(); Label lblT = new Label { Text = "ALERGIAS", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.Orange, AutoSize = true, Left = 60, Top = 5 }; head.Controls.Add(lblT); head.Controls.Add(btnBack); f.Controls.Add(head); f.Controls.Add(new Label { Text = "Niño: " + _nom.txt.Text, Font = new Font("Segoe UI", 10), ForeColor = Color.White, Margin = new Padding(0, 10, 0, 20) }); _ingrediente = UI.AddInput(f, "Ingrediente Prohibido (Ej: Maní)"); f.Controls.Add(new Panel { Height = 20 }); BotonModerno btnAdd = new BotonModerno("AGREGAR ALERGIA", Tema.Danger, Color.Red); btnAdd.Click += (s, e) => GuardarAlergia(); f.Controls.Add(btnAdd); }
        void Load() { try { _g.DataSource = _r.ObtenerTodosNiños(); if (_g.Columns["CostoFijoMensual"] != null) _g.Columns["CostoFijoMensual"].DefaultCellStyle.Format = "C2"; } catch { } }
        void Save() { try { var n = new Niño { Matricula = int.Parse(_mat.txt.Text), Nombre = _nom.txt.Text, FechaNacimiento = _nac.Value, FechaIngreso = _ing.Value, CostoFijoMensual = decimal.Parse(_costo.txt.Text), CIPagador = _pagador.txt.Text }; if (_r.CrearNiño(n) || _r.ActualizarNiño(n)) { MessageBox.Show("¡Datos Guardados!"); Load(); } } catch { MessageBox.Show("Verifica los datos."); } }
        void Del() { if (int.TryParse(_mat.txt.Text, out int m) && _r.EliminarNiño(m)) { MessageBox.Show("Eliminado"); Load(); Clear(); } }
        void Clear() { _mat.txt.Text = ""; _nom.txt.Text = ""; _costo.txt.Text = ""; _pagador.txt.Text = ""; }
        void GuardarAlergia() { if (_r.RegistrarAlergia(int.Parse(_mat.txt.Text), _ingrediente.txt.Text)) { MessageBox.Show("Alergia Registrada."); _ingrediente.txt.Text = ""; } }
    }

    public class VistaTutores : UserControl
    {
        GuarderiaRepository _r; DataGridView _g; InputModerno _ci, _nom, _dir, _tel;
        public VistaTutores(GuarderiaRepository r) { _r = r; BackColor = Tema.Background; Init(); }
        void Init() { TableLayoutPanel t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(0) }; t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 450)); t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); Controls.Add(t); Tarjeta form = new Tarjeta { Dock = DockStyle.Fill, Padding = new Padding(20) }; Panel w = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 20, 0) }; w.Controls.Add(form); t.Controls.Add(w, 0, 0); FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, WrapContents = false }; form.Controls.Add(f); f.Controls.Add(new Label { Text = "Registrar Tutor", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Tema.TextWhite, AutoSize = true, Margin = new Padding(0, 0, 0, 20) }); _ci = UI.AddInput(f, "Cédula"); _nom = UI.AddInput(f, "Nombre"); _dir = UI.AddInput(f, "Dirección"); _tel = UI.AddInput(f, "Teléfono"); f.Controls.Add(new Panel { Height = 30 }); BotonModerno b = new BotonModerno("Guardar Tutor", Tema.NeonPurple, Tema.NeonPink); b.Click += (s, e) => { if (_r.CrearPersona(new Persona { CI = _ci.txt.Text, Nombre = _nom.txt.Text, Direccion = _dir.txt.Text, Telefono = _tel.txt.Text })) { MessageBox.Show("Guardado"); Load(); } }; f.Controls.Add(b); f.Controls.Add(new Panel { Height = 15 }); BotonModerno bDel = new BotonModerno("Eliminar Seleccionado", Tema.Danger, Color.Maroon); bDel.Click += (s, e) => Eliminar(); f.Controls.Add(bDel); Tarjeta grid = new Tarjeta { Dock = DockStyle.Fill, Padding = new Padding(20) }; t.Controls.Add(grid, 1, 0); grid.Controls.Add(new Label { Text = "Lista de Tutores", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Tema.TextWhite, Dock = DockStyle.Top, Height = 50 }); _g = UI.CreateGrid(); _g.CellClick += (s, e) => { if (e.RowIndex >= 0) { var i = (Persona)_g.Rows[e.RowIndex].DataBoundItem; _ci.txt.Text = i.CI; _nom.txt.Text = i.Nombre; _dir.txt.Text = i.Direccion; _tel.txt.Text = i.Telefono; } }; grid.Controls.Add(_g); Load(); }
        void Load() { try { _g.DataSource = _r.ObtenerTodasPersonas(); } catch { } }
        void Eliminar() { if (string.IsNullOrEmpty(_ci.txt.Text)) { MessageBox.Show("Selecciona un tutor."); return; } if (MessageBox.Show("¿Eliminar Tutor?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes) { if (_r.EliminarPersona(_ci.txt.Text)) { MessageBox.Show("Eliminado"); Load(); _ci.txt.Text = ""; } } }
    }

    public class VistaMenus : UserControl
    {
        GuarderiaRepository _r; DataGridView _g; InputModerno _nom, _costo, _ingredientes;
        public VistaMenus(GuarderiaRepository r) { _r = r; BackColor = Tema.Background; Init(); }
        void Init()
        {
            TableLayoutPanel t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(0) };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); Controls.Add(t);
            Tarjeta form = new Tarjeta { Dock = DockStyle.Fill, Padding = new Padding(20) }; Panel w = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 20, 0) }; w.Controls.Add(form); t.Controls.Add(w, 0, 0);
            FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, WrapContents = false }; form.Controls.Add(f);
            f.Controls.Add(new Label { Text = "COCINA: NUEVO MENÚ", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Tema.TextWhite, AutoSize = true, Margin = new Padding(0, 0, 0, 20) });
            _nom = UI.AddInput(f, "Nombre del Plato"); _costo = UI.AddInput(f, "Costo ($)"); _ingredientes = UI.AddInput(f, "Ingredientes (Separar con comas)");
            f.Controls.Add(new Panel { Height = 20 }); BotonModerno btn = new BotonModerno("Guardar Menú", Tema.NeonPurple, Tema.NeonPink); btn.Click += (s, e) => Save(); f.Controls.Add(btn);
            Tarjeta grid = new Tarjeta { Dock = DockStyle.Fill, Padding = new Padding(20) }; t.Controls.Add(grid, 1, 0); grid.Controls.Add(new Label { Text = "Menús Disponibles", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Tema.TextWhite, Dock = DockStyle.Top, Height = 50 }); _g = UI.CreateGrid(); grid.Controls.Add(_g); Load();
        }
        void Load() { try { _g.DataSource = _r.ObtenerMenus(); } catch { } }
        void Save() { try { decimal costo; if (!decimal.TryParse(_costo.txt.Text.Replace('.', ','), out costo)) { MessageBox.Show("El costo debe ser numérico"); return; } if (_r.GuardarMenuCompleto(_nom.txt.Text, costo, _ingredientes.txt.Text)) { MessageBox.Show("Menú e Ingredientes Registrados!"); Load(); _nom.txt.Text = ""; _costo.txt.Text = ""; _ingredientes.txt.Text = ""; } } catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); } }
    }

    public class VistaComedor : UserControl
    {
        GuarderiaRepository _r; ComboBox _cNiño, _cMenu; DateTimePicker _dt;
        public VistaComedor(GuarderiaRepository r) { _r = r; BackColor = Tema.Background; Init(); }
        void Init() { Tarjeta card = new Tarjeta { Width = 600, Height = 550, BorderColor = Tema.NeonCyan }; card.Location = new Point((1100 - 600) / 2, 50); Controls.Add(card); FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(40) }; card.Controls.Add(f); f.Controls.Add(new Label { Text = "TERMINAL DE PEDIDOS", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Tema.NeonCyan, AutoSize = true, Margin = new Padding(0, 0, 0, 30) }); f.Controls.Add(new Label { Text = "1. Seleccionar Estudiante:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Tema.TextGray }); _cNiño = StyleCombo(); f.Controls.Add(_cNiño); f.Controls.Add(new Label { Text = "2. Elegir Menú:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Tema.TextGray }); _cMenu = StyleCombo(); f.Controls.Add(_cMenu); f.Controls.Add(new Label { Text = "3. Fecha Consumo:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Tema.TextGray }); _dt = new DateTimePicker { Width = 480, Height = 45, Font = new Font("Segoe UI", 12), Format = DateTimePickerFormat.Short, Margin = new Padding(0, 10, 0, 30) }; f.Controls.Add(_dt); BotonModerno btn = new BotonModerno("CONFIRMAR PEDIDO", Tema.NeonCyan, Color.FromArgb(0, 200, 255)); btn.Width = 480; btn.Click += (s, e) => Add(); f.Controls.Add(btn); try { _cNiño.DataSource = _r.ObtenerTodosNiños(); _cNiño.DisplayMember = "Nombre"; _cNiño.ValueMember = "Matricula"; _cMenu.DataSource = _r.ObtenerMenus(); _cMenu.DisplayMember = "Nombre"; _cMenu.ValueMember = "NumMenu"; } catch { } }
        ComboBox StyleCombo() => new ComboBox { Width = 480, Height = 45, Font = new Font("Segoe UI", 14), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Tema.SidebarBg, ForeColor = Tema.TextWhite, Margin = new Padding(0, 5, 0, 20) };
        void Add()
        {
            if (_cNiño.SelectedValue == null || _cMenu.SelectedValue == null) return;
            string alergia = _r.VerificarAlergia((int)_cNiño.SelectedValue, (int)_cMenu.SelectedValue);
            if (alergia != null) { MessageBox.Show($"⛔ ¡PELIGRO! ALERGIA DETECTADA ⛔\n\nEl niño es alérgico al ingrediente: {alergia}", "SEGURIDAD", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (_r.RegistrarConsumo((int)_cNiño.SelectedValue, (int)_cMenu.SelectedValue, _dt.Value)) MessageBox.Show("¡Pedido Registrado Correctamente!");
        }
    }

    public class VistaServicios : UserControl
    {
        GuarderiaRepository _r; ComboBox _cNiño, _cServ, _cEsp; DateTimePicker _dt; InputModerno _obs;
        public VistaServicios(GuarderiaRepository r) { _r = r; BackColor = Tema.Background; Init(); }
        void Init()
        {
            Tarjeta card = new Tarjeta { Width = 600, Height = 750, BorderColor = Tema.NeonGold }; card.Location = new Point((1100 - 600) / 2, 20); Controls.Add(card);
            FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(30) }; card.Controls.Add(f);
            f.Controls.Add(new Label { Text = "SERVICIOS EXTRA", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Tema.NeonGold, AutoSize = true, Margin = new Padding(0, 0, 0, 20) });
            f.Controls.Add(new Label { Text = "Estudiante:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Tema.TextGray }); _cNiño = StyleCombo(); f.Controls.Add(_cNiño);
            f.Controls.Add(new Label { Text = "Servicio:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Tema.TextGray }); _cServ = StyleCombo();
            _cServ.SelectedIndexChanged += (s, e) => FiltrarEspecialistas(); f.Controls.Add(_cServ);
            f.Controls.Add(new Label { Text = "Especialista Responsable:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Tema.NeonGold }); _cEsp = StyleCombo(); f.Controls.Add(_cEsp);
            f.Controls.Add(new Label { Text = "Fecha:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Tema.TextGray }); _dt = new DateTimePicker { Width = 480, Height = 45, Font = new Font("Segoe UI", 12), Format = DateTimePickerFormat.Short, Margin = new Padding(0, 10, 0, 20) }; f.Controls.Add(_dt);
            _obs = UI.AddInput(f, "Observación"); _obs.txt.Width = 450;
            f.Controls.Add(new Panel { Height = 20 });
            BotonModerno btn = new BotonModerno("REGISTRAR CARGO", Tema.NeonGold, Color.Orange); btn.Width = 480; btn.Click += (s, e) => Add(); f.Controls.Add(btn);
            try
            {
                _cNiño.DataSource = _r.ObtenerTodosNiños(); _cNiño.DisplayMember = "Nombre"; _cNiño.ValueMember = "Matricula";
                _cServ.DataSource = _r.ObtenerServicios(); _cServ.DisplayMember = "Nombre"; _cServ.ValueMember = "IdServicio";
                _cEsp.DataSource = _r.ObtenerEspecialistas("TODOS"); _cEsp.DisplayMember = "Nombre"; _cEsp.ValueMember = "IdEspecialista";
            }
            catch { }
        }
        void FiltrarEspecialistas() { if (_cServ.SelectedItem == null) return; var serv = (Servicio)_cServ.SelectedItem; _cEsp.DataSource = _r.ObtenerEspecialistas(serv.Categoria); _cEsp.DisplayMember = "Nombre"; _cEsp.ValueMember = "IdEspecialista"; }
        ComboBox StyleCombo() => new ComboBox { Width = 480, Height = 45, Font = new Font("Segoe UI", 14), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Tema.SidebarBg, ForeColor = Tema.TextWhite, Margin = new Padding(0, 5, 0, 10) };
        void Add()
        {
            if (_cNiño.SelectedValue == null) { MessageBox.Show("Seleccione un niño"); return; }
            if (_cServ.SelectedValue == null) { MessageBox.Show("Seleccione un servicio"); return; }
            int idEsp = (_cEsp.SelectedValue != null) ? (int)_cEsp.SelectedValue : 0;
            if (_r.RegistrarConsumoServicio((int)_cNiño.SelectedValue, (int)_cServ.SelectedValue, idEsp, _dt.Value, _obs.txt.Text)) MessageBox.Show("¡Servicio registrado con éxito!");
        }
    }

    // --- VISTA TIENDA (NUEVA: REPORTE CURSOR) ---
    public class VistaTienda : UserControl
    {
        GuarderiaRepository _r; DataGridView _g; Panel _panelIzq; ComboBox _cNiño, _cProd, _cProdStock; InputModerno _qtyStock, _nom, _stock, _min, _precio;
        public VistaTienda(GuarderiaRepository r) { _r = r; BackColor = Tema.Background; Init(); }
        void Init()
        {
            TableLayoutPanel t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(0) };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); Controls.Add(t);
            Tarjeta formCard = new Tarjeta { Dock = DockStyle.Fill, Padding = new Padding(20) }; Panel w = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 20, 0) }; w.Controls.Add(formCard); t.Controls.Add(w, 0, 0);
            _panelIzq = new Panel { Dock = DockStyle.Fill }; formCard.Controls.Add(_panelIzq);
            MostrarVenta();
            Tarjeta grid = new Tarjeta { Dock = DockStyle.Fill, Padding = new Padding(20) }; t.Controls.Add(grid, 1, 0);
            grid.Controls.Add(new Label { Text = "Inventario y Alertas", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Tema.TextWhite, Dock = DockStyle.Top, Height = 50 });
            _g = UI.CreateGrid();
            _g.CellFormatting += (s, e) => { if (e.RowIndex >= 0 && _g.Columns[e.ColumnIndex].Name == "StockActual") { var cellMin = _g.Rows[e.RowIndex].Cells["StockMinimo"].Value; if (cellMin != null && Convert.ToInt32(e.Value) <= Convert.ToInt32(cellMin)) { _g.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(100, 255, 0, 0); _g.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White; } } };
            grid.Controls.Add(_g); LoadData();
        }
        void MostrarVenta()
        {
            _panelIzq.Controls.Clear(); FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, WrapContents = false }; _panelIzq.Controls.Add(f);
            f.Controls.Add(new Label { Text = "DESPACHAR A NIÑO", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Tema.NeonCyan, AutoSize = true, Margin = new Padding(0, 0, 0, 20) });
            f.Controls.Add(new Label { Text = "Niño:", Font = new Font("Segoe UI", 10), ForeColor = Color.White }); _cNiño = StyleCombo(); f.Controls.Add(_cNiño);
            f.Controls.Add(new Label { Text = "Producto:", Font = new Font("Segoe UI", 10), ForeColor = Color.White }); _cProd = StyleCombo(); f.Controls.Add(_cProd);
            BotonModerno btnSell = new BotonModerno("ENTREGAR (CARGAR CUENTA)", Tema.NeonCyan, Color.DodgerBlue); btnSell.Width = 350; btnSell.Click += (s, e) => Vender(); f.Controls.Add(btnSell);
            f.Controls.Add(new Panel { Height = 40 }); f.Controls.Add(new Label { Text = "ACCIONES DE STOCK", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true });
            // REQUISITO CURSOR: BOTÓN REPORTE
            BotonModerno btnCursor = new BotonModerno("📋 REPORTE REPOSICIÓN (CURSOR)", Tema.NeonGold, Color.Orange); btnCursor.Width = 350; btnCursor.Height = 40; btnCursor.Click += (s, e) => GenerarReporteCursor(); f.Controls.Add(btnCursor);
            f.Controls.Add(new Panel { Height = 10 });
            BotonModerno btnRepo = new BotonModerno("📦 Reponer Stock", Tema.Success, Color.LimeGreen); btnRepo.Width = 350; btnRepo.Height = 40; btnRepo.Click += (s, e) => MostrarReposicion(); f.Controls.Add(btnRepo);
            f.Controls.Add(new Panel { Height = 10 }); BotonModerno btnNew = new BotonModerno("✨ Crear Nuevo Producto", Color.Purple, Color.MediumPurple); btnNew.Width = 350; btnNew.Height = 40; btnNew.Click += (s, e) => MostrarNuevo(); f.Controls.Add(btnNew);
            LoadData();
        }
        void MostrarReposicion()
        {
            _panelIzq.Controls.Clear(); FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, WrapContents = false }; _panelIzq.Controls.Add(f);
            Panel head = new Panel { Width = 350, Height = 40 }; BotonControlVentana btnBack = new BotonControlVentana("✕", Color.Red); btnBack.Click += (s, e) => MostrarVenta(); Label lblT = new Label { Text = "REPONER STOCK", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Tema.Success, AutoSize = true }; head.Controls.Add(lblT); head.Controls.Add(btnBack); f.Controls.Add(head);
            f.Controls.Add(new Label { Text = "Producto:", Font = new Font("Segoe UI", 10), ForeColor = Color.White, Margin = new Padding(0, 20, 0, 0) }); _cProdStock = StyleCombo(); f.Controls.Add(_cProdStock);
            _qtyStock = UI.AddInput(f, "Cantidad a Ingresar"); _qtyStock.txt.Width = 320;
            f.Controls.Add(new Panel { Height = 20 }); BotonModerno btnRestock = new BotonModerno("CONFIRMAR INGRESO", Tema.Success, Color.LimeGreen); btnRestock.Width = 350; btnRestock.Click += (s, e) => Reponer(); f.Controls.Add(btnRestock); LoadData();
        }
        void MostrarNuevo()
        {
            _panelIzq.Controls.Clear(); FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, WrapContents = false }; _panelIzq.Controls.Add(f);
            Panel head = new Panel { Width = 350, Height = 40 }; BotonControlVentana btnBack = new BotonControlVentana("✕", Color.Red); btnBack.Click += (s, e) => MostrarVenta(); Label lblT = new Label { Text = "NUEVO PRODUCTO", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.Purple, AutoSize = true }; head.Controls.Add(lblT); head.Controls.Add(btnBack); f.Controls.Add(head);
            _nom = UI.AddInput(f, "Nombre"); _stock = UI.AddInput(f, "Stock Inicial"); _min = UI.AddInput(f, "Stock Mínimo"); _precio = UI.AddInput(f, "Precio Unitario ($)");
            f.Controls.Add(new Panel { Height = 20 }); BotonModerno btn = new BotonModerno("CREAR PRODUCTO", Color.Purple, Color.MediumPurple); btn.Width = 350; btn.Click += (s, e) => Save(); f.Controls.Add(btn);
        }
        ComboBox StyleCombo() => new ComboBox { Width = 350, Height = 40, Font = new Font("Segoe UI", 12), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Tema.SidebarBg, ForeColor = Tema.TextWhite, Margin = new Padding(0, 0, 0, 15) };
        void LoadData() { try { var prods = _r.ObtenerProductos(); if (_g != null) _g.DataSource = prods; var ninos = _r.ObtenerTodosNiños(); if (_cProd != null) { _cProd.DataSource = new List<Producto>(prods); _cProd.DisplayMember = "Nombre"; _cProd.ValueMember = "IdProducto"; } if (_cNiño != null) { _cNiño.DataSource = ninos; _cNiño.DisplayMember = "Nombre"; _cNiño.ValueMember = "Matricula"; } if (_cProdStock != null) { _cProdStock.DataSource = new List<Producto>(prods); _cProdStock.DisplayMember = "Nombre"; _cProdStock.ValueMember = "IdProducto"; } } catch { } }
        void Save() { try { if (_r.GuardarProducto(_nom.txt.Text, int.Parse(_stock.txt.Text), int.Parse(_min.txt.Text), decimal.Parse(_precio.txt.Text))) { MessageBox.Show("Producto Creado"); MostrarVenta(); } } catch { MessageBox.Show("Datos inválidos"); } }
        void Vender() { if (_cNiño.SelectedValue != null && _cProd.SelectedValue != null && _r.RegistrarConsumoTienda((int)_cNiño.SelectedValue, (int)_cProd.SelectedValue)) { MessageBox.Show("Entregado"); LoadData(); } else MessageBox.Show("Error o Sin Stock"); }
        void Reponer() { if (_cProdStock.SelectedValue != null && int.TryParse(_qtyStock.txt.Text, out int qty) && _r.AumentarStock((int)_cProdStock.SelectedValue, qty)) { MessageBox.Show("Stock Aumentado"); MostrarVenta(); } }
        void GenerarReporteCursor()
        {
            var reporte = _r.GenerarReporteReposicionCursor();
            if (reporte.Count > 0)
            {
                string msg = "ALERTA DE REPOSICIÓN (GENERADO POR CURSOR SQL):\n\n";
                foreach (var r in reporte) msg += $"• {r.Producto}: Faltan {r.Faltante} (Stock: {r.StockActual})\n";
                MessageBox.Show(msg, "Reporte de Compra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else { MessageBox.Show("Inventario Saludable. No se requiere reposición.", "Reporte Cursor"); }
        }
    }

    // --- VISTA NOMINA ---
    public class VistaNomina : UserControl
    {
        GuarderiaRepository _r; ComboBox _cM; DataGridView _g; Label _lTotal;
        public VistaNomina(GuarderiaRepository r) { _r = r; BackColor = Tema.Background; Init(); }
        void Init()
        {
            TableLayoutPanel t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(20) }; t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); Controls.Add(t);
            Tarjeta left = new Tarjeta { Dock = DockStyle.Fill }; t.Controls.Add(left, 0, 0);
            FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(20) }; left.Controls.Add(f);
            f.Controls.Add(new Label { Text = "NÓMINA", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Tema.NeonGold, AutoSize = true, Margin = new Padding(0, 0, 0, 30) });
            f.Controls.Add(new Label { Text = "Mes:", Font = new Font("Segoe UI", 12), ForeColor = Color.White });
            _cM = new ComboBox { Width = 250, Font = new Font("Segoe UI", 12), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Tema.SidebarBg, ForeColor = Color.White };
            _cM.Items.AddRange(new object[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" }); _cM.SelectedIndex = DateTime.Now.Month - 1; f.Controls.Add(_cM);
            f.Controls.Add(new Panel { Height = 20 });
            BotonModerno btn = new BotonModerno("GENERAR REPORTE", Color.Gold, Color.Orange); btn.Width = 250; btn.Click += (s, e) => Generar(); f.Controls.Add(btn);

            Tarjeta right = new Tarjeta { Dock = DockStyle.Fill }; t.Controls.Add(right, 1, 0);
            right.Controls.Add(new Label { Text = "Ingresos Generados por Especialista", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 50 });
            _g = UI.CreateGrid(); right.Controls.Add(_g);
            _lTotal = new Label { Text = "TOTAL MES: $0.00", Font = new Font("Consolas", 18, FontStyle.Bold), ForeColor = Color.Gold, Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleRight, Height = 50 }; right.Controls.Add(_lTotal);
        }
        void Generar()
        {
            var datos = _r.ObtenerReporteNomina(_cM.SelectedIndex + 1);
            _g.DataSource = datos;
            decimal sum = 0; foreach (var d in datos) sum += d.TotalGenerado;
            _lTotal.Text = "TOTAL MES: " + sum.ToString("C2");
        }
    }

    // --- VISTA PAGOS ---
    public class VistaPagos : UserControl
    {
        GuarderiaRepository _r; ComboBox _cN; Label _l1, _l2, _l3, _l4, _lTotal; Panel _pB; decimal _tot; int _mat;
        public VistaPagos(GuarderiaRepository r) { _r = r; BackColor = Tema.Background; Init(); }
        void Init()
        {
            TableLayoutPanel t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(20) }; t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); Controls.Add(t);
            Tarjeta ctrl = new Tarjeta { Dock = DockStyle.Fill }; t.Controls.Add(ctrl, 0, 0); FlowLayoutPanel f = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(20) }; ctrl.Controls.Add(f);
            f.Controls.Add(new Label { Text = "CAJA PRINCIPAL", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Tema.TextWhite, AutoSize = true, Margin = new Padding(0, 0, 0, 40) }); f.Controls.Add(new Label { Text = "Buscar Deudor:", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Tema.TextGray }); _cN = new ComboBox { Width = 300, Font = new Font("Segoe UI", 14), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Tema.SidebarBg, ForeColor = Color.White }; f.Controls.Add(_cN); f.Controls.Add(new Panel { Height = 40 }); BotonModerno bC = new BotonModerno("CALCULAR DEUDA TOTAL", Tema.NeonPurple, Tema.NeonPink); bC.Click += (s, e) => Calc(); f.Controls.Add(bC);
            _pB = new Panel { Dock = DockStyle.Fill, Visible = false, Padding = new Padding(30) }; t.Controls.Add(_pB, 1, 0); Tarjeta paper = new Tarjeta { Dock = DockStyle.Top, Height = 650, BorderColor = Tema.Success }; _pB.Controls.Add(paper);
            Label lblHeader = new Label { Text = "ESTADO DE CUENTA", Font = new Font("Consolas", 22, FontStyle.Bold), ForeColor = Tema.Success, Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter, Height = 80 }; paper.Controls.Add(lblHeader); FlowLayoutPanel fp = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(30) }; paper.Controls.Add(fp); fp.BringToFront();
            _l1 = Ln(fp, "Mensualidad Base"); _l2 = Ln(fp, "Comedor (Acumulado)"); _l3 = Ln(fp, "Servicios Extra"); _l4 = Ln(fp, "Tienda (Pañales/etc)"); Panel div = new Panel { Height = 4, Width = 450, BackColor = Tema.Success, Margin = new Padding(0, 20, 0, 20) }; fp.Controls.Add(div); _lTotal = Ln(fp, "TOTAL A PAGAR"); _lTotal.ForeColor = Tema.Success; _lTotal.Font = new Font("Consolas", 20, FontStyle.Bold);
            fp.Controls.Add(new Panel { Height = 20 }); BotonModerno bQR = new BotonModerno("📲  VER QR PAGO", Color.Black, Color.White); bQR.Width = 450; bQR.Click += (s, e) => ShowQR(); fp.Controls.Add(bQR); fp.Controls.Add(new Panel { Height = 15 }); BotonModerno bP = new BotonModerno("✅  PAGO REALIZADO", Tema.Success, Color.LimeGreen); bP.Width = 450; bP.Click += (s, e) => Pay(); fp.Controls.Add(bP);
            try { _cN.DataSource = _r.ObtenerTodosNiños(); _cN.DisplayMember = "Nombre"; _cN.ValueMember = "Matricula"; } catch { }
        }
        Label Ln(FlowLayoutPanel p, string t) { FlowLayoutPanel r = new FlowLayoutPanel { Width = 480, Height = 40 }; r.Controls.Add(new Label { Text = t, Width = 280, Font = new Font("Consolas", 12), ForeColor = Tema.TextWhite }); Label v = new Label { Text = "$0.00", Width = 180, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Consolas", 14, FontStyle.Bold), ForeColor = Tema.TextWhite }; r.Controls.Add(v); p.Controls.Add(r); return v; }
        void Calc() { if (_cN.SelectedItem == null) return; _mat = (int)_cN.SelectedValue; var n = (Niño)_cN.SelectedItem; decimal f = n.CostoFijoMensual; decimal c = _r.CalcularDeudaTotalComedor(_mat); decimal s = _r.CalcularDeudaServicios(_mat); decimal ti = _r.CalcularDeudaTienda(_mat); _tot = f + c + s + ti; _l1.Text = f.ToString("C2"); _l2.Text = c.ToString("C2"); _l3.Text = s.ToString("C2"); _l4.Text = ti.ToString("C2"); _lTotal.Text = _tot.ToString("C2"); _pB.Visible = true; }
        void Pay() { if (_tot <= 0) { MessageBox.Show("No hay deuda pendiente."); return; } if (_r.RegistrarPagoTotal(_mat, _tot)) { MessageBox.Show("¡PAGO REGISTRADO EXITOSAMENTE!"); _pB.Visible = false; } }
        void ShowQR() { Form f = new Form { Size = new Size(400, 500), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedToolWindow }; PictureBox p = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom }; if (File.Exists("qr.png")) p.Image = Image.FromFile("qr.png"); else MessageBox.Show("Falta qr.png"); f.Controls.Add(p); f.ShowDialog(); }
    }

    // =============================================================
    // 5. REPOSITORIO Y MODELOS
    // =============================================================
    public class Niño { public int Matricula { get; set; } public string Nombre { get; set; } public DateTime FechaNacimiento { get; set; } public DateTime FechaIngreso { get; set; } public decimal CostoFijoMensual { get; set; } public string CIPagador { get; set; } }
    public class Persona { public string CI { get; set; } public string Nombre { get; set; } public string Direccion { get; set; } public string Telefono { get; set; } public string NumCuenta { get; set; } }
    public class Menu { public int NumMenu { get; set; } public string Nombre { get; set; } public decimal CostoBaseComida { get; set; } }
    public class Servicio { public int IdServicio { get; set; } public string Nombre { get; set; } public decimal CostoBase { get; set; } public string Categoria { get; set; } }
    public class Especialista { public int IdEspecialista { get; set; } public string Nombre { get; set; } }
    public class Producto { public int IdProducto { get; set; } public string Nombre { get; set; } public int StockActual { get; set; } public int StockMinimo { get; set; } public decimal PrecioUnitario { get; set; } }
    public class ReporteNomina { public string Especialista { get; set; } public string Especialidad { get; set; } public int Atenciones { get; set; } public decimal TotalGenerado { get; set; } }
    public class ReporteReposicion { public string Producto { get; set; } public int StockActual { get; set; } public int Faltante { get; set; } }

    public class GuarderiaRepository
    {
        string cs = "Server=localhost;Port=3306;Database=GuarderiaDB;Uid=root;Pwd=123456;";
        MySqlConnection C() => new MySqlConnection(cs);
        public List<Niño> ObtenerTodosNiños() { var l = new List<Niño>(); using (var c = C()) { c.Open(); using (var m = new MySqlCommand("SELECT * FROM NINO", c)) using (var r = m.ExecuteReader()) while (r.Read()) l.Add(new Niño { Matricula = r.GetInt32(0), Nombre = r.GetString(1), FechaNacimiento = r.GetDateTime(2), FechaIngreso = r.GetDateTime(3), CostoFijoMensual = r.GetDecimal(5), CIPagador = r.GetString(6) }); } return l; }
        public bool CrearNiño(Niño n) { using (var c = C()) { try { c.Open(); new MySqlCommand($"INSERT INTO NINO VALUES ({n.Matricula},'{n.Nombre}','{n.FechaNacimiento:yyyy-MM-dd}','{n.FechaIngreso:yyyy-MM-dd}',NULL,{n.CostoFijoMensual},'{n.CIPagador}')", c).ExecuteNonQuery(); return true; } catch { return false; } } }
        public bool ActualizarNiño(Niño n) { using (var c = C()) { try { c.Open(); new MySqlCommand($"UPDATE NINO SET Nombre='{n.Nombre}', CostoFijoMensual={n.CostoFijoMensual}, CIPagador='{n.CIPagador}' WHERE Matricula={n.Matricula}", c).ExecuteNonQuery(); return true; } catch { return false; } } }
        public bool EliminarNiño(int id) { using (var c = C()) { try { c.Open(); new MySqlCommand("DELETE FROM NINO WHERE Matricula=" + id, c).ExecuteNonQuery(); return true; } catch { return false; } } }
        public List<Persona> ObtenerTodasPersonas() { var l = new List<Persona>(); using (var c = C()) { c.Open(); using (var r = new MySqlCommand("SELECT * FROM PERSONA", c).ExecuteReader()) while (r.Read()) l.Add(new Persona { CI = r.GetString(0), Nombre = r.GetString(1), Direccion = r.GetString(2), Telefono = r.GetString(3) }); } return l; }
        public bool CrearPersona(Persona p) { using (var c = C()) { try { c.Open(); new MySqlCommand($"INSERT INTO PERSONA VALUES ('{p.CI}','{p.Nombre}','{p.Direccion}','{p.Telefono}',NULL)", c).ExecuteNonQuery(); return true; } catch { return false; } } }
        public bool EliminarPersona(string ci) { using (var c = C()) { try { c.Open(); var cmd = new MySqlCommand("DELETE FROM PERSONA WHERE CI=@c", c); cmd.Parameters.AddWithValue("@c", ci); return cmd.ExecuteNonQuery() > 0; } catch { return false; } } }
        public List<Menu> ObtenerMenus() { var l = new List<Menu>(); using (var c = C()) { c.Open(); using (var r = new MySqlCommand("SELECT * FROM MENU", c).ExecuteReader()) while (r.Read()) l.Add(new Menu { NumMenu = r.GetInt32(0), Nombre = r.GetString(1), CostoBaseComida = r.GetDecimal(2) }); } return l; }
        public bool RegistrarConsumo(int mat, int men, DateTime f) { using (var c = C()) { try { c.Open(); new MySqlCommand($"INSERT INTO CONSUMO_DIARIO (Matricula_Niño, Fecha, NumMenu_Consumido, Estado) VALUES ({mat},'{f:yyyy-MM-dd}',{men},'PENDIENTE')", c).ExecuteNonQuery(); return true; } catch { return false; } } }
        public decimal CalcularDeudaTotalComedor(int mat) { using (var c = C()) { c.Open(); var obj = new MySqlCommand($"SELECT IFNULL(SUM(m.CostoBaseComida),0) FROM CONSUMO_DIARIO cd JOIN MENU m ON cd.NumMenu_Consumido=m.NumMenu WHERE cd.Matricula_Niño={mat} AND cd.Estado='PENDIENTE'", c).ExecuteScalar(); return Convert.ToDecimal(obj); } }

        public List<Servicio> ObtenerServicios() { var l = new List<Servicio>(); using (var c = C()) { c.Open(); using (var r = new MySqlCommand("SELECT * FROM SERVICIO", c).ExecuteReader()) while (r.Read()) l.Add(new Servicio { IdServicio = r.GetInt32(0), Nombre = r.GetString(1), CostoBase = r.GetDecimal(2), Categoria = r.GetString("Categoria") }); } return l; }
        public List<Especialista> ObtenerEspecialistas(string categoria) { var l = new List<Especialista>(); string q = (categoria == "TODOS") ? "SELECT * FROM ESPECIALISTA" : $"SELECT * FROM ESPECIALISTA WHERE Categoria = '{categoria}'"; using (var c = C()) { c.Open(); using (var r = new MySqlCommand(q, c).ExecuteReader()) while (r.Read()) l.Add(new Especialista { IdEspecialista = r.GetInt32(0), Nombre = r.GetString(1) }); } return l; }
        public bool RegistrarConsumoServicio(int mat, int serv, int esp, DateTime f, string obs) { using (var c = C()) { try { c.Open(); decimal costo = Convert.ToDecimal(new MySqlCommand($"SELECT CostoBase FROM SERVICIO WHERE IdServicio={serv}", c).ExecuteScalar()); var cmd = new MySqlCommand("INSERT INTO CONSUMO_SERVICIO (Matricula_Niño, IdServicio, IdEspecialista, Fecha, Observacion, CostoAplicado, Estado) VALUES (@m, @s, @e, @f, @o, @c, 'PENDIENTE')", c); cmd.Parameters.AddWithValue("@m", mat); cmd.Parameters.AddWithValue("@s", serv); cmd.Parameters.AddWithValue("@e", esp); cmd.Parameters.AddWithValue("@f", f); cmd.Parameters.AddWithValue("@o", obs); cmd.Parameters.AddWithValue("@c", costo); cmd.ExecuteNonQuery(); return true; } catch (Exception ex) { MessageBox.Show("Error SQL: " + ex.Message); return false; } } }
        public decimal CalcularDeudaServicios(int mat) { using (var c = C()) { c.Open(); var obj = new MySqlCommand($"SELECT IFNULL(SUM(CostoAplicado),0) FROM CONSUMO_SERVICIO WHERE Matricula_Niño={mat} AND Estado='PENDIENTE'", c).ExecuteScalar(); return Convert.ToDecimal(obj); } }

        public List<Producto> ObtenerProductos() { var l = new List<Producto>(); using (var c = C()) { c.Open(); using (var r = new MySqlCommand("SELECT * FROM PRODUCTO", c).ExecuteReader()) while (r.Read()) l.Add(new Producto { IdProducto = r.GetInt32(0), Nombre = r.GetString(1), StockActual = r.GetInt32(2), StockMinimo = r.GetInt32(3), PrecioUnitario = r.GetDecimal(4) }); } return l; }
        public bool GuardarProducto(string n, int sa, int sm, decimal pu) { using (var c = C()) { try { c.Open(); var cmd = new MySqlCommand("INSERT INTO PRODUCTO (Nombre, StockActual, StockMinimo, PrecioUnitario) VALUES (@n, @s, @sm, @p)", c); cmd.Parameters.AddWithValue("@n", n); cmd.Parameters.AddWithValue("@s", sa); cmd.Parameters.AddWithValue("@sm", sm); cmd.Parameters.AddWithValue("@p", pu); cmd.ExecuteNonQuery(); return true; } catch { return false; } } }
        public bool AumentarStock(int idProd, int cantidad) { using (var c = C()) { try { c.Open(); new MySqlCommand($"UPDATE PRODUCTO SET StockActual = StockActual + {cantidad} WHERE IdProducto={idProd}", c).ExecuteNonQuery(); return true; } catch { return false; } } }

        public bool RegistrarConsumoTienda(int mat, int prod)
        {
            using (var c = C())
            {
                c.Open(); var t = c.BeginTransaction();
                try
                {
                    int stock = Convert.ToInt32(new MySqlCommand($"SELECT StockActual FROM PRODUCTO WHERE IdProducto={prod}", c, t).ExecuteScalar());
                    if (stock <= 0) throw new Exception("Sin Stock");
                    new MySqlCommand($"UPDATE PRODUCTO SET StockActual = StockActual - 1 WHERE IdProducto={prod}", c, t).ExecuteNonQuery();
                    decimal precio = Convert.ToDecimal(new MySqlCommand($"SELECT PrecioUnitario FROM PRODUCTO WHERE IdProducto={prod}", c, t).ExecuteScalar());
                    var cmd = new MySqlCommand("INSERT INTO CONSUMO_TIENDA (Matricula_Niño, IdProducto, Fecha, CostoAplicado, Estado) VALUES (@m, @p, CURDATE(), @c, 'PENDIENTE')", c, t);
                    cmd.Parameters.AddWithValue("@m", mat); cmd.Parameters.AddWithValue("@p", prod); cmd.Parameters.AddWithValue("@c", precio);
                    cmd.ExecuteNonQuery();
                    t.Commit(); return true;
                }
                catch { t.Rollback(); return false; }
            }
        }
        public decimal CalcularDeudaTienda(int mat) { using (var c = C()) { c.Open(); var obj = new MySqlCommand($"SELECT IFNULL(SUM(CostoAplicado),0) FROM CONSUMO_TIENDA WHERE Matricula_Niño={mat} AND Estado='PENDIENTE'", c).ExecuteScalar(); return Convert.ToDecimal(obj); } }

        public bool RegistrarAlergia(int mat, string ing) { using (var c = C()) { try { c.Open(); var cmd = new MySqlCommand("INSERT IGNORE INTO INGREDIENTE VALUES (@i)", c); cmd.Parameters.AddWithValue("@i", ing); cmd.ExecuteNonQuery(); var cmd2 = new MySqlCommand("INSERT INTO ALERGIA VALUES (@m, @i)", c); cmd2.Parameters.AddWithValue("@m", mat); cmd2.Parameters.AddWithValue("@i", ing); cmd2.ExecuteNonQuery(); return true; } catch { return false; } } }
        public bool GuardarMenuCompleto(string nombre, decimal costo, string ingredientesStr) { using (var c = C()) { c.Open(); var t = c.BeginTransaction(); try { var cmdMenu = new MySqlCommand("INSERT INTO MENU (Nombre, CostoBaseComida) VALUES (@n, @c)", c, t); cmdMenu.Parameters.AddWithValue("@n", nombre); cmdMenu.Parameters.AddWithValue("@c", costo); cmdMenu.ExecuteNonQuery(); long idMenu = cmdMenu.LastInsertedId; foreach (var ing in ingredientesStr.Split(',')) { string ni = ing.Trim(); if (string.IsNullOrEmpty(ni)) continue; var cmdI = new MySqlCommand("INSERT IGNORE INTO INGREDIENTE (NombreIngrediente) VALUES (@ni)", c, t); cmdI.Parameters.AddWithValue("@ni", ni); cmdI.ExecuteNonQuery(); var cmdR = new MySqlCommand("INSERT INTO MENU_INGREDIENTE (NumMenu, Nombre_Ingrediente) VALUES (@m, @i)", c, t); cmdR.Parameters.AddWithValue("@m", idMenu); cmdR.Parameters.AddWithValue("@i", ni); cmdR.ExecuteNonQuery(); } t.Commit(); return true; } catch { t.Rollback(); return false; } } }
        public string VerificarAlergia(int idNiño, int idMenu) { using (var c = C()) { c.Open(); string sql = @"SELECT i.NombreIngrediente FROM MENU_INGREDIENTE mi JOIN ALERGIA a ON mi.Nombre_Ingrediente = a.Nombre_Ingrediente JOIN INGREDIENTE i ON mi.Nombre_Ingrediente = i.NombreIngrediente WHERE a.Matricula_Niño = @n AND mi.NumMenu = @m LIMIT 1"; using (var cmd = new MySqlCommand(sql, c)) { cmd.Parameters.AddWithValue("@n", idNiño); cmd.Parameters.AddWithValue("@m", idMenu); return cmd.ExecuteScalar()?.ToString(); } } }

        // --- REPORTE NOMINA (VISTA SQL) ---
        public List<ReporteNomina> ObtenerReporteNomina(int mes)
        {
            var l = new List<ReporteNomina>();
            using (var c = C())
            {
                c.Open();
                string sql = "SELECT * FROM V_REPORTE_NOMINA WHERE Mes = @m";
                var cmd = new MySqlCommand(sql, c); cmd.Parameters.AddWithValue("@m", mes);
                using (var r = cmd.ExecuteReader()) while (r.Read()) l.Add(new ReporteNomina { Especialista = r.GetString("Especialista"), Especialidad = r.GetString("Especialidad"), Atenciones = r.GetInt32("CantidadAtenciones"), TotalGenerado = r.GetDecimal("TotalGenerado") });
            }
            return l;
        }

        // --- REPORTE REPOSICIÓN (CURSOR SQL) ---
        public List<ReporteReposicion> GenerarReporteReposicionCursor()
        {
            var l = new List<ReporteReposicion>();
            using (var c = C())
            {
                c.Open();
                var cmd = new MySqlCommand("SP_GENERAR_PEDIDO_REPOSICION", c);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                using (var r = cmd.ExecuteReader()) while (r.Read()) l.Add(new ReporteReposicion { Producto = r.GetString(0), StockActual = r.GetInt32(1), Faltante = r.GetInt32(2) });
            }
            return l;
        }

        // --- PAGO BLINDADO (STORED PROCEDURE) ---
        public bool RegistrarPagoTotal(int mat, decimal total)
        {
            using (var c = C())
            {
                try
                {
                    c.Open();
                    var cmd = new MySqlCommand("SP_REGISTRAR_PAGO", c);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_Matricula", mat);
                    cmd.Parameters.AddWithValue("p_Total", total);
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Error SP: " + ex.Message); return false; }
            }
        }
    }
}
