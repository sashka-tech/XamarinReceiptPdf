using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Print;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Motion.Utils;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.IO;
using XamarinPdfPrint.Common;

namespace XamarinPdfPrint
{
    [Activity(Label = "Чек PDF", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IPermissionListener
    {
        Button btn_create_pdf;
        public static string fileName = "Чек.pdf";
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            btn_create_pdf = FindViewById<Button>(Resource.Id.btn_create_pdf);
            await Common.Common.WriteFileToStorageAsync(this, "brandon_med.otf");
            Dexter.WithActivity(this)
                .WithPermission(Manifest.Permission.WriteExternalStorage)
                .WithListener(this)
                .Check();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            Toast.MakeText(this, "Вы должны принять это разрешение", ToastLength.Short).Show();
        }

        [System.Obsolete]
        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            btn_create_pdf.Click += delegate
            {
                CreatePDFFile(XamarinPdfPrint.Common.Common.GetAppPath(this) + fileName);
            };
        }

        [Obsolete]
        private void CreatePDFFile(string v)
        {
            if (new Java.IO.File(v).Exists())
                new Java.IO.File(v).Delete();
            try
            {
                Document document = new Document();
                PdfWriter.GetInstance(document, new FileStream(v, FileMode.Create));
                document.Open();
                document.SetPageSize(PageSize.A4);
                document.AddCreationDate();
                document.AddAuthor("AKasatkin");
                document.AddCreator("Александр Касаткин");

                Color colorAccent = new Color(0, 153, 204, 255);
                float fontSize = 20.0f, valueFontSize = 26.0f;
                BaseFont fontName = BaseFont.CreateFont(XamarinPdfPrint.Common.Common.GetFilePath(this, "brandon_med.otf"),
                    "UTF-8",
                    BaseFont.EMBEDDED);

                Font titleFont = new Font(fontName, 36.0f, Font.NORMAL, Color.BLACK);
                AddNewItem(document, "Receipt", Element.ALIGN_CENTER, titleFont);

                Font orderNumberFont = new Font(fontName, fontSize, Font.NORMAL, colorAccent);
                AddNewItem(document, "Number:", Element.ALIGN_CENTER, orderNumberFont);

                Font orderNumberValueFont = new Font(fontName, valueFontSize, Font.NORMAL, Color.BLACK);
                AddNewItem(document, "#00001", Element.ALIGN_CENTER, orderNumberValueFont);

                AddLineSeparator(document);

                AddNewItem(document, "Time", Element.ALIGN_LEFT, orderNumberFont);
                AddNewItem(document, "12:00", Element.ALIGN_LEFT, orderNumberValueFont);

                AddLineSeparator(document);

                AddNewItem(document, "Date", Element.ALIGN_LEFT, orderNumberFont);
                AddNewItem(document, "11.04.2021", Element.ALIGN_LEFT, orderNumberValueFont);

                AddLineSeparator(document);

                AddNewItem(document, "Buyer:", Element.ALIGN_LEFT, orderNumberFont);
                AddNewItem(document, "A. S. Kasatkin", Element.ALIGN_LEFT, orderNumberValueFont);

                AddLineSeparator(document);

                AddLineSpace(document);
                AddNewItem(document, "Products", Element.ALIGN_CENTER, titleFont);
                AddLineSeparator(document);
                
                AddNewItemWithLeftAndRight(document, "Sneakers Nike", "(3.0%)", titleFont, orderNumberFont);
                AddNewItemWithLeftAndRight(document, "Price:", "4999.99", titleFont, orderNumberValueFont);
                AddLineSeparator(document);

                AddNewItemWithLeftAndRight(document, "Man's T-shirt Adidas", "(3.0%)", titleFont, orderNumberFont);
                AddNewItemWithLeftAndRight(document, "Price:", "1999.99", titleFont, orderNumberValueFont);
                AddLineSeparator(document);

                AddNewItemWithLeftAndRight(document, "Backpack Nike", "(3.0%)", titleFont, orderNumberFont);
                AddNewItemWithLeftAndRight(document, "Price:", "2999.99", titleFont, orderNumberValueFont);
                AddLineSeparator(document);

                AddNewItemWithLeftAndRight(document, "Men's pants Fila", "(3.0%)", titleFont, orderNumberFont);
                AddNewItemWithLeftAndRight(document, "Price:", "3199.99", titleFont, orderNumberValueFont);
                AddLineSeparator(document);

                AddNewItemWithLeftAndRight(document, "Men's jacket OUTVENTURE", "(3.0%)", titleFont, orderNumberFont);
                AddNewItemWithLeftAndRight(document, "Price:", "5999.99", titleFont, orderNumberValueFont);
                AddLineSeparator(document);

                AddNewItemWithLeftAndRight(document, "Men socks Nike", "(3.0%)", titleFont, orderNumberFont);
                AddNewItemWithLeftAndRight(document, "Price:", "1999.99", titleFont, orderNumberValueFont);
                AddLineSeparator(document);


                AddLineSpace(document);
                AddLineSpace(document);

                AddNewItemWithLeftAndRight(document, "Total:", "21 199,94", titleFont, orderNumberValueFont);

                document.Close();
                Toast.MakeText(this, "PDF-чек успешно создан", ToastLength.Short).Show();

                PrintPDF();
            }
            catch (FileNotFoundException e)
            {
                Log.Debug("AKasatkin", "" + e.Message);
            }
            catch (DocumentException e)
            {
                Log.Debug("AKasatkin", "" + e.Message);
            }
            catch (IOException e) 
            {
                Log.Debug("AKasatkin", "" + e.Message);
            }
        }

        [Obsolete]
        private void PrintPDF()
        {
            PrintManager printManager = (PrintManager)GetSystemService(Context.PrintService);
            try
            {
                PrintDocumentAdapter adapter = new PrintPDFAdapter(this, Common.Common.GetAppPath(this) + fileName);
                printManager.Print("Document", adapter, new PrintAttributes.Builder().Build());
            }catch(Exception e)
            {
                Log.Error("AKasatkin", "" + e.Message);
            }
        }

        private void AddNewItemWithLeftAndRight(Document document, string leftText, string rightText, Font leftFont, Font rightFont)
        {
            Chunk chunkLeft = new Chunk(leftText, leftFont);
            Chunk chunkRight = new Chunk(rightText, rightFont);
            Paragraph p = new Paragraph(chunkLeft);
            p.Add(new Chunk(new VerticalPositionMark()));
            p.Add(chunkRight);
            document.Add(p);
        }

        private void AddLineSeparator(Document document)
        {
            LineSeparator lineSeparator = new LineSeparator();
            lineSeparator.LineColor = new Color(0, 0, 0, 68);
            AddLineSpace(document);
            document.Add(new Chunk(lineSeparator));
            AddLineSpace(document);
        }

        private void AddLineSpace(Document document)
        {
            document.Add(new Paragraph(""));
        }

        private void AddNewItem(Document document, string text, int align, Font font)
        {
            Chunk chunk = new Chunk(text, font);
            Paragraph p = new Paragraph(chunk);
            p.Alignment = align;
            document.Add(p);

        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {
            throw new System.NotImplementedException();
        }
    }
}