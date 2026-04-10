using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace ChatGPTApp
{
    public partial class Form1 : Form
    {
        private WebView2? webView; // Nullable لتجنب CS8618

public Form1()
{
    this.Text = "ChatGPT Lite";
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
                        AdditionalBrowserArguments = "--enable-gpu --disable-features=RendererCodeIntegrity"
                    });

                await webView.EnsureCoreWebView2Async(env);

                // تحسين الأداء
                webView.CoreWebView2.Settings.IsScriptEnabled = true;
                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                webView.CoreWebView2.Settings.IsZoomControlEnabled = false;

                // تحميل ChatGPT
                webView.CoreWebView2.Navigate("https://chat.openai.com");

                // تقليل lag عبر تنظيف DOM (اختياري لكن قوي)
                webView.CoreWebView2.NavigationCompleted += async (s, e) =>
                {
                    try
                    {
                        await webView.CoreWebView2.ExecuteScriptAsync(@"
                            (function(){
                                const keep = 60;
                                const msgs = document.querySelectorAll('article');
                                if(msgs.length > keep){
                                    for(let i=0;i<msgs.length-keep;i++){
                                        msgs[i].remove();
                                    }
                                }
                            })();
                        ");
                    }
                    catch {}
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في تهيئة WebView2:\n" + ex.Message);
            }
        }
    }
}
