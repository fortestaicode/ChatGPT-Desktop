using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace ChatGPTApp {
    public partial class Form1 : Form {
        // إضافة علامة الاستفهام لتعريف المتغير كـ Nullable لحل خطأ التحذير
        private WebView2? webView;

        public Form1() {
            this.Text = "ChatGPT Turbo - No Lag Mode";
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(1200, 800);
            InitWebView();
        }

        private async void InitWebView() {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            try {
                // إعدادات البيئة لتقليل الـ Lag
                var options = new CoreWebView2EnvironmentOptions("--disable-features=Translate --enable-gpu-rasterization");
                var env = await CoreWebView2Environment.CreateAsync(null, null, options);
                
                await webView.EnsureCoreWebView2Async(env);
                
                // تم تصحيح الخصائص هنا:
                // ملاحظة: IsPasswordAutosaveEnabled كافية لإيقاف التداخل في الكتابة
                webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
                
                // توجيه المتصفح للموقع
                webView.Source = new Uri("https://chatgpt.com");
            }
            catch (Exception ex) {
                MessageBox.Show($"خطأ في تشغيل المحرك: {ex.Message}");
            }
        }
    }
}
