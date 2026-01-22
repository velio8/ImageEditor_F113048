// ============================================================================
// РЕДАКТОР НА ИЗОБРАЖЕНИЯ (Image Editor)
// Автор: Велислав Кочев
// Факултетен номер: F113048
// Описание: Windows Forms приложение за обработка на изображения
//           с многоезичен интерфейс (Български / English)
// ============================================================================

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ImageEditor
{
    /// <summary>
    /// Главна форма на приложението за редактиране на изображения
    /// </summary>
    public partial class Form1 : Form
    {
        // ====================================================================
        // ПОЛЕТА
        // ====================================================================

        /// <summary>
        /// Оригиналното изображение (запазва се за възстановяване)
        /// </summary>
        private Bitmap originalImage;

        /// <summary>
        /// Текущото редактирано изображение
        /// </summary>
        private Bitmap currentImage;

        /// <summary>
        /// Флаг за текущия език (true = български, false = английски)
        /// </summary>
        private bool isLanguageBulgarian = true;

        /// <summary>
        /// Текущ ъгъл на завъртане
        /// </summary>
        private float currentRotation = 0;

        // ====================================================================
        // КОНТРОЛИ
        // ====================================================================

        // Меню
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem editMenu;
        private ToolStripMenuItem effectsMenu;
        private ToolStripMenuItem languageMenu;
        private ToolStripMenuItem helpMenu;

        // Toolbar
        private ToolStrip toolStrip;
        private ToolStripButton btnOpen;
        private ToolStripButton btnSave;
        private ToolStripButton btnUndo;
        private ToolStripSeparator toolSep1;
        private ToolStripButton btnRotateLeft;
        private ToolStripButton btnRotateRight;
        private ToolStripButton btnFlipH;
        private ToolStripButton btnFlipV;

        // Основен панел с изображение
        private PictureBox pictureBox;
        private Panel imagePanel;

        // Панел с контроли за ефекти
        private Panel controlPanel;
        private Label lblBrightness;
        private TrackBar trackBrightness;
        private Label lblBrightnessValue;
        private Label lblContrast;
        private TrackBar trackContrast;
        private Label lblContrastValue;
        private Label lblSaturation;
        private TrackBar trackSaturation;
        private Label lblSaturationValue;

        // Бутони за ефекти
        private Button btnGrayscale;
        private Button btnSepia;
        private Button btnInvert;
        private Button btnBlur;
        private Button btnSharpen;
        private Button btnReset;
        private Button btnApply;

        // Статус бар
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripStatusLabel imageSizeLabel;

        // ====================================================================
        // КОНСТРУКТОР
        // ====================================================================

        /// <summary>
        /// Конструктор - инициализира формата и всички компоненти
        /// </summary>
        public Form1()
        {
            // Основни свойства на формата
            this.Text = "Редактор на изображения";
            this.Size = new Size(1100, 750);
            this.MinimumSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);

            // Инициализация на компонентите
            InitializeMenu();
            InitializeToolbar();
            InitializeImagePanel();
            InitializeControlPanel();
            InitializeStatusBar();

            // Начално състояние - деактивирани контроли
            SetControlsEnabled(false);
        }

        // ====================================================================
        // ИНИЦИАЛИЗАЦИЯ НА МЕНЮТО
        // ====================================================================

        /// <summary>
        /// Създава главното меню на приложението
        /// </summary>
        private void InitializeMenu()
        {
            menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.FromArgb(60, 60, 65);
            menuStrip.ForeColor = Color.White;

            // === Меню "Файл" ===
            fileMenu = new ToolStripMenuItem("Файл");
            fileMenu.ForeColor = Color.White;

            var openItem = new ToolStripMenuItem("Отвори изображение...", null, (s, e) => OpenImage());
            openItem.ShortcutKeys = Keys.Control | Keys.O;

            var saveItem = new ToolStripMenuItem("Запази като...", null, (s, e) => SaveImage());
            saveItem.ShortcutKeys = Keys.Control | Keys.S;

            var exitItem = new ToolStripMenuItem("Изход", null, (s, e) => Application.Exit());
            exitItem.ShortcutKeys = Keys.Alt | Keys.F4;

            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { 
                openItem, saveItem, new ToolStripSeparator(), exitItem 
            });

            // === Меню "Редактиране" ===
            editMenu = new ToolStripMenuItem("Редактиране");
            editMenu.ForeColor = Color.White;

            var undoItem = new ToolStripMenuItem("Върни оригинала", null, (s, e) => ResetToOriginal());
            undoItem.ShortcutKeys = Keys.Control | Keys.Z;

            var rotateLeftItem = new ToolStripMenuItem("Завърти наляво (90°)", null, (s, e) => RotateImage(-90));
            rotateLeftItem.ShortcutKeys = Keys.Control | Keys.Left;

            var rotateRightItem = new ToolStripMenuItem("Завърти надясно (90°)", null, (s, e) => RotateImage(90));
            rotateRightItem.ShortcutKeys = Keys.Control | Keys.Right;

            var flipHItem = new ToolStripMenuItem("Обърни хоризонтално", null, (s, e) => FlipImage(true));
            flipHItem.ShortcutKeys = Keys.Control | Keys.H;

            var flipVItem = new ToolStripMenuItem("Обърни вертикално", null, (s, e) => FlipImage(false));
            flipVItem.ShortcutKeys = Keys.Control | Keys.J;

            editMenu.DropDownItems.AddRange(new ToolStripItem[] { 
                undoItem, new ToolStripSeparator(), 
                rotateLeftItem, rotateRightItem, new ToolStripSeparator(),
                flipHItem, flipVItem 
            });

            // === Меню "Ефекти" ===
            effectsMenu = new ToolStripMenuItem("Ефекти");
            effectsMenu.ForeColor = Color.White;

            var grayscaleItem = new ToolStripMenuItem("Черно-бяло", null, (s, e) => ApplyGrayscale());
            var sepiaItem = new ToolStripMenuItem("Сепия", null, (s, e) => ApplySepia());
            var invertItem = new ToolStripMenuItem("Инвертиране", null, (s, e) => ApplyInvert());
            var blurItem = new ToolStripMenuItem("Размазване", null, (s, e) => ApplyBlur());
            var sharpenItem = new ToolStripMenuItem("Изостряне", null, (s, e) => ApplySharpen());

            effectsMenu.DropDownItems.AddRange(new ToolStripItem[] { 
                grayscaleItem, sepiaItem, invertItem, new ToolStripSeparator(),
                blurItem, sharpenItem 
            });

            // === Меню "Език" ===
            languageMenu = new ToolStripMenuItem("Език / Language");
            languageMenu.ForeColor = Color.White;

            var bgItem = new ToolStripMenuItem("🇧🇬 Български", null, (s, e) => ChangeLanguage(true));
            bgItem.Checked = true;
            var enItem = new ToolStripMenuItem("🇬🇧 English", null, (s, e) => ChangeLanguage(false));

            languageMenu.DropDownItems.AddRange(new ToolStripItem[] { bgItem, enItem });

            // === Меню "Помощ" ===
            helpMenu = new ToolStripMenuItem("Помощ");
            helpMenu.ForeColor = Color.White;

            var aboutItem = new ToolStripMenuItem("За програмата", null, (s, e) => ShowAbout());
            aboutItem.ShortcutKeys = Keys.F1;

            helpMenu.DropDownItems.Add(aboutItem);

            // Добавяне към менюто
            menuStrip.Items.AddRange(new ToolStripItem[] { 
                fileMenu, editMenu, effectsMenu, languageMenu, helpMenu 
            });

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        // ====================================================================
        // ИНИЦИАЛИЗАЦИЯ НА TOOLBAR
        // ====================================================================

        /// <summary>
        /// Създава лентата с инструменти
        /// </summary>
        private void InitializeToolbar()
        {
            toolStrip = new ToolStrip();
            toolStrip.BackColor = Color.FromArgb(60, 60, 65);
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Padding = new Padding(5, 0, 5, 0);

            // Бутон "Отвори"
            btnOpen = new ToolStripButton("📂 Отвори");
            btnOpen.ForeColor = Color.White;
            btnOpen.Click += (s, e) => OpenImage();

            // Бутон "Запази"
            btnSave = new ToolStripButton("💾 Запази");
            btnSave.ForeColor = Color.White;
            btnSave.Click += (s, e) => SaveImage();

            // Бутон "Върни"
            btnUndo = new ToolStripButton("↩ Върни");
            btnUndo.ForeColor = Color.White;
            btnUndo.Click += (s, e) => ResetToOriginal();

            toolSep1 = new ToolStripSeparator();

            // Бутон "Завърти наляво"
            btnRotateLeft = new ToolStripButton("⟲ 90°");
            btnRotateLeft.ForeColor = Color.White;
            btnRotateLeft.ToolTipText = "Завърти наляво";
            btnRotateLeft.Click += (s, e) => RotateImage(-90);

            // Бутон "Завърти надясно"
            btnRotateRight = new ToolStripButton("⟳ 90°");
            btnRotateRight.ForeColor = Color.White;
            btnRotateRight.ToolTipText = "Завърти надясно";
            btnRotateRight.Click += (s, e) => RotateImage(90);

            // Бутон "Обърни хоризонтално"
            btnFlipH = new ToolStripButton("↔ Обърни");
            btnFlipH.ForeColor = Color.White;
            btnFlipH.ToolTipText = "Обърни хоризонтално";
            btnFlipH.Click += (s, e) => FlipImage(true);

            // Бутон "Обърни вертикално"
            btnFlipV = new ToolStripButton("↕ Обърни");
            btnFlipV.ForeColor = Color.White;
            btnFlipV.ToolTipText = "Обърни вертикално";
            btnFlipV.Click += (s, e) => FlipImage(false);

            // Добавяне към toolbar
            toolStrip.Items.AddRange(new ToolStripItem[] { 
                btnOpen, btnSave, btnUndo, toolSep1,
                btnRotateLeft, btnRotateRight, btnFlipH, btnFlipV 
            });

            this.Controls.Add(toolStrip);
        }

        // ====================================================================
        // ИНИЦИАЛИЗАЦИЯ НА ПАНЕЛА С ИЗОБРАЖЕНИЕ
        // ====================================================================

        /// <summary>
        /// Създава панела за показване на изображението
        /// </summary>
        private void InitializeImagePanel()
        {
            // Панел-контейнер със скролбарове
            imagePanel = new Panel();
            imagePanel.Location = new Point(0, 52);
            imagePanel.Size = new Size(this.ClientSize.Width - 250, this.ClientSize.Height - 80);
            imagePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            imagePanel.BackColor = Color.FromArgb(30, 30, 30);
            imagePanel.AutoScroll = true;
            imagePanel.BorderStyle = BorderStyle.FixedSingle;

            // PictureBox за изображението
            pictureBox = new PictureBox();
            pictureBox.Location = new Point(0, 0);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BackColor = Color.FromArgb(30, 30, 30);
            pictureBox.Dock = DockStyle.Fill;

            // Drag & Drop поддръжка
            pictureBox.AllowDrop = true;
            pictureBox.DragEnter += PictureBox_DragEnter;
            pictureBox.DragDrop += PictureBox_DragDrop;

            imagePanel.Controls.Add(pictureBox);
            this.Controls.Add(imagePanel);
        }

        // ====================================================================
        // ИНИЦИАЛИЗАЦИЯ НА ПАНЕЛА С КОНТРОЛИ
        // ====================================================================

        /// <summary>
        /// Създава страничния панел с контроли за ефекти
        /// </summary>
        private void InitializeControlPanel()
        {
            // Панел отдясно
            controlPanel = new Panel();
            controlPanel.Location = new Point(this.ClientSize.Width - 245, 52);
            controlPanel.Size = new Size(240, this.ClientSize.Height - 80);
            controlPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            controlPanel.BackColor = Color.FromArgb(37, 37, 38);
            controlPanel.BorderStyle = BorderStyle.FixedSingle;
            controlPanel.Padding = new Padding(10);

            int yPos = 15;

            // === Заглавие ===
            Label lblTitle = new Label();
            lblTitle.Text = "⚙ Настройки";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(10, yPos);
            lblTitle.Size = new Size(200, 25);
            controlPanel.Controls.Add(lblTitle);
            yPos += 40;

            // === Яркост ===
            lblBrightness = new Label();
            lblBrightness.Text = "☀ Яркост:";
            lblBrightness.ForeColor = Color.LightGray;
            lblBrightness.Location = new Point(10, yPos);
            lblBrightness.Size = new Size(120, 20);
            controlPanel.Controls.Add(lblBrightness);

            lblBrightnessValue = new Label();
            lblBrightnessValue.Text = "0";
            lblBrightnessValue.ForeColor = Color.Yellow;
            lblBrightnessValue.Location = new Point(180, yPos);
            lblBrightnessValue.Size = new Size(40, 20);
            lblBrightnessValue.TextAlign = ContentAlignment.MiddleRight;
            controlPanel.Controls.Add(lblBrightnessValue);
            yPos += 25;

            trackBrightness = new TrackBar();
            trackBrightness.Location = new Point(10, yPos);
            trackBrightness.Size = new Size(210, 45);
            trackBrightness.Minimum = -100;
            trackBrightness.Maximum = 100;
            trackBrightness.Value = 0;
            trackBrightness.TickFrequency = 20;
            trackBrightness.BackColor = Color.FromArgb(37, 37, 38);
            trackBrightness.Scroll += (s, e) => {
                lblBrightnessValue.Text = trackBrightness.Value.ToString();
            };
            controlPanel.Controls.Add(trackBrightness);
            yPos += 55;

            // === Контраст ===
            lblContrast = new Label();
            lblContrast.Text = "◐ Контраст:";
            lblContrast.ForeColor = Color.LightGray;
            lblContrast.Location = new Point(10, yPos);
            lblContrast.Size = new Size(120, 20);
            controlPanel.Controls.Add(lblContrast);

            lblContrastValue = new Label();
            lblContrastValue.Text = "0";
            lblContrastValue.ForeColor = Color.Yellow;
            lblContrastValue.Location = new Point(180, yPos);
            lblContrastValue.Size = new Size(40, 20);
            lblContrastValue.TextAlign = ContentAlignment.MiddleRight;
            controlPanel.Controls.Add(lblContrastValue);
            yPos += 25;

            trackContrast = new TrackBar();
            trackContrast.Location = new Point(10, yPos);
            trackContrast.Size = new Size(210, 45);
            trackContrast.Minimum = -100;
            trackContrast.Maximum = 100;
            trackContrast.Value = 0;
            trackContrast.TickFrequency = 20;
            trackContrast.BackColor = Color.FromArgb(37, 37, 38);
            trackContrast.Scroll += (s, e) => {
                lblContrastValue.Text = trackContrast.Value.ToString();
            };
            controlPanel.Controls.Add(trackContrast);
            yPos += 55;

            // === Наситеност ===
            lblSaturation = new Label();
            lblSaturation.Text = "🎨 Наситеност:";
            lblSaturation.ForeColor = Color.LightGray;
            lblSaturation.Location = new Point(10, yPos);
            lblSaturation.Size = new Size(120, 20);
            controlPanel.Controls.Add(lblSaturation);

            lblSaturationValue = new Label();
            lblSaturationValue.Text = "0";
            lblSaturationValue.ForeColor = Color.Yellow;
            lblSaturationValue.Location = new Point(180, yPos);
            lblSaturationValue.Size = new Size(40, 20);
            lblSaturationValue.TextAlign = ContentAlignment.MiddleRight;
            controlPanel.Controls.Add(lblSaturationValue);
            yPos += 25;

            trackSaturation = new TrackBar();
            trackSaturation.Location = new Point(10, yPos);
            trackSaturation.Size = new Size(210, 45);
            trackSaturation.Minimum = -100;
            trackSaturation.Maximum = 100;
            trackSaturation.Value = 0;
            trackSaturation.TickFrequency = 20;
            trackSaturation.BackColor = Color.FromArgb(37, 37, 38);
            trackSaturation.Scroll += (s, e) => {
                lblSaturationValue.Text = trackSaturation.Value.ToString();
            };
            controlPanel.Controls.Add(trackSaturation);
            yPos += 60;

            // === Бутон "Приложи" ===
            btnApply = new Button();
            btnApply.Text = "✓ Приложи";
            btnApply.Location = new Point(10, yPos);
            btnApply.Size = new Size(210, 35);
            btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.BackColor = Color.FromArgb(0, 122, 204);
            btnApply.ForeColor = Color.White;
            btnApply.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnApply.Cursor = Cursors.Hand;
            btnApply.Click += (s, e) => ApplyAdjustments();
            controlPanel.Controls.Add(btnApply);
            yPos += 50;

            // === Разделител ===
            Label separator = new Label();
            separator.BorderStyle = BorderStyle.Fixed3D;
            separator.Location = new Point(10, yPos);
            separator.Size = new Size(210, 2);
            controlPanel.Controls.Add(separator);
            yPos += 15;

            // === Заглавие за ефекти ===
            Label lblEffects = new Label();
            lblEffects.Text = "🎭 Ефекти";
            lblEffects.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblEffects.ForeColor = Color.White;
            lblEffects.Location = new Point(10, yPos);
            lblEffects.Size = new Size(200, 25);
            controlPanel.Controls.Add(lblEffects);
            yPos += 35;

            // === Бутони за ефекти ===
            btnGrayscale = CreateEffectButton("Черно-бяло", yPos);
            btnGrayscale.Click += (s, e) => ApplyGrayscale();
            controlPanel.Controls.Add(btnGrayscale);
            yPos += 40;

            btnSepia = CreateEffectButton("Сепия", yPos);
            btnSepia.Click += (s, e) => ApplySepia();
            controlPanel.Controls.Add(btnSepia);
            yPos += 40;

            btnInvert = CreateEffectButton("Инвертиране", yPos);
            btnInvert.Click += (s, e) => ApplyInvert();
            controlPanel.Controls.Add(btnInvert);
            yPos += 40;

            btnBlur = CreateEffectButton("Размазване", yPos);
            btnBlur.Click += (s, e) => ApplyBlur();
            controlPanel.Controls.Add(btnBlur);
            yPos += 40;

            btnSharpen = CreateEffectButton("Изостряне", yPos);
            btnSharpen.Click += (s, e) => ApplySharpen();
            controlPanel.Controls.Add(btnSharpen);
            yPos += 50;

            // === Бутон "Нулиране" ===
            btnReset = new Button();
            btnReset.Text = "↺ Върни оригинала";
            btnReset.Location = new Point(10, yPos);
            btnReset.Size = new Size(210, 35);
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.BackColor = Color.FromArgb(200, 80, 80);
            btnReset.ForeColor = Color.White;
            btnReset.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnReset.Cursor = Cursors.Hand;
            btnReset.Click += (s, e) => ResetToOriginal();
            controlPanel.Controls.Add(btnReset);

            this.Controls.Add(controlPanel);
        }

        /// <summary>
        /// Помощен метод за създаване на бутон за ефект
        /// </summary>
        private Button CreateEffectButton(string text, int yPos)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(10, yPos);
            btn.Size = new Size(210, 32);
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.FromArgb(55, 55, 58);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9);
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
            return btn;
        }

        // ====================================================================
        // ИНИЦИАЛИЗАЦИЯ НА СТАТУС БАР
        // ====================================================================

        /// <summary>
        /// Създава статус лентата в долната част
        /// </summary>
        private void InitializeStatusBar()
        {
            statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.FromArgb(0, 122, 204);

            statusLabel = new ToolStripStatusLabel();
            statusLabel.Text = "Готов. Отворете изображение или го пуснете тук.";
            statusLabel.ForeColor = Color.White;
            statusLabel.Spring = true;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            imageSizeLabel = new ToolStripStatusLabel();
            imageSizeLabel.Text = "";
            imageSizeLabel.ForeColor = Color.White;

            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, imageSizeLabel });
            this.Controls.Add(statusStrip);
        }

        // ====================================================================
        // МЕТОДИ ЗА РАБОТА С ФАЙЛОВЕ
        // ====================================================================

        /// <summary>
        /// Отваря диалог за избор на изображение
        /// </summary>
        private void OpenImage()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = isLanguageBulgarian ? "Избери изображение" : "Select Image";
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff|All Files|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadImage(dialog.FileName);
                }
            }
        }

        /// <summary>
        /// Зарежда изображение от файл
        /// </summary>
        private void LoadImage(string path)
        {
            try
            {
                // Освобождаване на старите изображения
                originalImage?.Dispose();
                currentImage?.Dispose();

                // Зареждане на новото изображение
                originalImage = new Bitmap(path);
                currentImage = new Bitmap(originalImage);

                // Показване
                pictureBox.Image = currentImage;

                // Нулиране на контролите
                ResetSliders();
                currentRotation = 0;

                // Активиране на контролите
                SetControlsEnabled(true);

                // Актуализиране на статуса
                string fileName = System.IO.Path.GetFileName(path);
                statusLabel.Text = isLanguageBulgarian 
                    ? $"Заредено: {fileName}" 
                    : $"Loaded: {fileName}";
                imageSizeLabel.Text = $"{originalImage.Width} x {originalImage.Height} px";
            }
            catch (Exception ex)
            {
                string errorMsg = isLanguageBulgarian
                    ? $"Грешка при зареждане: {ex.Message}"
                    : $"Error loading: {ex.Message}";
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Запазва редактираното изображение
        /// </summary>
        private void SaveImage()
        {
            if (currentImage == null) return;

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = isLanguageBulgarian ? "Запази изображение" : "Save Image";
                dialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap|*.bmp";
                dialog.DefaultExt = "png";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ImageFormat format = ImageFormat.Png;
                        if (dialog.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                            format = ImageFormat.Jpeg;
                        else if (dialog.FileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                            format = ImageFormat.Bmp;

                        currentImage.Save(dialog.FileName, format);

                        statusLabel.Text = isLanguageBulgarian
                            ? $"Запазено: {System.IO.Path.GetFileName(dialog.FileName)}"
                            : $"Saved: {System.IO.Path.GetFileName(dialog.FileName)}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ====================================================================
        // DRAG & DROP
        // ====================================================================

        /// <summary>
        /// Обработва влачене на файл върху формата
        /// </summary>
        private void PictureBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// Обработва пускане на файл
        /// </summary>
        private void PictureBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                LoadImage(files[0]);
            }
        }

        // ====================================================================
        // ТРАНСФОРМАЦИИ
        // ====================================================================

        /// <summary>
        /// Завърта изображението на зададен ъгъл
        /// </summary>
        private void RotateImage(float angle)
        {
            if (currentImage == null) return;

            currentRotation += angle;

            // Създаване на ново завъртяно изображение
            Bitmap rotated = new Bitmap(currentImage.Width, currentImage.Height);
            using (Graphics g = Graphics.FromImage(rotated))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TranslateTransform(currentImage.Width / 2f, currentImage.Height / 2f);
                g.RotateTransform(angle);
                g.TranslateTransform(-currentImage.Width / 2f, -currentImage.Height / 2f);
                g.DrawImage(currentImage, 0, 0);
            }

            currentImage.Dispose();
            currentImage = rotated;
            pictureBox.Image = currentImage;

            statusLabel.Text = isLanguageBulgarian
                ? $"Завъртяно на {angle}°"
                : $"Rotated {angle}°";
        }

        /// <summary>
        /// Обръща изображението хоризонтално или вертикално
        /// </summary>
        private void FlipImage(bool horizontal)
        {
            if (currentImage == null) return;

            if (horizontal)
                currentImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            else
                currentImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            pictureBox.Image = currentImage;

            statusLabel.Text = isLanguageBulgarian
                ? (horizontal ? "Обърнато хоризонтално" : "Обърнато вертикално")
                : (horizontal ? "Flipped horizontally" : "Flipped vertically");
        }

        // ====================================================================
        // НАСТРОЙКИ (ЯРКОСТ, КОНТРАСТ, НАСИТЕНОСТ)
        // ====================================================================

        /// <summary>
        /// Прилага настройките за яркост, контраст и наситеност
        /// </summary>
        private void ApplyAdjustments()
        {
            if (originalImage == null) return;

            // Стойности от плъзгачите
            float brightness = trackBrightness.Value / 100f;
            float contrast = (trackContrast.Value + 100) / 100f;
            float saturation = (trackSaturation.Value + 100) / 100f;

            // Създаване на ново изображение от оригинала
            Bitmap adjusted = new Bitmap(originalImage.Width, originalImage.Height);

            // Обработка пиксел по пиксел
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixel = originalImage.GetPixel(x, y);

                    // Яркост
                    int r = (int)(pixel.R + brightness * 255);
                    int g = (int)(pixel.G + brightness * 255);
                    int b = (int)(pixel.B + brightness * 255);

                    // Контраст
                    r = (int)(((r / 255f - 0.5f) * contrast + 0.5f) * 255);
                    g = (int)(((g / 255f - 0.5f) * contrast + 0.5f) * 255);
                    b = (int)(((b / 255f - 0.5f) * contrast + 0.5f) * 255);

                    // Наситеност
                    float gray = 0.299f * r + 0.587f * g + 0.114f * b;
                    r = (int)(gray + (r - gray) * saturation);
                    g = (int)(gray + (g - gray) * saturation);
                    b = (int)(gray + (b - gray) * saturation);

                    // Ограничаване в диапазона 0-255
                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    adjusted.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
                }
            }

            currentImage?.Dispose();
            currentImage = adjusted;
            pictureBox.Image = currentImage;

            statusLabel.Text = isLanguageBulgarian
                ? "Настройките са приложени"
                : "Adjustments applied";
        }

        // ====================================================================
        // ЕФЕКТИ
        // ====================================================================

        /// <summary>
        /// Прилага черно-бял ефект
        /// </summary>
        private void ApplyGrayscale()
        {
            if (currentImage == null) return;

            for (int y = 0; y < currentImage.Height; y++)
            {
                for (int x = 0; x < currentImage.Width; x++)
                {
                    Color pixel = currentImage.GetPixel(x, y);
                    int gray = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                    currentImage.SetPixel(x, y, Color.FromArgb(pixel.A, gray, gray, gray));
                }
            }

            pictureBox.Image = currentImage;
            statusLabel.Text = isLanguageBulgarian ? "Приложен ефект: Черно-бяло" : "Applied: Grayscale";
        }

        /// <summary>
        /// Прилага ефект сепия (стара снимка)
        /// </summary>
        private void ApplySepia()
        {
            if (currentImage == null) return;

            for (int y = 0; y < currentImage.Height; y++)
            {
                for (int x = 0; x < currentImage.Width; x++)
                {
                    Color pixel = currentImage.GetPixel(x, y);

                    int tr = (int)(0.393 * pixel.R + 0.769 * pixel.G + 0.189 * pixel.B);
                    int tg = (int)(0.349 * pixel.R + 0.686 * pixel.G + 0.168 * pixel.B);
                    int tb = (int)(0.272 * pixel.R + 0.534 * pixel.G + 0.131 * pixel.B);

                    tr = Math.Min(255, tr);
                    tg = Math.Min(255, tg);
                    tb = Math.Min(255, tb);

                    currentImage.SetPixel(x, y, Color.FromArgb(pixel.A, tr, tg, tb));
                }
            }

            pictureBox.Image = currentImage;
            statusLabel.Text = isLanguageBulgarian ? "Приложен ефект: Сепия" : "Applied: Sepia";
        }

        /// <summary>
        /// Инвертира цветовете на изображението
        /// </summary>
        private void ApplyInvert()
        {
            if (currentImage == null) return;

            for (int y = 0; y < currentImage.Height; y++)
            {
                for (int x = 0; x < currentImage.Width; x++)
                {
                    Color pixel = currentImage.GetPixel(x, y);
                    currentImage.SetPixel(x, y, Color.FromArgb(pixel.A, 
                        255 - pixel.R, 255 - pixel.G, 255 - pixel.B));
                }
            }

            pictureBox.Image = currentImage;
            statusLabel.Text = isLanguageBulgarian ? "Приложен ефект: Инвертиране" : "Applied: Invert";
        }

        /// <summary>
        /// Прилага ефект на размазване (blur)
        /// </summary>
        private void ApplyBlur()
        {
            if (currentImage == null) return;

            // Матрица за размазване 3x3
            Bitmap blurred = new Bitmap(currentImage.Width, currentImage.Height);

            for (int y = 1; y < currentImage.Height - 1; y++)
            {
                for (int x = 1; x < currentImage.Width - 1; x++)
                {
                    int r = 0, g = 0, b = 0;

                    // Средна стойност от съседните пиксели
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            Color pixel = currentImage.GetPixel(x + kx, y + ky);
                            r += pixel.R;
                            g += pixel.G;
                            b += pixel.B;
                        }
                    }

                    blurred.SetPixel(x, y, Color.FromArgb(r / 9, g / 9, b / 9));
                }
            }

            currentImage.Dispose();
            currentImage = blurred;
            pictureBox.Image = currentImage;
            statusLabel.Text = isLanguageBulgarian ? "Приложен ефект: Размазване" : "Applied: Blur";
        }

        /// <summary>
        /// Прилага ефект на изостряне (sharpen)
        /// </summary>
        private void ApplySharpen()
        {
            if (currentImage == null) return;

            // Матрица за изостряне
            int[,] kernel = {
                { 0, -1, 0 },
                { -1, 5, -1 },
                { 0, -1, 0 }
            };

            Bitmap sharpened = new Bitmap(currentImage.Width, currentImage.Height);

            for (int y = 1; y < currentImage.Height - 1; y++)
            {
                for (int x = 1; x < currentImage.Width - 1; x++)
                {
                    int r = 0, g = 0, b = 0;

                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            Color pixel = currentImage.GetPixel(x + kx, y + ky);
                            int k = kernel[ky + 1, kx + 1];
                            r += pixel.R * k;
                            g += pixel.G * k;
                            b += pixel.B * k;
                        }
                    }

                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));

                    sharpened.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            currentImage.Dispose();
            currentImage = sharpened;
            pictureBox.Image = currentImage;
            statusLabel.Text = isLanguageBulgarian ? "Приложен ефект: Изостряне" : "Applied: Sharpen";
        }

        // ====================================================================
        // ПОМОЩНИ МЕТОДИ
        // ====================================================================

        /// <summary>
        /// Връща изображението към оригинала
        /// </summary>
        private void ResetToOriginal()
        {
            if (originalImage == null) return;

            currentImage?.Dispose();
            currentImage = new Bitmap(originalImage);
            pictureBox.Image = currentImage;

            ResetSliders();
            currentRotation = 0;

            statusLabel.Text = isLanguageBulgarian
                ? "Върнато към оригинала"
                : "Reset to original";
        }

        /// <summary>
        /// Нулира плъзгачите
        /// </summary>
        private void ResetSliders()
        {
            trackBrightness.Value = 0;
            trackContrast.Value = 0;
            trackSaturation.Value = 0;
            lblBrightnessValue.Text = "0";
            lblContrastValue.Text = "0";
            lblSaturationValue.Text = "0";
        }

        /// <summary>
        /// Активира или деактивира контролите
        /// </summary>
        private void SetControlsEnabled(bool enabled)
        {
            trackBrightness.Enabled = enabled;
            trackContrast.Enabled = enabled;
            trackSaturation.Enabled = enabled;
            btnApply.Enabled = enabled;
            btnGrayscale.Enabled = enabled;
            btnSepia.Enabled = enabled;
            btnInvert.Enabled = enabled;
            btnBlur.Enabled = enabled;
            btnSharpen.Enabled = enabled;
            btnReset.Enabled = enabled;
            btnSave.Enabled = enabled;
            btnUndo.Enabled = enabled;
            btnRotateLeft.Enabled = enabled;
            btnRotateRight.Enabled = enabled;
            btnFlipH.Enabled = enabled;
            btnFlipV.Enabled = enabled;
        }

        // ====================================================================
        // МНОГОЕЗИЧЕН ИНТЕРФЕЙС
        // ====================================================================

        /// <summary>
        /// Сменя езика на интерфейса
        /// </summary>
        private void ChangeLanguage(bool toBulgarian)
        {
            isLanguageBulgarian = toBulgarian;

            if (toBulgarian)
            {
                // === Български ===
                this.Text = "Редактор на изображения";

                // Меню
                fileMenu.Text = "Файл";
                ((ToolStripMenuItem)fileMenu.DropDownItems[0]).Text = "Отвори изображение...";
                ((ToolStripMenuItem)fileMenu.DropDownItems[1]).Text = "Запази като...";
                ((ToolStripMenuItem)fileMenu.DropDownItems[3]).Text = "Изход";

                editMenu.Text = "Редактиране";
                ((ToolStripMenuItem)editMenu.DropDownItems[0]).Text = "Върни оригинала";
                ((ToolStripMenuItem)editMenu.DropDownItems[2]).Text = "Завърти наляво (90°)";
                ((ToolStripMenuItem)editMenu.DropDownItems[3]).Text = "Завърти надясно (90°)";
                ((ToolStripMenuItem)editMenu.DropDownItems[5]).Text = "Обърни хоризонтално";
                ((ToolStripMenuItem)editMenu.DropDownItems[6]).Text = "Обърни вертикално";

                effectsMenu.Text = "Ефекти";
                ((ToolStripMenuItem)effectsMenu.DropDownItems[0]).Text = "Черно-бяло";
                ((ToolStripMenuItem)effectsMenu.DropDownItems[1]).Text = "Сепия";
                ((ToolStripMenuItem)effectsMenu.DropDownItems[2]).Text = "Инвертиране";
                ((ToolStripMenuItem)effectsMenu.DropDownItems[4]).Text = "Размазване";
                ((ToolStripMenuItem)effectsMenu.DropDownItems[5]).Text = "Изостряне";

                helpMenu.Text = "Помощ";
                ((ToolStripMenuItem)helpMenu.DropDownItems[0]).Text = "За програмата";

                // Toolbar
                btnOpen.Text = "📂 Отвори";
                btnSave.Text = "💾 Запази";
                btnUndo.Text = "↩ Върни";
                btnFlipH.Text = "↔ Обърни";
                btnFlipV.Text = "↕ Обърни";

                // Контроли
                lblBrightness.Text = "☀ Яркост:";
                lblContrast.Text = "◐ Контраст:";
                lblSaturation.Text = "🎨 Наситеност:";
                btnApply.Text = "✓ Приложи";
                btnGrayscale.Text = "Черно-бяло";
                btnSepia.Text = "Сепия";
                btnInvert.Text = "Инвертиране";
                btnBlur.Text = "Размазване";
                btnSharpen.Text = "Изостряне";
                btnReset.Text = "↺ Върни оригинала";

                // Статус
                if (currentImage == null)
                    statusLabel.Text = "Готов. Отворете изображение или го пуснете тук.";

                // Меню отметки
                ((ToolStripMenuItem)languageMenu.DropDownItems[0]).Checked = true;
                ((ToolStripMenuItem)languageMenu.DropDownItems[1]).Checked = false;
            }
            else
            {
                // === English ===
                this.Text = "Image Editor";

                // Menu
                fileMenu.Text = "File";
                ((ToolStripMenuItem)fileMenu.DropDownItems[0]).Text = "Open Image...";
                ((ToolStripMenuItem)fileMenu.DropDownItems[1]).Text = "Save As...";
                ((ToolStripMenuItem)fileMenu.DropDownItems[3]).Text = "Exit";

                editMenu.Text = "Edit";
                ((ToolStripMenuItem)editMenu.DropDownItems[0]).Text = "Reset to Original";
                ((ToolStripMenuItem)editMenu.DropDownItems[2]).Text = "Rotate Left (90°)";
                ((ToolStripMenuItem)editMenu.DropDownItems[3]).Text = "Rotate Right (90°)";
                ((ToolStripMenuItem)editMenu.DropDownItems[5]).Text = "Flip Horizontal";
                ((ToolStripMenuItem)editMenu.DropDownItems[6]).Text = "Flip Vertical";

                effectsMenu.Text = "Effects";
                ((ToolStripMenuItem)effectsMenu.DropDownItems[0]).Text = "Grayscale";
                ((ToolStripMenuItem)effectsMenu.DropDownItems[1]).Text = "Sepia";
                ((ToolStripMenuItem)effectsMenu.DropDownItems[2]).Text = "Invert";
                ((ToolStripMenuItem)effectsMenu.DropDownItems[4]).Text = "Blur";
                ((ToolStripMenuItem)effectsMenu.DropDownItems[5]).Text = "Sharpen";

                helpMenu.Text = "Help";
                ((ToolStripMenuItem)helpMenu.DropDownItems[0]).Text = "About";

                // Toolbar
                btnOpen.Text = "📂 Open";
                btnSave.Text = "💾 Save";
                btnUndo.Text = "↩ Reset";
                btnFlipH.Text = "↔ Flip";
                btnFlipV.Text = "↕ Flip";

                // Controls
                lblBrightness.Text = "☀ Brightness:";
                lblContrast.Text = "◐ Contrast:";
                lblSaturation.Text = "🎨 Saturation:";
                btnApply.Text = "✓ Apply";
                btnGrayscale.Text = "Grayscale";
                btnSepia.Text = "Sepia";
                btnInvert.Text = "Invert";
                btnBlur.Text = "Blur";
                btnSharpen.Text = "Sharpen";
                btnReset.Text = "↺ Reset to Original";

                // Status
                if (currentImage == null)
                    statusLabel.Text = "Ready. Open an image or drag & drop here.";

                // Menu checks
                ((ToolStripMenuItem)languageMenu.DropDownItems[0]).Checked = false;
                ((ToolStripMenuItem)languageMenu.DropDownItems[1]).Checked = true;
            }
        }

        /// <summary>
        /// Показва информация за програмата
        /// </summary>
        private void ShowAbout()
        {
            string title = isLanguageBulgarian ? "За програмата" : "About";
            string message = isLanguageBulgarian
                ? "🖼 Редактор на изображения v1.0\n\n" +
                  "Автор: Студент\n" +
                  "Факултетен номер: XXXXX\n\n" +
                  "Функции:\n" +
                  "• Зареждане и запазване на изображения\n" +
                  "• Завъртане и обръщане\n" +
                  "• Яркост, контраст, наситеност\n" +
                  "• Ефекти: черно-бяло, сепия, инверсия\n" +
                  "• Размазване и изостряне\n" +
                  "• Многоезичен интерфейс\n\n" +
                  "© 2024"
                : "🖼 Image Editor v1.0\n\n" +
                  "Author: Student\n" +
                  "Faculty Number: XXXXX\n\n" +
                  "Features:\n" +
                  "• Load and save images\n" +
                  "• Rotate and flip\n" +
                  "• Brightness, contrast, saturation\n" +
                  "• Effects: grayscale, sepia, invert\n" +
                  "• Blur and sharpen\n" +
                  "• Multilingual interface\n\n" +
                  "© 2024";

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
