using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Android.Graphics;

namespace App2
{
    [Activity(Label = "App2", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int _counter = 0;
        string _imie;
        string _nazwisko;
        private Button clickButton;
        private EditText text;
        private EditText imie;
        private EditText nazwisko;
        private EditText link;
        private Button downloadButton;
        private ImageView image;
        String urlG;
        private WebClient webClient;
        private TextView infoLabel;
        private ProgressBar downloadProgress;
        private ImageView imageview;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt("counter", _counter);
            outState.PutString("imie", _imie);
            outState.PutString("nazwisko", _nazwisko);

            base.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedState)
        {
            base.OnRestoreInstanceState(savedState);
            _counter = savedState.GetInt("counter");
            _imie = savedState.GetString("imie");
            _nazwisko = savedState.GetString("nazwisko");
            load();

        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            if (bundle != null)
            {
                _counter = bundle.GetInt("counter", 0);
                _imie = bundle.GetString("imie", "");
                _nazwisko = bundle.GetString("nazwisko", "");
            }



            this.clickButton = FindViewById<Button>(Resource.Id.CounterButton);
            this.downloadButton = FindViewById<Button>(Resource.Id.downloadButton);

            var text = FindViewById<EditText>(Resource.Id.CounterText);
            var imie = FindViewById<EditText>(Resource.Id.imieText);
            var nazwisko = FindViewById<EditText>(Resource.Id.nazwiskoText);
            var urlText = FindViewById<EditText>(Resource.Id.linkText);
            this.infoLabel = FindViewById<TextView>(Resource.Id.textView4);
            this.imageview = FindViewById<ImageView>(Resource.Id.imageView1);
            this.downloadProgress = FindViewById<ProgressBar>(Resource.Id.progressBar);

            urlText.Text = "http://photojournal.jpl.nasa.gov/jpeg/PIA15416.jpg";
            urlG = urlText.Text;



            text.Text = _counter.ToString();
            imie.Text = _imie;
            nazwisko.Text = _nazwisko;

            downloadButton.Click += downloadAsync;

            clickButton.Click += (object sender, EventArgs e) =>
            {
                _counter++;
                text.Text = _counter.ToString();
            };


        
           // var url = new Uri(urlText.Text);
            
        }

        async void downloadAsync(object sender, System.EventArgs ea)
        {
            webClient = new WebClient();
            var url = new Uri("http://photojournal.jpl.nasa.gov/jpeg/PIA15416.jpg");
            byte[] bytes = null;


            webClient.DownloadProgressChanged += HandleDownloadProgressChanged;

            this.downloadButton.Text = "Przerwij";
            this.downloadButton.Click -= downloadAsync;
            this.downloadButton.Click += cancelDownload;
            infoLabel.Text = "Trwa Pobieranie";
            try
            {
                bytes = await webClient.DownloadDataTaskAsync(url);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Przerwano!");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                infoLabel.Text = "Kliknij aby pobrac";


                this.downloadButton.Click -= cancelDownload;
                this.downloadButton.Click += downloadAsync;
                this.downloadButton.Text = "Download";
                this.downloadProgress.Progress = 0;
                return;
            }
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string localFilename = "downloaded.png";
            string localPath = System.IO.Path.Combine(documentsPath, localFilename);
            infoLabel.Text = "Pobrano";


            FileStream fs = new FileStream(localPath, FileMode.OpenOrCreate);
            await fs.WriteAsync(bytes, 0, bytes.Length);

            Console.WriteLine("localPath:" + localPath);
            fs.Close();
            
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            await BitmapFactory.DecodeFileAsync(localPath, options);

            options.InSampleSize = options.OutWidth > options.OutHeight ? options.OutHeight / imageview.Height : options.OutWidth / imageview.Width;
            options.InJustDecodeBounds = false;

            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(localPath, options);


            imageview.SetImageBitmap(bitmap);

            infoLabel.Text = "Kliknij aby sciagnac obrazek";


            this.downloadButton.Click -= cancelDownload;
            this.downloadButton.Click += downloadAsync;
            this.downloadButton.Text = "Sciagnij";
            this.downloadProgress.Progress = 0;
        }

        void HandleDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.downloadProgress.Progress = e.ProgressPercentage;
        }

        void cancelDownload(object sender, System.EventArgs ea)
        {
            Console.WriteLine("Przerwano!");
            if (webClient != null)
                webClient.CancelAsync();

            webClient.DownloadProgressChanged -= HandleDownloadProgressChanged;

            this.downloadButton.Click -= cancelDownload;
            this.downloadButton.Click += downloadAsync;
            this.downloadButton.Text = "Download";
            this.downloadProgress.Progress = 0;

            infoLabel.Text = "Kliknij aby pobrac";
        }

        private async Task load()
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string localFilename = "downloaded.png";
            string localPath = System.IO.Path.Combine(documentsPath, localFilename);

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            await BitmapFactory.DecodeFileAsync(localPath, options);

            options.InSampleSize = options.OutWidth > options.OutHeight ? options.OutHeight / imageview.Height : options.OutWidth / imageview.Width;
            options.InJustDecodeBounds = false;

            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(localPath, options);


            imageview.SetImageBitmap(bitmap);

        }
    }
}

