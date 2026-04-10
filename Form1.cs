using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace ChatGPTLite
{
    public class Form1 : Form
    {
        private WebView2? webView;

        public Form1()
        {
            this.Text = "ChatGPT Lite (High Performance)";
            this.Width = 1000;
            this.Height = 800;

            InitWebView();
        }

        private async void InitWebView()
        {
            try
            {
                webView = new WebView2
                {
                    Dock = DockStyle.Fill
                };

                this.Controls.Add(webView);

                var env = await CoreWebView2Environment.CreateAsync(
                    null,
                    null,
                    new CoreWebView2EnvironmentOptions
                    {
                        AdditionalBrowserArguments = "--enable-gpu"
                    });

                await webView.EnsureCoreWebView2Async(env);

                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

                webView.CoreWebView2.Navigate("https://chat.openai.com");

                webView.CoreWebView2.NavigationCompleted += async (s, e) =>
                {
                    await InjectPerformanceScript();
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("WebView2 Error:\n" + ex.Message);
            }
        }

        private async System.Threading.Tasks.Task InjectPerformanceScript()
        {
            if (webView?.CoreWebView2 == null) return;

            string script = @"
                (function () {
                    const KEEP_VISIBLE = 60;
                    const SHOW_BUFFER = 20;

                    function update() {
                        const messages = Array.from(document.querySelectorAll('article'));
                        if (messages.length <= KEEP_VISIBLE) return;

                        const start = messages.length - KEEP_VISIBLE;

                        messages.forEach((el, index) => {
                            if (index < start - SHOW_BUFFER) {
                                el.style.display = 'none';
                            } else {
                                el.style.display = '';
                            }
                        });
                    }

                    update();

                    window.addEventListener('scroll', update);

                    const observer = new MutationObserver(update);
                    observer.observe(document.body, {
                        childList: true,
                        subtree: true
                    });
                })();
            ";

            try
            {
                await webView.CoreWebView2.ExecuteScriptAsync(script);
            }
            catch { }
        }
    }
}
