using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace ChatGPTApp {
    public partial class Form1 : Form {
        private WebView2? webView;

        public Form1() {
            this.Text = "ChatGPT Turbo - Maximum Performance";
            this.WindowState = FormWindowState.Maximized;
            InitWebView();
        }

        private async void InitWebView() {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            try {
                // استخدام وسائط تشغيل لتعطيل التدقيق الإملائي والميزات غير الضرورية على مستوى المحرك
                var options = new CoreWebView2EnvironmentOptions(
                    additionalBrowserArguments: "--disable-features=Translate --enable-gpu-rasterization --disable-spell-check --ignore-gpu-blocklist"
                );
                
                var env = await CoreWebView2Environment.CreateAsync(null, null, options);
                await webView.EnsureCoreWebView2Async(env);
                
                // إعدادات لتقليل الضغط على الذاكرة
                webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
                webView.CoreWebView2.Settings.IsGeneralAutosaveEnabled = false;

                // حقن كود لتعطيل التدقيق الإملائي برمجياً فور تحميل الصفحة
                webView.CoreWebView2.NavigationCompleted += async (s, e) => {
                    await webView.CoreWebView2.ExecuteScriptAsync(@"
                        const style = document.createElement('style');
                        style.innerHTML = '*{ spellcheck: false !important; }';
                        document.head.appendChild(style);
                        
                        // تعطيل التدقيق الإملائي في صناديق النصوص بشكل دوري
                        setInterval(() => {
                            document.querySelectorAll('textarea, [contenteditable]').forEach(el => {
                                el.setAttribute('spellcheck', 'false');
                            });
                        }, 1000);
                    ");
                };

                webView.Source = new Uri("https://chatgpt.com");
            }
            catch (Exception ex) {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
