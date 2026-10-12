using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using PersonnelManagement.DAL;
using PersonnelManagement.Models;
using static PersonnelManagement.DAL.IzinTuruDAL;

internal static partial class RegressionChecks
{
    private static SqlConnectionStringBuilder server;
    private static SqlConnection db;
    private static string scratch;
    private static int observations;
    private static string stage = "setup";

    private static SqlConnection Connect(string database)
    {
        var settings = new SqlConnectionStringBuilder(server.ConnectionString);
        settings.InitialCatalog = database;
        settings.ConnectTimeout = 5;
        var connection = new SqlConnection(settings.ConnectionString);
        connection.Open();
        return connection;
    }

    private static int Scalar(string sql)
    {
        using (var command = new SqlCommand(sql, db))
            return Convert.ToInt32(command.ExecuteScalar());
    }

    private static void Exec(string sql)
    {
        using (var command = new SqlCommand(sql, db)) command.ExecuteNonQuery();
    }

    private static void Observe(string name, bool confirmed, string detail)
    {
        Console.WriteLine((confirmed ? "PASS" : "FAIL") + " | " + name + " | " + detail);
        if (!confirmed) throw new InvalidOperationException("Test failed: " + name);
        observations++;
    }

    private static int Employee(int sequence, int shift, int department, int position)
    {
        return new PersonelDal().Kaydet(new Personel
        {
            TcKimlikNo = (80000000000L + sequence).ToString(), Ad = "Test", Soyad = "Fixture" + sequence,
            DepartmanId = department, PozisyonId = position, VardiyaId = shift,
            IseGirisTarihi = DateTime.Today.AddYears(-1), YillikIzinHakki = 14
        });
    }

    private static int Leave(int employee, int type, DateTime first, DateTime last)
    {
        return new IzinDal().Kaydet(new Izin
        {
            PersonelId = employee, IzinTuruId = type,
            BaslangicTarihi = first, BitisTarihi = last, Aciklama = "Synthetic test record"
        });
    }


