using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChatterLiveChat
{
    public partial class Form1 : Form
    {
        // Değişkenler
        private string username = "";
        private ListBox lstMessages = null!;
        private TextBox txtMessage = null!;
        private Button btnSend = null!;
        private TextBox txtUsername = null!;
        private Button btnLogin = null!;
        private Panel loginPanel = null!;
        private Panel chatPanel = null!;
        private ListBox lstActiveUsers = null!;
        private Timer refreshTimer = null!;
        private HttpClient httpClient = null!;
        private Timer messageTimer = null!;
        private Timer typingTimer = null!;
        private Label lblActiveTitle = null!;
        private int lastMessageId = 0;
        private DateTime joinTime = DateTime.UtcNow;
        private bool isLoggingOut = false;
        
        // Yazıyor bildirimi değişkenleri
        private bool isTyping = false;
        private DateTime lastTypingUpdate = DateTime.MinValue;
        private Label lblTypingStatus = null!;
        private Dictionary<string, DateTime> typingUsers = new Dictionary<string, DateTime>();

        // Renkler - Gece Kırmızı Tema
        private Color arkaPlanRenk = Color.FromArgb(20, 20, 30);      // Koyu lacivert
        private Color panelRenk = Color.FromArgb(30, 30, 40);         // Biraz daha açık
        private Color kirmiziRenk = Color.FromArgb(180, 40, 40);      // Koyu kırmızı
        private Color acikKirmizi = Color.FromArgb(220, 60, 60);      // Açık kırmızı
        private Color yaziRenk = Color.FromArgb(220, 220, 220);       // Açık gri
        private Color borderRenk = Color.FromArgb(100, 30, 30);       // Kırmızımsı border
        private Color yesilRenk = Color.FromArgb(40, 180, 40);        // Yeşil (online için)
        private Color sariRenk = Color.FromArgb(220, 220, 40);        // Sarı (yazıyor için)

        // Supabase bilgileri
        private readonly string supabaseUrl = "https://KENDİ SUPABASE URLNİ YAZ.supabase.co";
        private readonly string supabaseKey = "ANON PUBLİC KEYİNİ BURAYA YAZ";

        // Yasaklı kelimeler
        private List<string> badWords = new List<string> { 
            "küfür", "kufur", "piç", "pic", "amk", "aq", "sik", "siktir",
            "ananı", "babanı", "gerizekalı", "salak", "aptal", "ibne",
            "orospu", "çocuğu", "cocugu", "yarrak", "amcık", "amcik"
        };

        // Constructor
        public Form1()
        {
            // İKON EKLE - chat.ico dosyasını proje klasörüne koy
            try
            {
                if (System.IO.File.Exists("chat.ico"))
                {
                    this.Icon = new Icon("chat.ico");
                }
                else if (System.IO.File.Exists("../../chat.ico"))
                {
                    this.Icon = new Icon("../../chat.ico");
                }
                else
                {
                    // Varsayılan ikon - Windows Forms'un kendi ikonu
                    // İkon bulunamazsa hata verme
                }
            }
            catch { }

            // Form ayarları - GÖREV ÇUBUĞUNA KADAR BOYUT
            this.Text = "Chatter-LiveChat";
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.BackColor = arkaPlanRenk;
            this.ForeColor = yaziRenk;
            
            SetupUI();
            SetupHttpClient();
            this.FormClosing += Form1_FormClosing;
        }

        private void SetupHttpClient()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");
        }

        private void SetupUI()
        {
            int formWidth = this.Width;
            int formHeight = this.Height;
            
            // LOGIN PANELİ
            loginPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = arkaPlanRenk
            };

            Label lblTitle = new Label
            {
                Text = "Chatter-LiveChat",
                Font = new Font("Arial", 32, FontStyle.Bold),
                Location = new Point((formWidth - 500) / 2, formHeight / 3),
                Size = new Size(500, 70),
                ForeColor = kirmiziRenk,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblUsername = new Label
            {
                Text = "Kullanıcı Adı:",
                Location = new Point((formWidth - 340) / 2, formHeight / 2),
                Size = new Size(120, 30),
                Font = new Font("Arial", 12),
                ForeColor = yaziRenk
            };

            txtUsername = new TextBox
            {
                Location = new Point(((formWidth - 340) / 2) + 130, formHeight / 2),
                Size = new Size(210, 30),
                Font = new Font("Arial", 12),
                BackColor = panelRenk,
                ForeColor = yaziRenk,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnLogin = new Button
            {
                Text = "Giriş Yap",
                Location = new Point((formWidth - 120) / 2, formHeight / 2 + 50),
                Size = new Size(120, 40),
                BackColor = kirmiziRenk,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderColor = acikKirmizi;
            btnLogin.FlatAppearance.MouseOverBackColor = acikKirmizi;
            btnLogin.Click += BtnLogin_Click;

            loginPanel.Controls.Add(lblTitle);
            loginPanel.Controls.Add(lblUsername);
            loginPanel.Controls.Add(txtUsername);
            loginPanel.Controls.Add(btnLogin);

            // CHAT PANELİ
            chatPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false,
                BackColor = arkaPlanRenk
            };

            lstMessages = new ListBox
            {
                Location = new Point(10, 10),
                Size = new Size(formWidth - 240, formHeight - 130),
                Font = new Font("Arial", 11),
                BackColor = panelRenk,
                ForeColor = yaziRenk,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblTypingStatus = new Label
            {
                Location = new Point(10, formHeight - 110),
                Size = new Size(formWidth - 240, 20),
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = sariRenk,
                Text = ""
            };

            lblActiveTitle = new Label
            {
                Text = "Aktif Kullanıcılar",
                Location = new Point(formWidth - 220, 10),
                Size = new Size(210, 25),
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = kirmiziRenk,
                ForeColor = Color.White
            };

            lstActiveUsers = new ListBox
            {
                Location = new Point(formWidth - 220, 35),
                Size = new Size(210, formHeight - 200),
                Font = new Font("Arial", 10),
                BackColor = panelRenk,
                ForeColor = yaziRenk,
                BorderStyle = BorderStyle.FixedSingle
            };

            txtMessage = new TextBox
            {
                Location = new Point(10, formHeight - 90),
                Size = new Size(formWidth - 300, 60),
                Font = new Font("Arial", 11),
                Multiline = true,
                BackColor = panelRenk,
                ForeColor = yaziRenk,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtMessage.TextChanged += TxtMessage_TextChanged;
            txtMessage.KeyDown += TxtMessage_KeyDown;

            btnSend = new Button
            {
                Text = "Gönder",
                Location = new Point(formWidth - 280, formHeight - 90),
                Size = new Size(70, 60),
                BackColor = kirmiziRenk,
                ForeColor = Color.White,
                Font = new Font("Arial", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnSend.FlatAppearance.BorderColor = acikKirmizi;
            btnSend.FlatAppearance.MouseOverBackColor = acikKirmizi;
            btnSend.Click += BtnSend_Click;

            chatPanel.Controls.Add(lstMessages);
            chatPanel.Controls.Add(lblTypingStatus);
            chatPanel.Controls.Add(lblActiveTitle);
            chatPanel.Controls.Add(lstActiveUsers);
            chatPanel.Controls.Add(txtMessage);
            chatPanel.Controls.Add(btnSend);

            this.Controls.Add(chatPanel);
            this.Controls.Add(loginPanel);

            refreshTimer = new Timer();
            refreshTimer.Interval = 3000;
            refreshTimer.Tick += RefreshTimer_Tick;

            messageTimer = new Timer();
            messageTimer.Interval = 2000;
            messageTimer.Tick += MessageTimer_Tick;

            typingTimer = new Timer();
            typingTimer.Interval = 1000;
            typingTimer.Tick += TypingTimer_Tick;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            if (chatPanel != null && chatPanel.Visible && lstMessages != null)
            {
                int formWidth = this.Width;
                int formHeight = this.Height;
                
                lstMessages.Size = new Size(formWidth - 240, formHeight - 130);
                lblTypingStatus.Location = new Point(10, formHeight - 110);
                lblTypingStatus.Size = new Size(formWidth - 240, 20);
                
                if (lblActiveTitle != null)
                    lblActiveTitle.Location = new Point(formWidth - 220, 10);
                
                if (lstActiveUsers != null)
                {
                    lstActiveUsers.Location = new Point(formWidth - 220, 35);
                    lstActiveUsers.Size = new Size(210, formHeight - 200);
                }
                
                if (txtMessage != null)
                {
                    txtMessage.Location = new Point(10, formHeight - 90);
                    txtMessage.Size = new Size(formWidth - 300, 60);
                }
                
                if (btnSend != null)
                    btnSend.Location = new Point(formWidth - 280, formHeight - 90);
            }
            else if (loginPanel != null && loginPanel.Visible)
            {
                int formWidth = this.Width;
                int formHeight = this.Height;
                
                foreach (Control ctrl in loginPanel.Controls)
                {
                    if (ctrl is Label && ctrl.Text == "Chatter-LiveChat")
                        ctrl.Location = new Point((formWidth - 500) / 2, formHeight / 3);
                    else if (ctrl is Label && ctrl.Text == "Kullanıcı Adı:")
                        ctrl.Location = new Point((formWidth - 340) / 2, formHeight / 2);
                    else if (ctrl is TextBox)
                        ctrl.Location = new Point(((formWidth - 340) / 2) + 130, formHeight / 2);
                    else if (ctrl is Button)
                        ctrl.Location = new Point((formWidth - 120) / 2, formHeight / 2 + 50);
                }
            }
        }

        private void TxtMessage_TextChanged(object? sender, EventArgs e)
        {
            if (!isTyping && !string.IsNullOrEmpty(txtMessage.Text))
            {
                isTyping = true;
                lastTypingUpdate = DateTime.UtcNow;
                _ = UpdateTypingStatus(true);
            }
            else if (isTyping && string.IsNullOrEmpty(txtMessage.Text))
            {
                isTyping = false;
                _ = UpdateTypingStatus(false);
            }
            else if (isTyping && (DateTime.UtcNow - lastTypingUpdate).TotalSeconds > 2)
            {
                lastTypingUpdate = DateTime.UtcNow;
                _ = UpdateTypingStatus(true);
            }
        }

        private async Task UpdateTypingStatus(bool isTyping)
        {
            try
            {
                var typingData = new { 
                    username = username, 
                    is_typing = isTyping,
                    timestamp = DateTime.UtcNow 
                };
                
                string json = JsonConvert.SerializeObject(typingData);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            catch { }
        }

        private void TypingTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                var now = DateTime.UtcNow;
                var toRemove = typingUsers.Where(x => (now - x.Value).TotalSeconds > 3).Select(x => x.Key).ToList();
                foreach (var user in toRemove)
                    typingUsers.Remove(user);

                if (typingUsers.Count > 0)
                {
                    if (typingUsers.Count == 1)
                    {
                        string user = typingUsers.First().Key;
                        lblTypingStatus.Text = $"✏️ {user} yazıyor...";
                    }
                    else
                    {
                        lblTypingStatus.Text = $"✏️ {typingUsers.Count} kişi yazıyor...";
                    }
                }
                else
                {
                    lblTypingStatus.Text = "";
                }
            }
            catch { }
        }

        private async Task ShowStats()
        {
            try
            {
                string timeStr = joinTime.ToString("yyyy-MM-dd HH:mm:ss");
                string url = $"{supabaseUrl}/rest/v1/messages?select=*&created_at=gt.{timeStr}";
                
                var response = await httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var allMessages = JArray.Parse(content);
                
                int totalMessages = allMessages.Count;

                var topUsers = allMessages
                    .GroupBy(x => x["username"]?.ToString() ?? "")
                    .Select(g => new { Username = g.Key, Count = g.Count() })
                    .Where(x => x.Username != "SİSTEM")
                    .OrderByDescending(x => x.Count)
                    .Take(3)
                    .ToList();

                int myMessages = allMessages.Count(x => x["username"]?.ToString() == username);
                var today = DateTime.UtcNow.Date;
                int todayMessages = allMessages.Count(x => 
                {
                    DateTime createdAt = x["created_at"]?.Value<DateTime>() ?? DateTime.MinValue;
                    return createdAt.Date == today && createdAt >= joinTime;
                });

                string statsMessage = $"📊 ** İSTATİSTİKLER (Katıldığından Beri) **\n" +
                                     $"Toplam Mesaj: {totalMessages}\n" +
                                     $"Bugünkü Mesaj: {todayMessages}\n" +
                                     $"Senin Mesajın: {myMessages}\n" +
                                     $"🏆 En Çok Mesaj Atanlar:\n";

                for (int i = 0; i < topUsers.Count; i++)
                    statsMessage += $"   {i + 1}. {topUsers[i].Username}: {topUsers[i].Count} mesaj\n";

                var statsMsg = new { 
                    content = statsMessage, 
                    username = "SİSTEM", 
                    created_at = DateTime.UtcNow 
                };
                
                string json = JsonConvert.SerializeObject(statsMsg);
                HttpContent postContent = new StringContent(json, Encoding.UTF8, "application/json");
                await httpClient.PostAsync($"{supabaseUrl}/rest/v1/messages", postContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İstatistikler alınamadı: {ex.Message}");
            }
        }

        private async Task SendLogoutMessage()
        {
            if (!string.IsNullOrEmpty(username) && !isLoggingOut)
            {
                isLoggingOut = true;
                try
                {
                    var logoutMessage = new { 
                        content = $"👋 {username} görüşürüz!", 
                        username = "SİSTEM", 
                        created_at = DateTime.UtcNow 
                    };
                    
                    string json = JsonConvert.SerializeObject(logoutMessage);
                    HttpContent postContent = new StringContent(json, Encoding.UTF8, "application/json");
                    await httpClient.PostAsync($"{supabaseUrl}/rest/v1/messages", postContent);
                    
                    await httpClient.DeleteAsync($"{supabaseUrl}/rest/v1/active_users?username=eq.{username}");
                }
                catch { }
            }
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _ = SendLogoutMessage();
            System.Threading.Thread.Sleep(500);
            httpClient?.Dispose();
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            string name = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Kullanıcı adı boş olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ContainsBadWord(name))
            {
                MessageBox.Show("Bu kullanıcı adı yasaklı kelime içeriyor!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ÖNCE 10 SANİYEDEN ESKİ AKTİF KULLANICILARI TEMİZLE
                string cleanUrl = $"{supabaseUrl}/rest/v1/active_users?last_seen=lt.{DateTime.UtcNow.AddSeconds(-10):yyyy-MM-dd HH:mm:ss}";
                await httpClient.DeleteAsync(cleanUrl);

                // Kullanıcı adı kullanımda mı kontrol et (son 10 saniye)
                string url = $"{supabaseUrl}/rest/v1/active_users?username=eq.{name}&select=username";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();
                JArray users = JArray.Parse(content);

                if (users.Count > 0)
                {
                    MessageBox.Show("Bu kullanıcı adı şu anda kullanımda!\nBaşka bir ad dene.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                username = name;
                joinTime = DateTime.UtcNow;

                // Aktif kullanıcıya ekle
                var newUser = new { username = username, last_seen = DateTime.UtcNow };
                string json = JsonConvert.SerializeObject(newUser);
                HttpContent postContent = new StringContent(json, Encoding.UTF8, "application/json");
                await httpClient.PostAsync($"{supabaseUrl}/rest/v1/active_users", postContent);

                // Giriş mesajı gönder
                var joinMessage = new { 
                    content = $"🚪 {username} sunucuya katıldı! Hoşgeldin {username}!", 
                    username = "SİSTEM", 
                    created_at = DateTime.UtcNow 
                };
                string joinJson = JsonConvert.SerializeObject(joinMessage);
                HttpContent joinPostContent = new StringContent(joinJson, Encoding.UTF8, "application/json");
                await httpClient.PostAsync($"{supabaseUrl}/rest/v1/messages", joinPostContent);

                loginPanel.Visible = false;
                chatPanel.Visible = true;
                this.Text = $"Chatter-LiveChat - {username}";

                refreshTimer.Start();
                messageTimer.Start();
                typingTimer.Start();

                string saat = DateTime.Now.ToString("HH:mm");
                lstMessages.Items.Add($"⚡ [{saat}] SİSTEM: 🚪 {username} sunucuya katıldı! Hoşgeldin {username}!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Giriş hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void MessageTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                string timeStr = joinTime.ToString("yyyy-MM-dd HH:mm:ss");
                string url = $"{supabaseUrl}/rest/v1/messages?select=*&order=id.desc&limit=5&created_at=gt.{timeStr}";
                
                HttpResponseMessage response = await httpClient.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();
                JArray messages = JArray.Parse(content);

                foreach (JToken msg in messages.OrderBy(x => x["id"]))
                {
                    int id = msg["id"]?.Value<int>() ?? 0;
                    string msgUsername = msg["username"]?.ToString() ?? "";
                    string msgContent = msg["content"]?.ToString() ?? "";
                    DateTime createdAt = msg["created_at"]?.Value<DateTime>() ?? DateTime.UtcNow;

                    if (id > lastMessageId && msgUsername != username)
                    {
                        string saat = createdAt.ToLocalTime().ToString("HH:mm");
                        
                        if (msgUsername == "SİSTEM")
                        {
                            if (msgContent.Contains("📊"))
                                lstMessages.Items.Add($"📊 [{saat}] {msgContent}");
                            else if (msgContent.Contains("👋"))
                                lstMessages.Items.Add($"👋 [{saat}] {msgContent}");
                            else
                                lstMessages.Items.Add($"⚡ [{saat}] {msgContent}");
                        }
                        else
                        {
                            lstMessages.Items.Add($"[{saat}] {msgUsername}: {msgContent}");
                        }
                        
                        lastMessageId = id;
                        lstMessages.TopIndex = lstMessages.Items.Count - 1;
                    }
                }
            }
            catch { }
        }

        private async void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                // ÖNCE 10 SANİYEDEN ESKİ KULLANICILARI TEMİZLE
                string cleanUrl = $"{supabaseUrl}/rest/v1/active_users?last_seen=lt.{DateTime.UtcNow.AddSeconds(-10):yyyy-MM-dd HH:mm:ss}";
                await httpClient.DeleteAsync(cleanUrl);

                // Aktif kullanıcıları al
                string timeStr = DateTime.UtcNow.AddSeconds(-10).ToString("yyyy-MM-dd HH:mm:ss");
                string url = $"{supabaseUrl}/rest/v1/active_users?select=username&last_seen=gt.{timeStr}";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();
                JArray users = JArray.Parse(content);

                lstActiveUsers.Items.Clear();
                foreach (JToken user in users)
                {
                    string userUsername = user["username"]?.ToString() ?? "";
                    if (userUsername != username)
                    {
                        if (typingUsers.ContainsKey(userUsername))
                            lstActiveUsers.Items.Add($"✏️ {userUsername}");
                        else
                            lstActiveUsers.Items.Add($"🟢 {userUsername}");
                    }
                    else
                    {
                        lstActiveUsers.Items.Add($"🟢 {userUsername} (sen)");
                    }
                }

                // LastSeen'i güncelle
                var updateUser = new { last_seen = DateTime.UtcNow };
                string json = JsonConvert.SerializeObject(updateUser);
                HttpContent patchContent = new StringContent(json, Encoding.UTF8, "application/json");
                
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Patch, $"{supabaseUrl}/rest/v1/active_users?username=eq.{username}");
                request.Content = patchContent;
                request.Headers.Add("apikey", supabaseKey);
                request.Headers.Add("Authorization", $"Bearer {supabaseKey}");
                
                await httpClient.SendAsync(request);
            }
            catch { }
        }

        private async void BtnSend_Click(object? sender, EventArgs e)
        {
            await SendMessage();
        }

        private async void TxtMessage_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                await SendMessage();
            }
            else if (txtMessage.Text.Trim() == "/stats" && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtMessage.Clear();
                await ShowStats();
            }
        }

        private async Task SendMessage()
        {
            string content = txtMessage.Text.Trim();

            if (string.IsNullOrEmpty(content))
                return;

            if (content == "/stats")
            {
                txtMessage.Clear();
                await ShowStats();
                return;
            }

            if (ContainsBadWord(content))
            {
                MessageBox.Show("Mesajınız yasaklı kelime içeriyor!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMessage.Clear();
                return;
            }

            try
            {
                var message = new { content = content, username = username, created_at = DateTime.UtcNow };
                string json = JsonConvert.SerializeObject(message);
                HttpContent postContent = new StringContent(json, Encoding.UTF8, "application/json");
                
                await httpClient.PostAsync($"{supabaseUrl}/rest/v1/messages", postContent);

                if (isTyping)
                {
                    isTyping = false;
                    await UpdateTypingStatus(false);
                }

                string saat = DateTime.Now.ToString("HH:mm");
                
                lstMessages.Items.Add($"[{saat}] {username}: {content}");
                lstMessages.TopIndex = lstMessages.Items.Count - 1;
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mesaj gönderilemedi: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ContainsBadWord(string text)
        {
            string lowerText = text.ToLower();
            foreach (string word in badWords)
            {
                if (lowerText.Contains(word.ToLower()))
                    return true;
            }
            return false;
        }
    }
}
