// ============================================================================
// РЕДАКТОР НА ИЗОБРАЖЕНИЯ - Входна точка
// Автор: Велислав Кочев, F113048
// Описание: Стартира Windows Forms приложението
// ============================================================================

using System;
using System.Windows.Forms;

namespace ImageEditor
{
    /// <summary>
    /// Главен клас с входната точка на приложението
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Главен метод - стартира приложението
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Активиране на визуални стилове за модерен изглед
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Стартиране на главната форма
            Application.Run(new Form1());
        }
    }
}
