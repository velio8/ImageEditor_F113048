// ============================================================================
// РЕДАКТОР НА ИЗОБРАЖЕНИЯ - Designer файл
// Автор: Велислав Кочев, F113048
// ============================================================================

namespace ImageEditor
{
    partial class Form1
    {
        /// <summary>
        /// Контейнер за компоненти
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освобождава ресурсите
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Освобождаване на изображенията
                originalImage?.Dispose();
                currentImage?.Dispose();

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Инициализация на компонентите (автоматично генериран код)
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 750);
            this.Text = "Редактор на изображения";
        }

        #endregion
    }
}
