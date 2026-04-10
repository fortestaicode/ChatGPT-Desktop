using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace ChatGPTApp {
    public partial class Form1 : Form {
        private WebView2 webView;

        public Form1() {
            this.Text = "ChatGPT Turbo - No Lag Mode";
            this.WindowState = FormWindowState.Maximized;
            InitWebView();
        }

        private async void InitWebView() {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            // إعدادات البيئة لتقليل الـ Lag واستغلال الرام 32GB
            var options = new CoreWebView2EnvironmentOptions("--disable-features=Translate --enable-gpu-rasterization");
            var env = await CoreWebView2Environment.CreateAsync(null, null, options);
            
            await webView.EnsureCoreWebView2Async(env);
            
            // تعطيل التدقيق الإملائي لتسريع الكتابة في المحادثات الطويلة
            webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            webView.CoreWebView2.Settings.IsGeneralAutosaveEnabled = false;
            
            webView.Source = new Uri("https://chatgpt.com");
        }
    }
}
