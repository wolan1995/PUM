using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;

namespace Zadanie_2
{
    [Activity(Label = "Dzwon, SMSuj, MAILuj", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button actionButton;
        private EditText addressText;
        private EditText messageText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView (Resource.Layout.Main);

            actionButton = FindViewById<Button>(Resource.Id.actionButton);
            addressText = FindViewById<EditText>(Resource.Id.addres);
            messageText = FindViewById<EditText>(Resource.Id.message);



            actionButton.Click += (object sender, EventArgs e) =>
            {
                int n;
                bool isNumeric = int.TryParse(addressText.Text, out n);

                if (isNumeric && String.IsNullOrWhiteSpace(messageText.Text))
                {
                    var callIntent = new Intent(Intent.ActionCall);
                    callIntent.SetData(Android.Net.Uri.Parse("tel:" + addressText.Text));
                    StartActivity(callIntent);
                }

                else if (isNumeric && !String.IsNullOrWhiteSpace(messageText.Text))
                {
                    var smsUri = Android.Net.Uri.Parse("smsto:" + addressText.Text);
                    var smsIntent = new Intent(Intent.ActionSendto, smsUri);
                    smsIntent.PutExtra("sms_body", messageText.Text);
                    StartActivity(smsIntent);
                }
                else if (String.IsNullOrWhiteSpace(messageText.Text) && String.IsNullOrWhiteSpace(addressText.Text))
                {
                    Toast.MakeText(this, "Nie podales zadnych danych", ToastLength.Long).Show();
                }
                else
                {
                    var email = new Intent(Android.Content.Intent.ActionSend);
                    email.SetType("message/rfc822");

                    email.PutExtra(Android.Content.Intent.ExtraEmail, addressText.Text);
                    email.PutExtra(Android.Content.Intent.ExtraText, messageText.Text);


                    try
                    {
                        StartActivity(email);
                    }
                    catch (Android.Content.ActivityNotFoundException ex)
                    {

                            Toast.MakeText(this, "Brak aplikacji do wysylania e-maili!", ToastLength.Long).Show();

                    }
                }
            };



        }


    }
}

