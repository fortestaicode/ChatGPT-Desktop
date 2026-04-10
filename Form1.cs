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
    webView = new WebView2 { Dock = DockStyle.Fill };
    this.Controls.Add(webView);

    // وسائط تشغيل قوية جداً لفرض استخدام كارت الشاشة وتعطيل العمليات الثقيلة
    var options = new CoreWebView2EnvironmentOptions(
        additionalBrowserArguments: "--enable-gpu-rasterization --ignore-gpu-blocklist --disable-low-res-tiling --disable-features=Translate"
    );

    var env = await CoreWebView2Environment.CreateAsync(null, null, options);
    await webView.EnsureCoreWebView2Async(env);

    // 1. تعطيل التدقيق الإملائي (Spelling Check) نهائياً
    webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
    await webView.CoreWebView2.Profile.SetPermissionStateAsync(CoreWebView2PermissionKind.Autofill, CoreWebView2PermissionState.Deny);

    // 2. حقن كود CSS و JS لتحسين الأداء عند تحميل أي صفحة
    webView.CoreWebView2.NavigationCompleted += async (s, e) =>
    {
        // تعطيل التدقيق الإملائي داخل حقل الكتابة في ChatGPT
        await webView.CoreWebView2.ExecuteScriptAsync(@"
            const style = document.createElement('style');
            style.innerHTML = '*{ spellcheck: false !important; content-visibility: auto !important; }';
            document.head.appendChild(style);
            
            // محاولة إيقاف التدقيق في مربعات النص
            setInterval(() => {
                const textareas = document.querySelectorAll('textarea, div[contenteditable]');
                textareas.forEach(el => {
                    if(el.getAttribute('spellcheck') !== 'false') {
                        el.setAttribute('spellcheck', 'false');
                    }
                });
            }, 2000);
        ");
    };

    webView.Source = new Uri("https://chatgpt.com");
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
