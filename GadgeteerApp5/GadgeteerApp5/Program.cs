using System.Net;
using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using Microsoft.SPOT.Net.Security;

using NetworkModule = Gadgeteer.Modules.Module.NetworkModule;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using System.IO;
using System.Text;
using GHI.Networking;





namespace GadgeteerApp5
{
    public partial class Program
    {

        Window mainWindow;
        Utente utenteCorrente;
        String userStr;
        int ntoccou = 0;
        int ntoccop = 0;
        String pinStr;
        int timeout_acquisisci = 0;
        Canvas canvas = new Canvas();
        Bitmap currentBitmapData;
        public static Bitmap foto1 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.foto1), Bitmap.BitmapImageType.Bmp);
        Image foto = new Image(foto1);

        public static Bitmap header1 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.auth), Bitmap.BitmapImageType.Bmp);
        Image header = new Image(header1);
        
        public static Bitmap istruzioniFoto1 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.istruzioni), Bitmap.BitmapImageType.Bmp);
        public static Bitmap istrKey = new Bitmap(Resources.GetBytes(Resources.BinaryResources.istrKey), Bitmap.BitmapImageType.Bmp);
        public static Bitmap istruzioniSD = new Bitmap(Resources.GetBytes(Resources.BinaryResources.istrSD), Bitmap.BitmapImageType.Bmp);
        public static Bitmap authTry = new Bitmap(Resources.GetBytes(Resources.BinaryResources.Tryauth), Bitmap.BitmapImageType.Bmp);
        public static Bitmap invalidPin = new Bitmap(Resources.GetBytes(Resources.BinaryResources.invalidPass), Bitmap.BitmapImageType.Bmp);
        public static Bitmap invalidUser = new Bitmap(Resources.GetBytes(Resources.BinaryResources.invalidUser), Bitmap.BitmapImageType.Bmp);


        public static Bitmap attendi1 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.attendi), Bitmap.BitmapImageType.Bmp);
        Image attendi = new Image(attendi1);

        public static Bitmap sd1 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.sdcard), Bitmap.BitmapImageType.Bmp);
        Image sdcard = new Image(sd1);

        public static Bitmap keybIcon = new Bitmap(Resources.GetBytes(Resources.BinaryResources.keyboard), Bitmap.BitmapImageType.Bmp);
        Image keyboardIcona = new Image(keybIcon);

        public static Bitmap keyboard1 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.keyNum), Bitmap.BitmapImageType.Bmp);
        Image numericKey = new Image(keyboard1);

        
        Text username = new Text(Resources.GetFont(Resources.FontResources.NinaB), "Username:  ");
        Text pin = new Text(Resources.GetFont(Resources.FontResources.NinaB), "PIN:  ");
        Text usernameValue= new Text (Resources.GetFont(Resources.FontResources.NinaB), "");
        Text pinValue = new Text(Resources.GetFont(Resources.FontResources.NinaB), "");

        
        EthernetBuiltIn netif;
        GT.Timer timer;
        //public static Bitmap keyboardSymbol = new Bitmap(Resources.GetBytes(Resources.BinaryResources.keyboard), Bitmap.BitmapImageType.Bmp);
        //Image keyboard1 = new Image(keyboard);

        //public static Bitmap keyboardUpper = new Bitmap(Resources.GetBytes(Resources.BinaryResources.keyboard), Bitmap.BitmapImageType.Bmp);
        //Image keyboard1 = new Image(keyboard);

        
        
        
