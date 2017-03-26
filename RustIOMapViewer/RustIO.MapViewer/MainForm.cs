using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace RustIO.MapViewer
{
    public partial class MainForm : Form
    {
        private ChromiumWebBrowser browser;

        public MainForm()
        {
            InitializeComponent();
            Cef.Initialize();

            browser = new ChromiumWebBrowser("http://www.google.com");
            browser.Dock = DockStyle.Fill;

            Controls.Add(browser);
        }
    }
}