    private static bool Denied(Action action)
    {
        try{action();return false;}catch(InvalidOperationException){return true;}catch(UnauthorizedAccessException){return true;}catch(ArgumentException){return true;}
    }
    private static int ManualLeave(int employee,int type,DateTime bas,DateTime bit)
    {
        using(var command=new SqlCommand("INSERT dbo.Izin(PersonelId,IzinTuruId,BaslangicTarihi,BitisTarihi,ToplamGun) VALUES(@Pid,@Type,@Bas,@Bit,@Days);SELECT CAST(SCOPE_IDENTITY() AS int);",db))
        {
            command.Parameters.AddWithValue("@Pid",employee);command.Parameters.AddWithValue("@Type",type);
            command.Parameters.AddWithValue("@Bas",bas);command.Parameters.AddWithValue("@Bit",bit);command.Parameters.AddWithValue("@Days",(bit-bas).Days+1);
            return Convert.ToInt32(command.ExecuteScalar());
        }
    }
    private static int AbsenceCount(int id,DateTime date,DateTime now)
    {
        using(var command=new SqlCommand("SELECT COUNT(*) FROM dbo.fnDevamsizlik(@Day,@Day,@Now) WHERE PersonelId=@Pid OPTION(MAXRECURSION 0)",db))
        {
            command.Parameters.AddWithValue("@Day",date);command.Parameters.AddWithValue("@Now",now);command.Parameters.AddWithValue("@Pid",id);
            return Convert.ToInt32(command.ExecuteScalar());
        }
    }
    private static bool SqlDenied(Action action)
    {
        try { action(); return false; } catch (SqlException) { return true; }
    }
    private static void RunBatches(string text, SqlConnection connection)
    {
        foreach (string batch in Regex.Split(text, @"^[ \t]*GO[ \t]*\r?$",RegexOptions.Multiline|RegexOptions.IgnoreCase).Where(s=>!string.IsNullOrWhiteSpace(s)))
            using(var command=new SqlCommand(batch,connection)) command.ExecuteNonQuery();
    }
    private static void AdditionalChecks(int department,int position,int shift,int admin,int worker,int leaveType,string[] args)
    {
        var people=new PersonelDal();
        var shifts=new VardiyaDal();
        var leaves=new IzinDal();
        var users=new KullaniciDal();
        var attendance=new GirisKayitDAL();
        int count=Scalar("SELECT COUNT(*) FROM dbo.Personel");
        Func<Personel> valid=()=>new Personel { TcKimlikNo="89999999990",Ad="Synthetic",Soyad="Validation",DepartmanId=department,PozisyonId=position,VardiyaId=shift,IseGirisTarihi=DateTime.Today.AddYears(-1),YillikIzinHakki=14 };
        var invalid=valid();invalid.TcKimlikNo="+8000000000";
        Observe("ascii_identity",Denied(()=>people.Kaydet(invalid)),"signed identifiers rejected");
        invalid=valid();invalid.TcKimlikNo="８００００００００００";
        Observe("unicode_identity",Denied(()=>people.Kaydet(invalid)),"only ASCII digits accepted");
        invalid=valid();invalid.Ad=new string('a',51);
        Observe("name_length",Denied(()=>people.Kaydet(invalid)),"oversized name rejected before insert");
        invalid=valid();invalid.DogumTarihi=DateTime.Today.AddDays(1);
        Observe("future_birth",Denied(()=>people.Kaydet(invalid)),"future birth date rejected");
        invalid=valid();invalid.DogumTarihi=new DateTime(1600,1,1);
        Observe("birth_date_range",Denied(()=>people.Kaydet(invalid)),"unsupported birth date rejected before SQL conversion");
        invalid=valid();invalid.YillikIzinHakki=-1;
        Observe("negative_allowance",Denied(()=>people.Kaydet(invalid)),"negative allowance rejected");
        Observe("invalid_insert_rollback",Scalar("SELECT COUNT(*) FROM dbo.Personel")==count,"invalid employees not inserted");
        new DepartmanDAL().Kaydet(new Departman { DepartmanAdi="Other synthetic department" });
        int otherDepartment=Scalar("SELECT MAX(DepartmanId) FROM dbo.Departman");
        invalid=valid();invalid.DepartmanId=otherDepartment;
        Observe("position_department",Denied(()=>people.Kaydet(invalid)),"position must belong to department");
        Observe("used_shift_deactivation",Denied(()=>shifts.Sil(shift)),"assigned shift stays active");
        Observe("used_department_deactivation",Denied(()=>new DepartmanDAL().Sil(department)),"assigned department stays active");
        Observe("used_position_deactivation",Denied(()=>new PozisyonDal().Sil(position)),"assigned position stays active");
        Func<Vardiya> validShift=()=>new Vardiya { VardiyaAdi="Validation shift",BaslangicSaati=TimeSpan.FromHours(8),BitisSaati=TimeSpan.FromHours(17),MolaDakika=0,Pazartesi=true };
        var badShift=validShift();badShift.BitisSaati=badShift.BaslangicSaati;
        Observe("zero_shift_duration",Denied(()=>shifts.Kaydet(badShift)),"equal start/end rejected");
        badShift=validShift();badShift.MolaDakika=540;
        Observe("oversized_shift_break",Denied(()=>shifts.Kaydet(badShift)),"break cannot consume entire shift");
        badShift=validShift();badShift.Pazartesi=false;
        Observe("no_workday",Denied(()=>shifts.Kaydet(badShift)),"at least one workday required");
        int employee=people.Kaydet(valid());
        Observe("null_birth_persisted",!people.IdIleGetir(employee).DogumTarihi.HasValue,"nullable birth survives SQL mapping");
        Observe("literal_search",people.Ara("%_").Count==0,"wildcard characters searched literally");
        Observe("invalid_leave_dates",Denied(()=>leaves.Kaydet(new Izin { PersonelId=employee,IzinTuruId=leaveType,BaslangicTarihi=DateTime.Today.AddDays(2),BitisTarihi=DateTime.Today })),"reversed leave interval rejected");
        Observe("invalid_report_dates",Denied(()=>new RaporDal().PersonelOzetRaporu(DateTime.Today,DateTime.Today.AddDays(-1))),"reversed report interval rejected");
        Observe("invalid_attendance_dates",Denied(()=>attendance.TarihAraligindaGetir(DateTime.Today,DateTime.Today.AddDays(-1))),"reversed attendance interval rejected");
        Observe("sql_identity_constraint",SqlDenied(()=>Exec("UPDATE dbo.Personel SET TcKimlikNo='abcdefghijk' WHERE PersonelId="+employee)),"direct invalid SQL identity rejected");
        Observe("sql_position_constraint",SqlDenied(()=>Exec("UPDATE dbo.Personel SET DepartmanId="+otherDepartment+" WHERE PersonelId="+employee)),"direct position/department mismatch rejected");
        Observe("sql_shift_constraint",SqlDenied(()=>Exec("UPDATE dbo.Vardiya SET MolaDakika=9999 WHERE VardiyaId="+shift)),"direct invalid shift rejected");
        string setup=File.ReadAllText(args[1]).Replace("PersonelTakipV090Db", scratch).Replace("PersonelTakipDb",scratch);
        int departments=Scalar("SELECT COUNT(*) FROM dbo.Departman");
        bool repeatStopped=false;
        using(var master=Connect("master"))try{RunBatches(setup,master);}catch(SqlException ex){repeatStopped=ex.Number==51000;}
        Observe("setup_repeat_preserves_data",repeatStopped&&Scalar("SELECT COUNT(*) FROM dbo.Departman")==departments,"existing database is not recreated");
        string update=File.ReadAllText(Path.Combine(Path.GetDirectoryName(args[1]),"DatabaseUpdate.sql"));
        string[] tables={"Departman","Pozisyon","Vardiya","Personel","Kullanici","GirisKayit","Devamsizlik","IzinTuru","Izin","SistemLog"};
        int[] before=tables.Select(t=>Scalar("SELECT COUNT(*) FROM dbo.["+t+"]")).ToArray();
        RunBatches(update,db);RunBatches(update,db);
        Observe("update_repeat_preserves_all_tables",before.SequenceEqual(tables.Select(t=>Scalar("SELECT COUNT(*) FROM dbo.["+t+"]"))),"all ten table counts preserved");
        users.Kaydet(new Kullanici { KullaniciAdi="case_fixture",Ad="Synthetic",Soyad="Login",Rol="Admin" },"CaseFixture123!");
        int loginId=Scalar("SELECT KullaniciId FROM dbo.Kullanici WHERE KullaniciAdi=N'case_fixture'");
        Observe("password_case_sensitive",users.GirisDogrula("case_fixture","casefixture123!")==null,"password case matters");
        Observe("password_space_sensitive",users.GirisDogrula("case_fixture","CaseFixture123! ")==null,"password spaces matter");
        Observe("valid_login",users.GirisDogrula("case_fixture","CaseFixture123!")!=null,"valid password opens session");
        Observe("login_timestamp_and_audit",Scalar("SELECT COUNT(*) FROM dbo.Kullanici WHERE KullaniciId="+loginId+" AND SonGirisTarihi IS NOT NULL")==1&&Scalar("SELECT COUNT(*) FROM dbo.SistemLog WHERE KullaniciId="+loginId+" AND Islem=N'Login'")==1,"login and audit committed together");
        users.Kaydet(new Kullanici { KullaniciAdi="audit_login_fixture",Ad="Synthetic",Soyad="Failure",Rol="Admin" },"AuditFixture123!");
        int auditLogin=Scalar("SELECT KullaniciId FROM dbo.Kullanici WHERE KullaniciAdi=N'audit_login_fixture'");
        int leave=Leave(employee,leaveType,DateTime.Today.AddDays(90),DateTime.Today.AddDays(91));
        Exec("CREATE TRIGGER dbo.TR_AuditFailure ON dbo.SistemLog INSTEAD OF INSERT AS THROW 51099,N'Synthetic audit failure',1;");
        try
        {
            Observe("audit_personnel_failure",SqlDenied(()=>people.Kaydet(new Personel { TcKimlikNo="89999999991",Ad="Synthetic",Soyad="Rollback",DepartmanId=department,PozisyonId=position,VardiyaId=shift,IseGirisTarihi=DateTime.Today.AddYears(-1),YillikIzinHakki=14 })),"audit failure propagates");
            Observe("audit_personnel_rollback",Scalar("SELECT COUNT(*) FROM dbo.Personel WHERE TcKimlikNo='89999999991'")==0,"employee insert rolled back");
            Observe("audit_login_failure",SqlDenied(()=>users.GirisDogrula("audit_login_fixture","AuditFixture123!")),"login blocked by audit failure");
            Observe("audit_login_rollback",Scalar("SELECT COUNT(*) FROM dbo.Kullanici WHERE KullaniciId="+auditLogin+" AND SonGirisTarihi IS NULL")==1,"failed login did not update timestamp");
            Observe("audit_leave_failure",SqlDenied(()=>leaves.Onayla(leave,admin)),"approval blocked by audit failure");
            Observe("audit_leave_rollback",Scalar("SELECT COUNT(*) FROM dbo.Izin WHERE IzinId="+leave+" AND Durum=N'Beklemede'")==1,"failed approval stays pending");
            Observe("audit_attendance_failure",SqlDenied(()=>attendance.GirisYap(employee,admin)),"attendance blocked by audit failure");
            Observe("audit_attendance_rollback",Scalar("SELECT COUNT(*) FROM dbo.GirisKayit WHERE PersonelId="+employee)==0,"failed attendance entry rolled back");
        }
        finally { Exec("DROP TRIGGER dbo.TR_AuditFailure;"); }
        SistemLogger.SetKullanici(admin);
        var current=users.TumunuGetir().First(u=>u.KullaniciId==admin);
        typeof(PersonnelManagement.Forms.frmLogin).GetProperty("AktifKullanici").GetSetMethod(true).Invoke(null,new object[]{current});
        int historicalPerson=Employee(890,shift,department,position);
        int historicalLeave=ManualLeave(historicalPerson,leaveType,DateTime.Today.AddDays(110),DateTime.Today.AddDays(111));
        people.Sil(historicalPerson,DateTime.Today);
        foreach(Type type in new[]{typeof(PersonnelManagement.Forms.frmLogin),typeof(PersonnelManagement.Forms.frmAnaMenu),typeof(PersonnelManagement.Forms.frmPersonelYonetim),typeof(PersonnelManagement.Forms.frmVardiyaYonetim),typeof(PersonnelManagement.Forms.frmIzinYonetim),typeof(PersonnelManagement.Forms.frmGirisKayit),typeof(PersonnelManagement.Forms.frmRaporlar)})
        {
            using(var form=(System.Windows.Forms.Form)Activator.CreateInstance(type))
            {
                form.GetType().GetMethod("OnLoad",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(form,new object[]{EventArgs.Empty});
                Observe("load_"+type.Name,!form.IsDisposed,"form initialized against isolated database");
                if(type==typeof(PersonnelManagement.Forms.frmLogin))
                {
                    Observe("single_login_button",form.Controls.OfType<System.Windows.Forms.Button>().Count(b=>b.Text=="Giriş")==1,"one functional login button");
                    Observe("login_password_length",form.Controls.OfType<System.Windows.Forms.TextBox>().Single(t=>t.Name=="txtSifre").MaxLength==100,"login accepts the stored password column length");
                }
                if(type==typeof(PersonnelManagement.Forms.frmPersonelYonetim))
                {
                    var flags=BindingFlags.Instance|BindingFlags.NonPublic;
                    type.GetMethod("dgvPersonel_CellClick",flags).Invoke(form,new object[]{null,new System.Windows.Forms.DataGridViewCellEventArgs(0,0)});
                    Observe("personnel_selection",(int)type.GetField("_seciliPersonelId",flags).GetValue(form)>0,"fixture selected through row handler");
                    var search=form.Controls.Find("txtAra",true).Cast<System.Windows.Forms.TextBox>().Single();
                    search.Text="not_present_fixture";
                    type.GetMethod("PersonelleriYukle",flags).Invoke(form,new object[]{search.Text});
                    Observe("search_clears_stale_selection",(int)type.GetField("_seciliPersonelId",flags).GetValue(form)==0&&!form.Controls.Find("btnGuncelle",true).Single().Enabled,"hidden row cannot be edited after search");
                    Observe("search_text_retained",search.Text=="not_present_fixture","refresh keeps requested search text");
                    string photo=Path.Combine(args[2],"photo_fixture.bmp");
                    using(var source=new System.Drawing.Bitmap(12,8))source.Save(photo,System.Drawing.Imaging.ImageFormat.Bmp);
                    type.GetMethod("FotografYukle",flags).Invoke(form,new object[]{photo});
                    using(var exclusive=new FileStream(photo,FileMode.Open,FileAccess.ReadWrite,FileShare.None))
                        Observe("photo_file_unlocked",exclusive.Length>0,"image loading releases file handle");
                    File.Delete(photo);
                    var picture=form.Controls.Find("picFoto",true).Cast<System.Windows.Forms.PictureBox>().Single();
                    Observe("photo_copy_valid",picture.Image.Width==12&&picture.Image.Height==8,"copied bitmap survives closed and deleted source");
                    type.GetMethod("FotografYukle",flags).Invoke(form,new object[]{null});
                    Observe("photo_reset",picture.Image==null,"old image released when resetting photo");
                }
                if(type==typeof(PersonnelManagement.Forms.frmIzinYonetim))
                {
                    var flags=BindingFlags.Instance|BindingFlags.NonPublic;
                    var grid=form.Controls.Find("dgvIzinler",true).Cast<System.Windows.Forms.DataGridView>().Single();
                    int activeIndex=grid.Rows.Cast<System.Windows.Forms.DataGridViewRow>().First(r=>(r.DataBoundItem as Izin)?.IzinId==leave).Index;
                    int historicalIndex=grid.Rows.Cast<System.Windows.Forms.DataGridViewRow>().First(r=>(r.DataBoundItem as Izin)?.IzinId==historicalLeave).Index;
                    type.GetMethod("dgvIzinler_CellClick",flags).Invoke(form,new object[]{null,new System.Windows.Forms.DataGridViewCellEventArgs(0,activeIndex)});
                    var selection=form.Controls.Find("cmbIzinPersonel",true).Cast<System.Windows.Forms.ComboBox>().Single();
                    Observe("leave_active_person_selected",(selection.SelectedItem as Personel)?.PersonelId==employee,"active leave row selects its actual person");
                    type.GetMethod("dgvIzinler_CellClick",flags).Invoke(form,new object[]{null,new System.Windows.Forms.DataGridViewCellEventArgs(0,historicalIndex)});
                    Observe("leave_history_clears_stale_person",selection.SelectedIndex==-1,"inactive historical row cannot retain previously selected active person");
                }
            }
        }
        using(var grid=new System.Windows.Forms.DataGridView { AllowUserToAddRows=false })
        {
            grid.Columns.Add("Value","Synthetic rows");
            for(int i=0;i<250;i++)grid.Rows.Add("Row "+i);
            using(var document=new ReportPrintDocument(grid,"Synthetic pagination"))
            using(var bitmap=new System.Drawing.Bitmap(1000,700))
            using(var graphics=System.Drawing.Graphics.FromImage(bitmap))
            {
                var flags=BindingFlags.NonPublic|BindingFlags.Instance;
                document.GetType().GetMethod("OnBeginPrint",flags).Invoke(document,new object[]{new System.Drawing.Printing.PrintEventArgs()});
                int pages=0;bool more;
                do
                {
                    var page=new System.Drawing.Printing.PrintPageEventArgs(graphics,new System.Drawing.Rectangle(40,40,900,600),new System.Drawing.Rectangle(0,0,1000,700),new System.Drawing.Printing.PageSettings());
                    document.GetType().GetMethod("OnPrintPage",flags).Invoke(document,new object[]{page});more=page.HasMorePages;pages++;
                    if(pages>50)throw new InvalidOperationException("Pagination did not finish.");
                }while(more);
                Observe("print_all_rows",pages>1&&(int)document.GetType().GetField("_nextRow",flags).GetValue(document)==250,"all 250 rows processed across pages");
            }
        }
    }

    [STAThread]
    private static int Main(string[] args)
    {
        bool created = false;
        int exit = 0;
        try
        {
            string configured = Environment.GetEnvironmentVariable("PERSONNELMANAGEMENT_TEST_ADMIN_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(configured)) throw new InvalidOperationException("A test admin connection is required.");
            server = new SqlConnectionStringBuilder(configured);
            string host = server.DataSource.Split('\\')[0].Split(',')[0];
            if (host.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase)) host = host.Substring(4);
            if (!(host == "." || host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                || host == "127.0.0.1" || host == "[::1]"
                || host.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Regression tests require a local SQL Server.");

            scratch = "PersonnelManagementFix_" + Guid.NewGuid().ToString("N");
            if (!Regex.IsMatch(scratch, "^PersonnelManagementFix_[a-f0-9]{32}$")
                || scratch.Equals(server.InitialCatalog, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Scratch database safety guard failed.");
            string script = File.ReadAllText(args[1]).Replace("PersonelTakipV090Db", scratch).Replace("PersonelTakipDb", scratch);
            stage = "isolated schema setup";
            using (var master = Connect("master"))
            {
                    created = true;
                    foreach (string batch in Regex.Split(script, @"^[ \t]*GO[ \t]*(?:--[^\r\n]*)?\r?$",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase).Where(s => !string.IsNullOrWhiteSpace(s)))
                        using (var command = new SqlCommand(batch, master)) command.ExecuteNonQuery();
            }
            db = Connect(scratch);
            var applicationConnection = new SqlConnectionStringBuilder(server.ConnectionString);
            applicationConnection.InitialCatalog = scratch;
            Environment.SetEnvironmentVariable("PERSONNELMANAGEMENT_V090_CONNECTION_STRING", applicationConnection.ConnectionString);
            typeof(SqlDBConnection).GetField("_connection", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, db);
            stage = "synthetic fixtures";
            int department = new DepartmanDAL().Kaydet(new Departman { DepartmanAdi = "Test Department" })
                ? Scalar("SELECT MAX(DepartmanId) FROM dbo.Departman") : 0;
            new PozisyonDal().Kaydet(new Pozisyon { DepartmanId = department, PozisyonAdi = "Test Position" });
            int position = Scalar("SELECT MAX(PozisyonId) FROM dbo.Pozisyon");
            var shifts = new VardiyaDal();
            int dayShift = shifts.Kaydet(new Vardiya
            {
                VardiyaAdi = "Test Weekdays", BaslangicSaati = TimeSpan.FromHours(8),
                BitisSaati = TimeSpan.FromHours(17), MolaDakika = 60,
                Pazartesi = true, Sali = true, Carsamba = true, Persembe = true, Cuma = true
            });
            int nightShift = shifts.Kaydet(new Vardiya
            {
                VardiyaAdi = "Test Night", BaslangicSaati = TimeSpan.FromHours(22),
                BitisSaati = TimeSpan.FromHours(6), MolaDakika = 30,
                Pazartesi = true, Sali = true, Carsamba = true, Persembe = true, Cuma = true
            });
            Exec("INSERT dbo.Kullanici (KullaniciAdi,Sifre,Ad,Soyad,Rol) VALUES "
                + "(N'test_admin',N'fixture-only',N'Test',N'Admin',N'Admin'),"
                + "(N'test_employee',N'fixture-only',N'Test',N'Employee',N'Calisan');"
                + "INSERT dbo.IzinTuru (TuruAdi,Ucretli) VALUES (N'Yıllık İzin',1);");
            int admin = Scalar("SELECT KullaniciId FROM dbo.Kullanici WHERE Rol=N'Admin'");
            int worker = Scalar("SELECT KullaniciId FROM dbo.Kullanici WHERE Rol=N'Calisan'");
            int leaveType = Scalar("SELECT MAX(IzinTuruId) FROM dbo.IzinTuru");
            var attendance = new GirisKayitDAL();
            var leaves = new IzinDal();
            var reports = new RaporDal();
            var today = DateTime.Today;


            stage="night exit";
            int nightEmployee=Employee(1,nightShift,department,position);
            Exec("INSERT dbo.GirisKayit (PersonelId,KayitTarihi,GirisSaati,GirisZamani,PlanlananGirisZamani,PlanlananCikisZamani,PlanlananMolaDakika,VardiyaAdiKayit) VALUES ("
                +nightEmployee+",DATEADD(day,-1,CAST(GETDATE() AS date)),'22:00',DATEADD(hour,22,CAST(DATEADD(day,-1,CAST(GETDATE() AS date)) AS datetime)),"
                +"DATEADD(hour,22,CAST(DATEADD(day,-1,CAST(GETDATE() AS date)) AS datetime)),DATEADD(hour,6,CAST(CAST(GETDATE() AS date) AS datetime)),30,N'Test Night')");
            Observe("night_exit",attendance.CikisYap(nightEmployee,admin),"previous-day open entry closed");
            Observe("night_closed",!attendance.AcikKayitVarMi(nightEmployee),"no open entry remains");
            var midnight=GirisKayitDAL.VardiyaPlani(today.AddMinutes(30),TimeSpan.FromHours(22),TimeSpan.FromHours(6));
            Observe("night_shift_date",midnight.baslangic==today.AddDays(-1).AddHours(22)&&midnight.bitis==today.AddHours(6),"after-midnight entry belongs to previous shift date");
            var work=GirisKayitDAL.CalismaSuresi(today.AddDays(-1).AddHours(22),today.AddHours(6),midnight.baslangic,midnight.bitis,30);
            Observe("night_duration",work.calismaDakika==450&&work.mesaiDakika==0,"8-hour overnight shift minus 30-minute break");
            Observe("clock_backwards",Denied(()=>GirisKayitDAL.CalismaSuresi(today.AddHours(8),today.AddHours(7),today.AddHours(8),today.AddHours(17),60)),"negative duration rejected");

            stage="leave approval invariants";
            int overlapEmployee=Employee(2,dayShift,department,position);
            int first=Leave(overlapEmployee,leaveType,today.AddDays(10),today.AddDays(12));
            Observe("pending_overlap",Denied(()=>Leave(overlapEmployee,leaveType,today.AddDays(11),today.AddDays(13))),"overlapping pending request rejected");
            int second=ManualLeave(overlapEmployee,leaveType,today.AddDays(11),today.AddDays(13));
            Observe("admin_approval",leaves.Onayla(first,admin),"valid admin approval succeeds");
            Observe("approved_overlap",Denied(()=>leaves.Onayla(second,admin)),"existing overlapping pending request cannot be approved");
            Observe("approval_rollback",Scalar("SELECT COUNT(*) FROM dbo.Izin WHERE IzinId="+second+" AND Durum=N'Beklemede'")==1,"failed approval did not change status");
            Observe("employee_approval",Denied(()=>leaves.Onayla(second,worker)),"non-admin actor rejected at DAL");
            Observe("employee_cancellation",Denied(()=>leaves.Iptal(first,worker)),"non-admin cancellation rejected");
            Observe("admin_cancellation",leaves.Iptal(first,admin)&&Scalar("SELECT COUNT(*) FROM dbo.Izin WHERE IzinId="+first+" AND Durum=N'İptal Edildi' AND IptalEdenKullaniciId="+admin+" AND IptalTarihi IS NOT NULL")==1,"cancellation records actor and time");
            Observe("cancelled_reapproval",Denied(()=>leaves.Onayla(first,admin)),"cancelled request cannot be reapproved");
            Observe("cancelled_rejection",Denied(()=>leaves.Reddet(first,admin,"fixture")),"cancelled request cannot be rejected");
            Observe("empty_rejection_reason",Denied(()=>leaves.Reddet(second,admin,"")),"empty rejection reason rejected");

            stage="annual allowance";
            int balanceEmployee=Employee(3,dayShift,department,position);
            var yearStart=new DateTime(today.Year,1,1);
            Observe("oversized_leave_save",Denied(()=>Leave(balanceEmployee,leaveType,yearStart.AddDays(100),yearStart.AddDays(119))),"20-day request rejected for 14-day allowance");
            int oversized=ManualLeave(balanceEmployee,leaveType,yearStart.AddDays(100),yearStart.AddDays(119));
            Observe("oversized_leave_approval",Denied(()=>leaves.Onayla(oversized,admin)),"legacy pending 20-day request cannot be approved");
            Observe("balance_nonnegative",leaves.KalanYillikIzin(balanceEmployee)==14,"failed approval leaves balance unchanged");
            int ten=Leave(balanceEmployee,leaveType,yearStart.AddDays(10),yearStart.AddDays(19));leaves.Onayla(ten,admin);
            Observe("remaining_allowance",leaves.KalanYillikIzin(balanceEmployee)==4,"approved 10-day leave consumes 10 days");
            Observe("cumulative_allowance",Denied(()=>Leave(balanceEmployee,leaveType,yearStart.AddDays(30),yearStart.AddDays(34))),"cumulative use cannot exceed allowance");

            stage="report date intersection";
            int reportEmployee=Employee(4,dayShift,department,position);
            var monthStart=new DateTime(today.Year,today.Month,1);
            int overlap=Leave(reportEmployee,leaveType,monthStart.AddDays(-2),monthStart.AddDays(2));leaves.Onayla(overlap,admin);
            Observe("report_overlap",reports.PersonelOzetRaporu(monthStart,monthStart.AddDays(4),reportEmployee).Single().ToplamIzinGun==3,"only in-range days counted");
            Observe("report_single_day",reports.PersonelOzetRaporu(monthStart.AddDays(-2),monthStart.AddDays(-2),reportEmployee).Single().ToplamIzinGun==1,"single-day report counts one day");
            Observe("leave_filter_overlap",leaves.TarihAraligindaGetir(monthStart,monthStart,reportEmployee).Count==1,"date filter includes overlapping leave");
            Observe("leave_filter_exclusion",leaves.TarihAraligindaGetir(monthStart.AddDays(10),monthStart.AddDays(11),reportEmployee).Count==0,"out-of-range leave excluded");
            Observe("all_leave_statuses",leaves.TarihAraligindaGetir(null,null).Any(x=>x.IzinId==first)&&leaves.TarihAraligindaGetir(null,null).Any(x=>x.IzinId==overlap),"all-person report includes cancelled and approved records");
            int crossEmployee=Employee(5,dayShift,department,position);
            int cross=Leave(crossEmployee,leaveType,yearStart.AddDays(-2),yearStart.AddDays(2));leaves.Onayla(cross,admin);
            Observe("cross_year_current",leaves.KalanYillikIzin(crossEmployee)==11,"current year consumes 3 days");
            Observe("cross_year_previous",leaves.KalanYillikIzin(crossEmployee,today.Year-1)==12,"previous year consumes 2 days");
            Observe("balance_view",Scalar("SELECT KalanYillikIzin FROM dbo.vwIzinOzet WHERE PersonelId="+crossEmployee)==11,"SQL view matches application balance");

            stage="attendance snapshot and uniqueness";
            int snapshotEmployee=Employee(6,dayShift,department,position);
            int entry=attendance.GirisYap(snapshotEmployee,admin);
            shifts.Guncelle(new Vardiya{VardiyaId=dayShift,VardiyaAdi="Changed Shift",BaslangicSaati=TimeSpan.FromHours(8),BitisSaati=TimeSpan.FromHours(15),MolaDakika=0,Pazartesi=true});
            Observe("entry_snapshot",Scalar("SELECT COUNT(*) FROM dbo.GirisKayit WHERE GirisKayitId="+entry+" AND PlanlananMolaDakika=60 AND CAST(PlanlananCikisZamani AS time)='17:00' AND VardiyaAdiKayit=N'Test Weekdays'")==1,"record keeps original shift plan after shift edit");
            Observe("duplicate_dal",Denied(()=>attendance.GirisYap(snapshotEmployee,admin)),"duplicate entry rejected by DAL");
            bool unique=false;
            try{Exec("INSERT dbo.GirisKayit (PersonelId,KayitTarihi,GirisSaati,GirisZamani) VALUES ("+snapshotEmployee+",CAST(GETDATE() AS date),'08:00',GETDATE())");}
            catch(SqlException ex){unique=ex.Number==2601||ex.Number==2627;}
            Observe("duplicate_schema",unique,"database unique index rejects duplicate daily entry");
            Observe("deactivate_open_attendance",Denied(()=>new PersonelDal().Sil(snapshotEmployee,today)),"employee with open attendance cannot be deactivated");
            attendance.CikisYap(snapshotEmployee,admin);
            new PersonelDal().Sil(snapshotEmployee,today);
            Observe("inactive_history",reports.PersonelOzetRaporu(today,today,snapshotEmployee).Count==1,"inactive employee remains in historical summary");
            Observe("inactive_entry",Denied(()=>attendance.GirisYap(snapshotEmployee,admin)),"inactive employee cannot start another entry");

            stage="absence shared rules";
            int absenceEmployee=Employee(7,dayShift,department,position);
            Observe("weekend_absence",AbsenceCount(absenceEmployee,new DateTime(2026,10,10),new DateTime(2026,10,11))==0,"weekday-only employee not absent on weekend");
            var monday=new DateTime(2026,10,5);
            Observe("shift_not_finished",AbsenceCount(absenceEmployee,monday,monday.AddHours(14))==0,"no absence before shift ends");
            Observe("finished_shift_absence",AbsenceCount(absenceEmployee,monday,monday.AddHours(16))==1,"absence after shift completion");
            Observe("report_absence",reports.DevamsizlikRaporu(monday,monday,absenceEmployee).Rows.Count==1,"absence report sees calculated absence without stored row");
            Observe("summary_absence",reports.PersonelOzetRaporu(monday,monday,absenceEmployee).Single().ToplamDevamsizlik==1,"summary uses same absence rule");
            int authorizedLeave=Leave(absenceEmployee,leaveType,monday,monday);leaves.Onayla(authorizedLeave,admin);
            Observe("leave_not_absent",AbsenceCount(absenceEmployee,monday,monday.AddHours(16))==0,"approved leave excludes absence");
            Observe("long_report",reports.PersonelOzetRaporu(yearStart,today,absenceEmployee).Count==1,"calendar recursion supports a report longer than 100 days");

            stage="concurrent entry";
            int concurrentEmployee=Employee(8,dayShift,department,position);int successes=0;
            System.Threading.Tasks.Parallel.For(0,2,i=>
            {
              using(var connection=Connect(scratch))
              {
                typeof(SqlDBConnection).GetField("_connection",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,connection);
                try{new GirisKayitDAL().GirisYap(concurrentEmployee,admin);System.Threading.Interlocked.Increment(ref successes);}
                catch(InvalidOperationException){}
              }
            });
            typeof(SqlDBConnection).GetField("_connection",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,db);
            Observe("concurrent_entry",successes==1&&Scalar("SELECT COUNT(*) FROM dbo.GirisKayit WHERE PersonelId="+concurrentEmployee)==1,"two connections result in one successful entry");

            stage="nullable date and exports";
            using(var form=new PersonnelManagement.Forms.frmPersonelYonetim())
            {
              var flags=BindingFlags.Instance|BindingFlags.NonPublic;
              form.GetType().GetMethod("FormTemizle",flags).Invoke(form,new object[]{true});
              var date=(System.Windows.Forms.DateTimePicker)form.GetType().GetField("dtpDogumTarihi",flags).GetValue(form);
              date.ShowCheckBox=true;
              var person=(Personel)form.GetType().GetMethod("FormdenPersonelOlustur",flags).Invoke(form,null);
              Observe("nullable_birth_date",!person.DogumTarihi.HasValue,"unchecked date is mapped to null");
              date.Value=new DateTime(2000,2,3);date.Checked=true;
              person=(Personel)form.GetType().GetMethod("FormdenPersonelOlustur",flags).Invoke(form,null);
              Observe("selected_birth_date",person.DogumTarihi==new DateTime(2000,2,3),"selected date is preserved");
            }
            var rows=new System.Collections.Generic.List<object[]>{new object[]{"Metin","Sayı","Formül","Boş"},new object[]{"Türkçe; \"alıntı\"\nyeni satır",12.5m,"=1+1",null}};
            RaporDisariAktarma.Kaydet(rows,args[2]+"\\fixture.xlsx");RaporDisariAktarma.Kaydet(rows,args[2]+"\\fixture.csv");
            Observe("xlsx_zip",File.ReadAllBytes(args[2]+"\\fixture.xlsx").Take(2).SequenceEqual(new byte[]{80,75}),"xlsx is an Open XML ZIP package");
            Observe("csv_quotes",File.ReadAllText(args[2]+"\\fixture.csv").Contains("\"\"alıntı\"\""),"CSV double quotes escaped");
            Observe("csv_formula",File.ReadAllText(args[2]+"\\fixture.csv").Contains("'=1+1"),"CSV text formula protected");

            stage="concurrent leave approval";
            int raceEmployee=Employee(9,dayShift,department,position);
            int[] raceIds={ManualLeave(raceEmployee,leaveType,today.AddDays(20),today.AddDays(22)),ManualLeave(raceEmployee,leaveType,today.AddDays(21),today.AddDays(23))};
            int approvalSuccesses=0;
            System.Threading.Tasks.Parallel.For(0,2,i=>
            {
                using(var connection=Connect(scratch))
                {
                    typeof(SqlDBConnection).GetField("_connection",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,connection);
                    try{new IzinDal().Onayla(raceIds[i],admin);System.Threading.Interlocked.Increment(ref approvalSuccesses);}
                    catch(InvalidOperationException){}
                }
            });
            typeof(SqlDBConnection).GetField("_connection",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,db);
            Observe("concurrent_approval",approvalSuccesses==1&&Scalar("SELECT COUNT(*) FROM dbo.Izin WHERE PersonelId="+raceEmployee+" AND Durum=N'Onaylı'")==1,"concurrent overlap approval allows only one request");
            stage="password change authorization";
            var users=new KullaniciDal();
            Observe("employee_password_change",Denied(()=>users.SifreDegistir(admin,"FixturePass123!",worker)),"non-admin cannot change another user password");
            Observe("admin_password_change",users.SifreDegistir(worker,"FixturePass123!",admin)&&Scalar("SELECT COUNT(*) FROM dbo.Kullanici WHERE KullaniciId="+worker+" AND Sifre=N'FixturePass123!'")==1,"authorized password change preserves this version password format");
            stage="restricted SQL permissions";
            Exec("CREATE USER [PersonnelManagementPermissionFixture] WITHOUT LOGIN;");
            foreach(string table in new[]{"Departman","Pozisyon","Vardiya","Personel","IzinTuru","Izin","GirisKayit","Kullanici","Devamsizlik"})
                Exec("GRANT SELECT,INSERT,UPDATE ON OBJECT::dbo.["+table+"] TO [PersonnelManagementPermissionFixture];");
            Exec("GRANT INSERT ON OBJECT::dbo.SistemLog TO [PersonnelManagementPermissionFixture];");
            foreach(string view in new[]{"vwPersonelDetay","vwGunlukOzet","vwIzinOzet","fnDevamsizlik"})
                Exec("GRANT SELECT ON OBJECT::dbo.["+view+"] TO [PersonnelManagementPermissionFixture];");
            Exec("EXECUTE AS USER='PersonnelManagementPermissionFixture';");
            try
            {
                Observe("limited_sql_privileges",Scalar("SELECT CASE WHEN IS_MEMBER('db_owner')=1 OR HAS_PERMS_BY_NAME(DB_NAME(),'DATABASE','ALTER')=1 OR HAS_PERMS_BY_NAME('dbo.Personel','OBJECT','DELETE')=1 THEN 1 ELSE 0 END")==0,"application permissions exclude owner, schema change and personnel deletion");
                int restrictedEmployee=Employee(11,dayShift,department,position);
                Observe("limited_sql_personnel",restrictedEmployee>0,"personnel insert works with only application permissions");
                int restrictedLeave=Leave(restrictedEmployee,leaveType,today.AddDays(30),today.AddDays(31));
                Observe("limited_sql_leave",new IzinDal().Onayla(restrictedLeave,admin)&&new IzinDal().Iptal(restrictedLeave,admin),"leave approval and cancellation work with application permissions");
                var restrictedAttendance=new GirisKayitDAL();
                Observe("limited_sql_attendance",restrictedAttendance.GirisYap(restrictedEmployee,admin)>0&&restrictedAttendance.CikisYap(restrictedEmployee,admin),"entry and exit work with application permissions");
                var restrictedReports=new RaporDal();
                Observe("limited_sql_reports",restrictedReports.PersonelOzetRaporu(today.AddDays(-120),today,restrictedEmployee).Count==1,"historical summary and absence function work with application permissions");
                restrictedReports.BugunOzet();
                Observe("limited_sql_dashboard",true,"dashboard works with application permissions");
                Observe("limited_sql_user_password",new KullaniciDal().SifreDegistir(worker,"FixturePass456!",admin),"authorized user update and logging work with application permissions");
            }
            finally{Exec("REVERT;");}
            stage="additional regressions";
            DateTime precise=DateTime.Today.AddHours(12).AddMilliseconds(2);
            DateTime stored;
            using(var precisionCommand=new SqlCommand("SELECT CAST(@Time AS datetime);",db))
            {
                precisionCommand.Parameters.Add("@Time",System.Data.SqlDbType.DateTime2).Value=precise;
                stored=(DateTime)precisionCommand.ExecuteScalar();
            }
            Observe("sql_datetime_precision",GirisKayitDAL.VeritabaniZamani(precise)==stored,"application timestamp agrees with SQL datetime conversion");
            var shortVisit=GirisKayitDAL.CalismaSuresi(stored,GirisKayitDAL.VeritabaniZamani(precise.AddTicks(1000)),DateTime.Today.AddHours(8),DateTime.Today.AddHours(17),30);
            Observe("rounded_immediate_checkout",shortVisit.calismaDakika==0,"SQL rounding does not turn immediate checkout into a backwards clock");
            AdditionalChecks(department,position,dayShift,admin,worker,leaveType,args);
            stage="legacy schema migration";
            LegacyMigrationChecks(args);
            stage="connection lifecycle";
            SqlDBConnection.CloseConnection();
            db=SqlDBConnection.GetConnection();
            Observe("connection_reopens_after_close",db.State==System.Data.ConnectionState.Open&&db.Database==scratch,"closed DAL connection replaced with isolated database connection");
            Console.WriteLine("CHECKS="+observations);
        }
        catch (Exception ex)
        {
            var sql = ex as SqlException;
            Console.WriteLine("TEST_RUN_ERROR | stage=" + stage + " | type=" + ex.GetType().Name
                + (sql == null ? "" : " | sql_number=" + sql.Number));
            if (ex is InvalidOperationException) Console.WriteLine("VALIDATION_ERROR=" + ex.Message);
            exit = 1;
        }
        finally
        {
            Environment.SetEnvironmentVariable("PERSONNELMANAGEMENT_V090_CONNECTION_STRING", null);
            SistemLogger.ClearKullanici();
            if (db != null) { db.Close(); db.Dispose(); }
            if (db != null) SqlConnection.ClearAllPools();
            if (created)
            {
                try
                {
                    if (!Regex.IsMatch(scratch, "^PersonnelManagementFix_[a-f0-9]{32}$"))
                        throw new InvalidOperationException("Cleanup safety guard failed.");
                    using (var master = Connect("master"))
                    using (var command = new SqlCommand("IF DB_ID(N'" + scratch + "') IS NOT NULL BEGIN ALTER DATABASE [" + scratch
                        + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [" + scratch + "]; END", master))
                        command.ExecuteNonQuery();
                    Console.WriteLine("SCRATCH_DATABASE_REMOVED=true");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SCRATCH_CLEANUP_ERROR | type=" + ex.GetType().Name + " | database=" + scratch);
                    exit = 2;
                }
            }
        }
        return exit;
    }
}