// This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {

            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/
            
            timer = new GT.Timer(15000); 
            timer.Tick += timer_Tick;
            
            Debug.Print("Program Started");
            mainWindow = displayTE35.WPFWindow;
            mainWindow.Child = canvas;
            //InitEthernet();
            SetupDisplay();
            camera.BitmapStreamed += camera_BitmapStreamed;
            netif = new EthernetBuiltIn();
            netif.Open();
            netif.EnableStaticIP("169.254.202.65", "255.255.255.0", "169.254.202.73");
                

        }


        public void ConvertToFile(Bitmap bitmap, byte[] outputBuffer)
        {
            var inputBuffer = bitmap.GetBitmap();
            var width = (uint)bitmap.Width;
            var height = (uint)bitmap.Height;

            if (outputBuffer.Length != width * height * 3 + 54) throw new System.ArgumentException("outputBuffer.Length must be width * height * 3 + 54.", "outputSize");

            System.Array.Clear(outputBuffer, 0, outputBuffer.Length);

            Microsoft.SPOT.Hardware.Utility.InsertValueIntoArray(outputBuffer, 0, 2, 19778);
            Microsoft.SPOT.Hardware.Utility.InsertValueIntoArray(outputBuffer, 2, 4, width * height * 3 + 54);
            Microsoft.SPOT.Hardware.Utility.InsertValueIntoArray(outputBuffer, 10, 4, 54);
            Microsoft.SPOT.Hardware.Utility.InsertValueIntoArray(outputBuffer, 14, 4, 40);
            Microsoft.SPOT.Hardware.Utility.InsertValueIntoArray(outputBuffer, 18, 4, width);
            Microsoft.SPOT.Hardware.Utility.InsertValueIntoArray(outputBuffer, 22, 4, (uint)(-height));
            Microsoft.SPOT.Hardware.Utility.InsertValueIntoArray(outputBuffer, 26, 2, 1);
            Microsoft.SPOT.Hardware.Utility.InsertValueIntoArray(outputBuffer, 28, 2, 24);

            for (int i = 0, j = 54; i < width * height * 4; i += 4, j += 3)
            {
                outputBuffer[j + 0] = inputBuffer[i + 2];
                outputBuffer[j + 1] = inputBuffer[i + 1];
                outputBuffer[j + 2] = inputBuffer[i + 0];
            }
        }

        void display_Standby_TouchDown(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            if (displayTE35.BacklightEnabled == false)
            {
                mainWindow.TouchDown -= display_Standby_TouchDown;
                displayTE35.BacklightEnabled = true;
                SetupDisplay();
            }
        }

        void timer_Tick(GT.Timer timer)
        {
            HideButtons();
            foto.TouchDown -= new Microsoft.SPOT.Input.TouchEventHandler(getImage_TouchDown);
            sdcard.TouchDown -= sdcard_TouchDown;
            keyboardIcona.TouchDown -= keyboardIcona_TouchDown;
            mainWindow.TouchDown += display_Standby_TouchDown;
            button.TurnLedOn();
            button.ButtonPressed += button_ButtonPressed;
            displayTE35.BacklightEnabled = false;
            
        }

        void button_ButtonPressed(Button sender, Button.ButtonState state)
        {
            if (displayTE35.BacklightEnabled == false){
                mainWindow.TouchDown -= display_Standby_TouchDown;
                displayTE35.BacklightEnabled = true;
                Thread.Sleep(1000);
                SetupDisplay();
                button.TurnLedOff();
            }
            else {
                //significa che è in streaming
                if (timeout_acquisisci != 0)
                {
                    camera.StopStreaming();
                    multicolorLED.TurnOff();
                    timeout_acquisisci = 0;
                    HideButtons();
                }
                
                button.ButtonPressed -= button_ButtonPressed;
                SetupDisplay();
            }
        }

        private void camera_BitmapStreamed(Camera sender, Bitmap e)
        {
            button.ButtonPressed += button_ButtonPressed;
           
            if (timeout_acquisisci != 13)
            {
                if (timeout_acquisisci == 8)
                    multicolorLED.TurnBlue();
                if (timeout_acquisisci == 11)
                    multicolorLED.TurnWhite();

                this.displayTE35.SimpleGraphics.DisplayImage(e, 0, 0);
                currentBitmapData = e;
                timeout_acquisisci++;
            }
            else
            {
                currentBitmapData = e;
                camera.StopStreaming();
                timeout_acquisisci = 0;
                multicolorLED.TurnOff();

                this.displayTE35.SimpleGraphics.DisplayImage(currentBitmapData, 0, 0);
                button.ButtonPressed -= button_ButtonPressed;


                
                int fileSize = currentBitmapData.Width * currentBitmapData.Height * 3 + 54;
                Microsoft.SPOT.Hardware.LargeBuffer data = new Microsoft.SPOT.Hardware.LargeBuffer(fileSize);
                ConvertToFile(currentBitmapData, data.Bytes);

                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://169.254.202.73/api/Img");
                //request.Method = "POST";
                //request.ContentLength = data.Bytes.Length;

                //using (var requestStream = request.GetRequestStream()) {
                //    requestStream.Write(data.Bytes, 0, data.Bytes.Length);                
                //}
                //var response = request.GetResponse().GetResponseStream();


                //if (response.CanRead && response.Length > 1) {
                //    byte[] buffer = new byte[response.Length];
                //    if (response.CanSeek)
                //        response.Position = 0;
                //    response.Read(buffer, 0, (int)response.Length);
                //    var responseTxt = new String(Encoding.UTF8.GetChars(buffer));
                //    Debug.Print("s Res:" + responseTxt);
                
                
                //}


                var request = BuildServerRequest(@"http://169.254.202.73/api/Img", data.Bytes, "Image", "image/bmp", "image.bmp");
                var response = request.GetResponse().GetResponseStream();
               
                if (response.CanRead && response.Length > 1)
                {
                    byte[] buffer = null;
                    buffer = new byte[response.Length];
                    if (response.CanSeek)
                        response.Position = 0;

                    response.Read(buffer, 0, (int)response.Length);
                    var responseText = new String(Encoding.UTF8.GetChars(buffer));
                    Debug.Print("Server Response: " + responseText);
                    response.Close();
                }



                ////POSTContent content = POSTContent.CreateTextBasedContent();
                //// Debug.Print(content.ToString());
                //POSTContent content = Gadgeteer.Networking.POSTContent.CreateBinaryBasedContent(data.Bytes);
                ////HttpRequest request;
                //request = HttpHelper.CreateHttpPostRequest(@"http://169.254.202.73/api/Img?lenght="+data.Bytes.Length, content, "multipart/from-data");
                
                //request.ResponseReceived += request_ResponseReceived_IMG;
                ////Send the request
                //request.SendRequest();

                

                












                //salvare immagine su sd
                //if (sdCard.IsCardInserted && sdCard.IsCardMounted)
                //{
                //    GT.StorageDevice storage = sdCard.StorageDevice;
                    
                //    int fileSize1 = currentBitmapData.Width * currentBitmapData.Height * 3 + 54;
                //    Microsoft.SPOT.Hardware.LargeBuffer data1 = new Microsoft.SPOT.Hardware.LargeBuffer(fileSize1);
                //    ConvertToFile(currentBitmapData, data1.Bytes);
                //    string rootdirectory = sdCard.StorageDevice.RootDirectory;
                //    FileStream FileHandle = new FileStream(rootdirectory + @"\picture.bmp", FileMode.Create);
                //    FileHandle.Write(data1.Bytes, 0, fileSize1);
                //    FileHandle.Close();
                //    timeout_acquisisci = 0;
                //    multicolorLED.TurnOff();
                //}
        
            }
        
        }

        private void request_ResponseReceived_IMG(HttpRequest sender, HttpResponse response)
        {
            
            Debug.Print(response.StatusCode);
            Debug.Print(response.ContentType);

            if (response.StatusCode == "201")
            {
                
                var reader = new StreamReader(response.Stream);
                string responseBody = reader.ReadToEnd();
                if (responseBody.Length > 0)
                    responseBody = responseBody.Substring(0, responseBody.Length - 1);
                Debug.Print(responseBody);


            }
            else
            {

                var reader = new StreamReader(response.Stream);
                string responseBody = reader.ReadToEnd();
                if (responseBody.Length > 0)
                    responseBody = responseBody.Substring(0, responseBody.Length - 1);

                Debug.Print(responseBody);


            }
            sender.ResponseReceived -= request_ResponseReceived_IMG;
        }

        void HideButtons()
        {
            canvas.Children.Clear();
            canvas.Children.Add(attendi);
            mainWindow.Invalidate();
        }

        void keyboardIcona_TouchDown(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            timer.Stop();
            button.TurnLedOn();
            button.ButtonPressed += button_ButtonPressed;
            HideButtons();
            displayTE35.SimpleGraphics.DisplayImage(istrKey, 0, 0);
            Thread.Sleep(2000);
            canvas.Children.Clear();
            canvas.Children.Add(numericKey);
            canvas.Children.Add(username);
            Canvas.SetBottom(username, 15);
            Canvas.SetLeft(username, 0);
            canvas.Children.Add(usernameValue);
            Canvas.SetBottom(usernameValue, 15);
            Canvas.SetLeft(usernameValue, 80);


            canvas.Children.Add(pin);
            Canvas.SetBottom(pin, 15);
            Canvas.SetLeft(pin, 200);
            canvas.Children.Add(pinValue);
            Canvas.SetBottom(pinValue, 15);
            Canvas.SetLeft(pinValue, 235);

            numericKey.TouchDown += numericKey_TouchDown;

        }

        void numericKey_TouchDown(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            
            // cancella tutto con delete e con invio contatta il server
            int y,x;
            e.GetPosition(mainWindow,0,out x,out y);
            char carattere = riconosci_tocco_tastiera(x, y);
            
            if (carattere == 'v')
                return;

            if (ntoccou == 10 && carattere != 'e') {
                return;
            }

            if (carattere == 'e' && ntoccou == 10 )
            {
                button.ButtonPressed -= button_ButtonPressed;
                Debug.Print("Contatta server");

                utenteCorrente.id = userStr.ToString();
                utenteCorrente.user = userStr.ToString();
                utenteCorrente.pin = pinStr.ToString();

                //Debug.Print("Net AVAIL: " + netif.NetworkIsAvailable);
                //Debug.Print("IP ADD: "+netif.IPAddress);
                //Debug.Print("DHCP EN: " + netif.IsDhcpEnabled);
                //Debug.Print("SUBNET MASK:" + netif.SubnetMask);
                //Debug.Print("GATEWAY" + netif.GatewayAddress);
                //Debug.Print("PIN "+ pinStr.ToString());
                Hashtable ht = new Hashtable();
                ht.Add("Id", utenteCorrente.id);
                ht.Add("User",utenteCorrente.user);
                ht.Add("Pwd",utenteCorrente.pin);
  
                POSTContent content = Gadgeteer.Networking.POSTContent.CreateWebFormData(ht);
                var request = HttpHelper.CreateHttpPostRequest(@"http://169.254.202.73/api/Utente/", content, "application/x-www-form-urlencoded");
                request.ResponseReceived += request_ResponseReceived_USER_PIN;

                request.SendRequest();
                HideButtons();
                return;
            }
            else if (carattere == 'e')
                return;

            if (ntoccou == 0 && carattere == 'c')
            {
                SetupDisplay();
            }

            if (ntoccou >= 0 && ntoccou <= 6){
                if (carattere == 'c' && ntoccou != 0){
                    if (ntoccou == 1)
                    {
                        userStr = "";
                        ntoccou = 0;
                        usernameValue.TextContent = "";
                        return;
                    }
                    else
                    {
                        userStr = userStr.Substring(0, ntoccou - 1);
                        ntoccou--;
                        usernameValue.TextContent = userStr;
                        return;
                    }
                }
                else if(carattere!='c' && ntoccou != 6){
                    userStr += carattere.ToString();
                    ntoccou++;
                    usernameValue.TextContent = userStr;
                    return;
                }
                    
            }

            if (ntoccou >= 6 && ntoccou <= 10){
                if (carattere == 'c' && ntoccou != 6)
                {
                    if (ntoccou == 7)
                    {
                        pinStr = "";
                        pinValue.TextContent = "";
                        ntoccou = 6;
                        ntoccop = 0;
                        return;
                    }
                    else {
                        pinStr = pinStr.Substring(0, ntoccop - 1);
                        string tmp = "";
                        for (int j = 0; j < pinStr.Length; j++)
                            tmp += '*';
                        pinValue.TextContent = tmp;
                        ntoccop--;
                        ntoccou--;
                        return;
                    }
                }
                else if (carattere != 'c' && ntoccou != 10)
                {
                    pinStr += carattere.ToString();
                    pinValue.TextContent += '*';
                    ntoccou++;
                    ntoccop++;
                    return;
                }
            }
            
        }

        void SetupDisplay()
        {
            button.TurnLedOff();
            numericKey.TouchDown -= numericKey_TouchDown;
            foto.TouchDown -= new Microsoft.SPOT.Input.TouchEventHandler(getImage_TouchDown);
            sdcard.TouchDown -= sdcard_TouchDown;
            keyboardIcona.TouchDown -= keyboardIcona_TouchDown;
            utenteCorrente = new Utente();
            canvas.Children.Clear();
            userStr = "";
            pinStr = "";
            ntoccou = 0;
            ntoccop = 0;
            usernameValue.TextContent = "";
            pinValue.TextContent = "";


            canvas.SetMargin(5);
            canvas.Children.Add(header);
            
            canvas.Children.Add(foto);
            Canvas.SetBottom(foto, 25);
            Canvas.SetLeft(foto, 10);
            canvas.Children.Add(sdcard);
            Canvas.SetLeft(sdcard, 110);
            Canvas.SetBottom(sdcard, 25);
            canvas.Children.Add(keyboardIcona);
            Canvas.SetLeft(keyboardIcona, 210);
            Canvas.SetBottom(keyboardIcona, 25);

            
            foto.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(getImage_TouchDown);
            sdcard.TouchDown += sdcard_TouchDown;
            keyboardIcona.TouchDown += keyboardIcona_TouchDown;
            timer.Restart();

        }

        void sdcard_TouchDown(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            timer.Stop();
            button.TurnLedOn();
            button.ButtonPressed += button_ButtonPressed;
            HideButtons();
            displayTE35.SimpleGraphics.DisplayImage(istruzioniSD, 0, 0);
            Thread.Sleep(5000);
            if (sdCard.IsCardInserted && sdCard.IsCardMounted)
            {
                button.ButtonPressed -= button_ButtonPressed;
                button.TurnLedOff();
                HideButtons();
                displayTE35.SimpleGraphics.DisplayImage(authTry, 0, 0);
                string rootdirectory = sdCard.StorageDevice.RootDirectory;
                rootdirectory += @"\accessDoor";
                FileStream fileStream = new FileStream(rootdirectory + @"\authDoor.txt", FileMode.Open);
                byte[] data = new byte[fileStream.Length];
                Debug.Print(fileStream.Length + "");
                fileStream.Read(data, 0, data.Length);
                fileStream.Close();
                Debug.Print("data: " + data);
                string text = new string(Encoding.UTF8.GetChars(data));
                Debug.Print("text: " + text);
                userStr = text.Substring(0, 6);
                Debug.Print(userStr);
                pinStr = text.Substring(8, 4);
                Debug.Print(pinStr);
                

                button.ButtonPressed -= button_ButtonPressed;
                Debug.Print("Contatta server");


                utenteCorrente.id = userStr.ToString();
                utenteCorrente.user = userStr.ToString();
                utenteCorrente.pin = pinStr.ToString();

                //Debug.Print("Net AVAIL: " + netif.NetworkIsAvailable);
                //Debug.Print("IP ADD: "+netif.IPAddress);
                //Debug.Print("DHCP EN: " + netif.IsDhcpEnabled);
                //Debug.Print("SUBNET MASK:" + netif.SubnetMask);
                //Debug.Print("GATEWAY" + netif.GatewayAddress);
                //Debug.Print("PIN "+ pinStr.ToString());
                Hashtable ht = new Hashtable();
                ht.Add("Id", utenteCorrente.id);
                ht.Add("User", utenteCorrente.user);
                ht.Add("Pwd", utenteCorrente.pin);

                POSTContent content = Gadgeteer.Networking.POSTContent.CreateWebFormData(ht);
                var request = HttpHelper.CreateHttpPostRequest(@"http://169.254.202.73/api/Utente/", content, "application/x-www-form-urlencoded");
                request.ResponseReceived += request_ResponseReceived_USER_PIN;

                request.SendRequest();
                HideButtons();

            }

            SetupDisplay();

        }

        void getImage_TouchDown(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            timer.Stop();
            button.TurnLedOn();
            button.ButtonPressed += button_ButtonPressed;
            HideButtons();
            displayTE35.SimpleGraphics.DisplayImage(istruzioniFoto1,0,0);

            Thread.Sleep(3000);
            camera.CurrentPictureResolution = Camera.PictureResolution.Resolution320x240;

            if (camera.CameraReady)
            {
                camera.StartStreaming();
            }
            else if(!camera.CameraReady)
            {
                SetupDisplay();
            }
        }

        char riconosci_tocco_tastiera(int x, int y) {

            if (x >= 0 && x < 76)
            {
                if (y < 57)
                {
                    return '1';
                }
                else if (y >= 57 && y < 108)
                {
                    return '5';
                }
                else if (y >= 108 && y < 159)
                {
                    return 'c';
                }
            }
            else if (x >= 76 && x < 154)
            {
                if (y < 57)
                {
                    return '2';
                }
                else if (y >= 57 && y < 108)
                {
                    return '6';
                }
                else if(y >= 108 && y < 159)
                {
                    return '9';
                }
            }
            else if (x >= 154 && x < 231)
            {
                if (y < 57)
                {
                    return '3';
                }
                else if (y >= 57 && y < 108)
                {
                    return '7';
                }
                else if (y >= 108 && y < 159)
                {
                    return '0';
                }
            }
            else if (x >= 231 && x < 306)
            {
                if (y < 57)
                {
                    return '4';
                }
                else if (y >= 57 && y < 108)
                {
                    return '8';
                }
                else if(y >= 108 && y < 159)
                {
                    return 'e';
                }
            }

            return 'v';
        }

        void InitEthernet()
        {
            netif = new EthernetBuiltIn();
            netif.Open();
            netif.EnableDhcp();
            netif.EnableDynamicDns();

            while (netif.IPAddress == "0.0.0.0")
            {
                Debug.Print("Waiting for DHCP");
                Thread.Sleep(250);
            }
        }

        void request_ResponseReceived_USER_PIN(HttpRequest sender, HttpResponse response)
        {
            
            Debug.Print(response.Text);
            Debug.Print(sender.ToString());
            //QUI dobbiamo vedere come riconoscere lo stato se corretto o meno, modificare tutto
            Debug.Print(response.StatusCode);
            if (response.StatusCode == "201")
            {

                System.Xml.XmlReader reader = System.Xml.XmlReader.Create(response.Stream);
                while (reader.Read()) {
                    Debug.Print(reader.ReadElementString());
                
                }
                

            }
            else if (response.StatusCode == "403")
            {
                displayTE35.SimpleGraphics.DisplayImage(invalidUser, 0, 0);
                Thread.Sleep(2000);
                SetupDisplay();

            }
            else if (response.StatusCode == "401")
            {

                displayTE35.SimpleGraphics.DisplayImage(invalidPin, 0, 0);
                Thread.Sleep(2000);
                SetupDisplay();
                Debug.Print(response.Text);

            }
            else {
                System.Xml.XmlReader reader = System.Xml.XmlReader.Create(response.Stream);
                reader.ReadToFollowing("string");
                Debug.Print("qua\n");
                string s = reader.Value;
                Debug.Print(s);            
            }

            sender.ResponseReceived -= request_ResponseReceived_USER_PIN;
        }

        public static HttpWebRequest BuildServerRequest(string url,  byte[] fileBytes, string fileParamName, string fileContentType, string fileName)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString();
            byte[] boundarybytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");

            string fileHeader = "";
            byte[] fileHeaderBytes = null;
            byte[] fileTrailer = null;
            
            ArrayList formItems = new ArrayList();
            int requestLen = 0;

            if (fileBytes != null && fileBytes.Length > 0)
            {
                fileHeader = "Content-Disposition: form-data; name=\"" + fileParamName + "\"; filename=\"" + fileName + "\"\r\nContent-Type: " + fileContentType + "\r\n\r\n";
                fileHeaderBytes = System.Text.Encoding.UTF8.GetBytes(fileHeader);
                //fileTrailer = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                fileTrailer = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                requestLen += boundarybytes.Length + fileHeaderBytes.Length + fileTrailer.Length + fileBytes.Length;
            }

            //var eof = System.Text.Encoding.UTF8.GetBytes();
            var eof = System.Text.Encoding.UTF8.GetBytes("\r\n");
            HttpWebRequest request = null;
            Stream requestStream = null;
            
            request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.HttpsAuthentCerts = null;
            

            if (request.Headers["Content-Type"] != null)
            {
                request.Headers.Remove("Content-Type");
            }
        
            request.ContentType = "multipart/form-data; boundary=" + boundary;

            request.Method = "POST";
            // request.KeepAlive = true;
            request.ContentLength = requestLen;
            request.Expect = string.Empty;


            try
            {
                requestStream = request.GetRequestStream();

                foreach (var item in formItems)
                {
                    requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                    var bytes = item as byte[];
                    requestStream.Write(bytes, 0, bytes.Length);
                }



                if (fileBytes != null && fileBytes.Length > 0)
                {
                    requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                    requestStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
                    int chunkSize = 1427; // any larger will cause an SSL request to fail

                    for (int i = 0; i < fileBytes.Length; i += chunkSize)
                    {
                        var toWrite = chunkSize;

                        if (i + 1 + chunkSize > fileBytes.Length)
                            toWrite = fileBytes.Length - i;

                        requestStream.Write(fileBytes, i, toWrite);
                    }

                    requestStream.Write(fileTrailer, 0, fileTrailer.Length);

                }



                requestStream.Close();
                requestStream.Dispose();
                requestStream = null;

            }

            catch (Exception ex)
            {
                if (request != null)
                    request.Dispose();

                request = null;

                Debug.Print(ex.ToString());
            }

            finally
            {
                if (requestStream != null)
                    requestStream.Dispose();
            }

            return request;
        }

    }
    
}
