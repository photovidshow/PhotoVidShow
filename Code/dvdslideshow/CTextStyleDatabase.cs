using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;

namespace DVDSlideshow
{
    public class CTextStyleDatabase
    {
        private List<CTextStyle> mStyles = new List<CTextStyle>();

        private static CTextStyleDatabase mInstance = new CTextStyleDatabase();

        public static CTextStyleDatabase GetInstance() { return mInstance; }

        private CTextStyle emptyStyle = new CTextStyle();


        //*******************************************************************
        private CTextStyleDatabase()
        {
            // create default fonts

            // vanilla
            //<TextStyle FontName="Segoe UI" FontSize="6.054319" OutLine="True" OutLineColour="Color [White]" OutlineLength="4" 
            // OutlineAlpha="87" />
            CTextStyle vanilla = new CTextStyle();
            vanilla.Name = "Vanilla";
            vanilla.FontName = "Segoe UI";
            vanilla.Outline = true;
            vanilla.OutlineColor = Color.White;
            vanilla.TextColor = Color.White;
            vanilla.OutlineLength = 4;
            vanilla.OutlineAlpha = 14;
            mStyles.Add(vanilla);


            // Salt and Pepper
           //    <TextStyle FontName="Arial" FontSize="46.93481" Bold="True" OutLine="True" OutLineColour="Color [Black]" FontColor2="Color [Silver]"
           // Gradient="True" ShadowAlpha="166" ShadowOffsetX="3.106055" ShadowOffsetY="1.382905" OutlineLength="5" OutlineAlpha="83" /         
            CTextStyle saltandpepper = new CTextStyle();
            saltandpepper.Name = "Salt & Pepper";
            saltandpepper.FontName = "Arial";
            saltandpepper.TextColor = Color.White;
            saltandpepper.Bold = true;
            saltandpepper.Shadow = false;
            saltandpepper.Outline = true;
            saltandpepper.OutlineColor = Color.Black;
            saltandpepper.TextColor2 = Color.Silver;
            saltandpepper.Gradient = true;
            saltandpepper.OutlineLength = 5;
            saltandpepper.OutlineAlpha = 83;
            mStyles.Add(saltandpepper);
            

            // LEMON GRASS

            //  <TextStyle FontName="Arial" FontColor="Color [A=255, R=128, G=255, B=128]" Bold="True" Shadow="True" 
            //  ShadowColor="Color [Black]" OutLine="True" OutLineColour="Color [Green]" FontColor2="Color [A=255, R=128, G=255, B=0]" 
            // Gradient="True" ShadowAlpha="166" ShadowOffsetX="3.106055" ShadowOffsetY="1.382905" OutlineLength="5" OutlineAlpha="83" />

            CTextStyle lemongrass = new CTextStyle();
            lemongrass.Name ="Lemongrass";
            lemongrass.FontName = "Arial";
            lemongrass.TextColor = Color.FromArgb(128,255,128);
            lemongrass.Bold = true;
            lemongrass.Shadow = false;
            lemongrass.ShadowColor = Color.Black;
            lemongrass.Outline = true;
            lemongrass.OutlineColor = Color.Green;
            lemongrass.TextColor2 = Color.FromArgb(128,255,0);
            lemongrass.Gradient=true;
            lemongrass.ShadowAlpha = 166;
            lemongrass.ShadowOffsetX = 3.106055f;
            lemongrass.ShadowOffsetY = 1.382905f;
            lemongrass.OutlineLength = 5;
            lemongrass.OutlineAlpha = 83;
            mStyles.Add(lemongrass);


            // GALANGAL
            //      <TextStyle FontName="Arial" FontSize="3.854447" FontColor="Color [A=255, R=254, G=250, B=233]" OutLine="True" 
            //  OutLineColour="Color [A=255, R=248, G=217, B=92]" Alignment="Center" FontColor2="Color [A=255, R=251, G=237, B=181]" 
            // Gradient="True" ShadowAlpha="166" ShadowOffsetX="3.106055" ShadowOffsetY="1.382905" OutlineLength="4" OutlineAlpha="5" />

            CTextStyle galangal = new CTextStyle();
            galangal.Name = "Galangal";
            galangal.FontName = "Arial";
            galangal.TextColor = Color.FromArgb(254, 250, 233);
            galangal.Bold = false;
            galangal.Shadow = false;
            galangal.ShadowColor = Color.Black;
            galangal.Outline = true;
            galangal.OutlineColor = Color.FromArgb(248, 217, 92);
            galangal.TextColor2 = Color.FromArgb(251, 237, 181);
            galangal.Gradient = true;
            galangal.ShadowAlpha = 166;
            galangal.ShadowOffsetX = 3.106055f;
            galangal.ShadowOffsetY = 1.382905f;
            galangal.OutlineLength = 4;
            galangal.OutlineAlpha = 5;
            mStyles.Add(galangal);


            // PINK peppercorn

           //<TextStyle FontName="Tahoma" FontSize="86.73699" FontColor="Color [A=255, R=255, G=128, B=128]" 
           // Bold="True" Shadow="True" ShadowColor="Color [Black]" OutLine="True" OutLineColour="Color [White]"
            // Alignment="Center" ShadowAlpha="62" ShadowOffsetX="6.311612" ShadowOffsetY="1.929653" 
            // OutlineLength="3" OutlineAlpha="116" />

            CTextStyle boldPink = new CTextStyle();
            boldPink.Name = "Pink peppercorn";
            boldPink.FontName = "Tahoma";
            boldPink.TextColor = Color.FromArgb(255, 128, 128);
            boldPink.Bold = true;
            boldPink.Shadow = true;
            boldPink.ShadowColor = Color.Black;
            boldPink.Outline = true;
            boldPink.OutlineColor = Color.White;
            boldPink.Gradient = false;
            boldPink.ShadowAlpha = 62;
            boldPink.ShadowOffsetX = 6.311612f;
            boldPink.ShadowOffsetY = 1.929653f;
            boldPink.OutlineLength = 3;
            boldPink.OutlineAlpha = 116;
            mStyles.Add(boldPink);


            // Blue Fenugreek
            //
            // <TextStyle FontName="Arial" FontSize="16.82969" FontColor="Color [A=255, R=193, G=240, B=249]" Bold="True" OutLine="True"
            // OutLineColour="Color [A=255, R=21, G=189, B=219]" Alignment="Far" FontColor2="Color [A=255, R=98, G=219, B=240]" 
            // Gradient="True" ShadowAlpha="166" ShadowOffsetX="3.106055" ShadowOffsetY="1.382905" OutlineLength="5" OutlineAlpha="52" />

            CTextStyle bluefenugreek = new CTextStyle();
            bluefenugreek.Name = "Blue Fenugreek";
            bluefenugreek.FontName = "Arial";
            bluefenugreek.TextColor = Color.FromArgb(193, 240, 249);
            bluefenugreek.TextColor2 = Color.FromArgb(98, 219, 240);
            bluefenugreek.Gradient = true;
            bluefenugreek.Bold = true;
            bluefenugreek.Shadow = false;
            bluefenugreek.Outline = true;
            bluefenugreek.OutlineColor = Color.FromArgb(21, 189, 219);
            bluefenugreek.OutlineLength = 5;
            bluefenugreek.OutlineAlpha = 52;
            mStyles.Add(bluefenugreek);


            // ginger
            // <TextStyle FontName="Segoe UI" FontSize="17.19294" OutLine="True" OutLineColour="Color [Black]" Alignment="Center" FontColor2="Color [Silver]" 
            // ShadowAlpha="0" ShadowOffsetX="4.368691" ShadowOffsetY="3.17404" OutlineLength="9" OutlineAlpha="33" />
            //FontColor="Color [A=255, R=244, G=194, B=77]"

            CTextStyle ginger = new CTextStyle();
            ginger.Name = "Ginger";
            ginger.FontName = "Segoe UI";
            ginger.TextColor = Color.FromArgb(244, 194, 77);
            ginger.Gradient = false;
            ginger.Bold = false;
            ginger.Shadow = false;
            ginger.Outline = true;
            ginger.OutlineColor = Color.Black;
            ginger.OutlineLength = 9;
            ginger.OutlineAlpha = 33;
            mStyles.Add(ginger);

            // xmas bold
            // <TextStyle FontName="Segoe UI" FontSize="33.33333" Bold="True" OutLine="True" OutLineColour="Color [Black]" Alignment="Center"
            // FontColor2="Color [Silver]" ShadowAlpha="0" ShadowOffsetX="4.368691" ShadowOffsetY="3.17404" OutlineLength="9" OutlineAlpha="33" />

            CTextStyle xmasBold = new CTextStyle();
            xmasBold.Name = "XmasBold";
            xmasBold.FontName = "Segoe UI";
            xmasBold.TextColor = Color.White;
            xmasBold.Gradient = false;
            xmasBold.Bold = true;
            xmasBold.Shadow = false;
            xmasBold.Outline = true;
            xmasBold.OutlineColor = Color.Black;
            xmasBold.OutlineLength = 9;
            xmasBold.OutlineAlpha = 33;
            mStyles.Add(xmasBold);

            // Menu header
            //  <TextStyle FontName="Segoe UI" FontSize="30" Bold="True" OutLine="True" OutLineColour="Color [Black]" Italic="True" Alignment="Center" 
            //  OutlineLength="5" OutlineAlpha="64" />

            CTextStyle blackSaltItalic = new CTextStyle();
            blackSaltItalic.Name = "Black salt italic";
            blackSaltItalic.FontName = "Segoe UI";
            blackSaltItalic.TextColor = Color.FromArgb(60, 60, 60);
            blackSaltItalic.Gradient = false;
            blackSaltItalic.Bold = true;
            blackSaltItalic.Shadow = false;
            blackSaltItalic.Outline = false;
            blackSaltItalic.Italic = true;
            blackSaltItalic.OutlineLength = 5;
            blackSaltItalic.OutlineAlpha = 64;
            mStyles.Add(blackSaltItalic);

            CTextStyle blackSalt = new CTextStyle();
            blackSalt.Name = "Black salt";
            blackSalt.FontName = "Segoe UI";
            blackSalt.TextColor = Color.FromArgb(60, 60, 60);
            blackSalt.Gradient = false;
            blackSalt.Bold = true;
            blackSalt.Shadow = false;
            blackSalt.Outline = false;
            blackSalt.OutlineLength = 5;
            blackSalt.OutlineAlpha = 64;
            mStyles.Add(blackSalt);

            // black pepper

           //   <TextStyle FontName="Segoe UI"  FontColor="Color [Black]" Bold="True" OutLine="True" OutLineColour="Color [Black]" 
           // OutlineLength="3" OutlineAlpha="41" />

            CTextStyle backpepper = new CTextStyle();
            backpepper.Name = "Black pepper";
            backpepper.FontName = "Segoe UI";
            backpepper.Outline = true;
            backpepper.OutlineColor = Color.Black;
            backpepper.TextColor = Color.Black;
            backpepper.OutlineLength = 3;
            backpepper.OutlineAlpha = 41;
            mStyles.Add(backpepper);

            CTextStyle backpepperbold = new CTextStyle();
            backpepperbold.Name = "Black pepper bold";
            backpepperbold.FontName = "Segoe UI";
            backpepperbold.Outline = true;
            backpepperbold.Bold = true;
            backpepperbold.OutlineColor = Color.Black;
            backpepperbold.TextColor = Color.Black;
            backpepperbold.OutlineLength = 3;
            backpepperbold.OutlineAlpha = 41;
            mStyles.Add(backpepperbold);


            // Turmeric
            //<TextStyle FontName="Comic Sans MS" FontSize="46.11977" FontColor="Color [A=255, R=244, G=244, B=0]" OutLine="True" 
            //OutLineColour="Color [A=255, R=173, G=148, B=5]" Alignment="Center" FontColor2="Color [A=255, R=244, G=244, B=0]" Gradient="True"
            //OutlineLength="6" OutlineAlpha="69" />
            CTextStyle turmeric = new CTextStyle();
            turmeric.Name = "Turmeric";
            turmeric.FontName = "Comic Sans MS";
            turmeric.Outline = true;
            turmeric.OutlineColor = Color.FromArgb(173, 148, 5);
            turmeric.TextColor = Color.FromArgb(244, 244, 0);
            turmeric.OutlineLength = 6;
            turmeric.OutlineAlpha = 69;
            mStyles.Add(turmeric);

            // Blue berry
           // <TextStyle FontName="Comic Sans MS" FontSize="34.66214" FontColor="Color [A=255, R=9, G=24, B=91]" Shadow="True" 
            //ShadowColor="Color [Black]" OutLine="True" OutLineColour="Color [White]" FontColor2="Color [A=255, R=4, G=11, B=40]" 
            //ShadowAlpha="71" ShadowOffsetX="2.527622" ShadowOffsetY="3.354269" OutlineLength="3" OutlineAlpha="248" />
            CTextStyle blueBerry = new CTextStyle();
            blueBerry.Name = "Blueberry";
            blueBerry.FontName = "Comic Sans MS";
            blueBerry.TextColor = Color.FromArgb(9, 24, 91);
            blueBerry.Bold = false;
            blueBerry.Shadow = true;
            blueBerry.ShadowColor = Color.Black;
            blueBerry.Outline = true;
            blueBerry.OutlineColor = Color.White;
            blueBerry.Gradient = false;
            blueBerry.ShadowAlpha = 71;
            blueBerry.ShadowOffsetX = 2.527622f;
            blueBerry.ShadowOffsetY = 3.354269f;
            blueBerry.OutlineLength = 3;
            blueBerry.OutlineAlpha = 248;
            mStyles.Add(blueBerry);


            // Red chilli
            // <TextStyle FontName="Arial Black" FontSize="26.66667" FontColor="Color [A=255, R=254, G=22, B=22]" Shadow="True" 
            // ShadowColor="Color [Black]" OutLine="True" OutLineColour="Color [A=255, R=198, G=39, B=0]" 
            // FontColor2="Color [A=255, R=255, G=128, B=64]" Gradient="True" ShadowAlpha="71" 
            //ShadowOffsetX="2.527622" ShadowOffsetY="3.354269" OutlineLength="11" OutlineAlpha="33" />            
            CTextStyle redChilli = new CTextStyle();
            redChilli.Name = "Red chilli";
            redChilli.FontName = "Arial Black";
            redChilli.TextColor = Color.FromArgb(254, 22, 22);
            redChilli.Bold = false;
            redChilli.Shadow = true;
            redChilli.ShadowColor = Color.Black;
            redChilli.Outline = true;
            redChilli.OutlineColor = Color.FromArgb(198, 39, 0);
            redChilli.TextColor2 = Color.FromArgb(255, 128, 64);
            redChilli.Gradient = true;
            redChilli.ShadowAlpha = 71;
            redChilli.ShadowOffsetX = 2.527622f;
            redChilli.ShadowOffsetY = 3.354269f;
            redChilli.OutlineLength = 11;
            redChilli.OutlineAlpha = 33;
            mStyles.Add(redChilli);

            // Ground coriander
            // <TextStyle FontSize="26.66666" FontColor="Color [A=255, R=183, G=153, B=49]" OutLine="True" 
            // OutLineColour="Color [Black]" Italic="True" FontColor2="Color [A=255, R=175, G=120, B=65]" Gradient="True" 
            // ShadowAlpha="143" ShadowOffsetX="1.9" ShadowOffsetY="3.290896" OutlineLength="4" OutlineAlpha="186" />
            CTextStyle groundCoriander = new CTextStyle();
            groundCoriander.Name = "Ground coriander";
            groundCoriander.FontName = "Verdana";
            groundCoriander.TextColor = Color.FromArgb(183, 153, 49);
            groundCoriander.Bold = false;
            groundCoriander.Shadow = false;
            groundCoriander.Outline = true;
            groundCoriander.OutlineColor = Color.Black;
            groundCoriander.TextColor2 = Color.FromArgb(175, 120, 65);
            groundCoriander.Gradient = true;  
            groundCoriander.OutlineLength = 4;
            groundCoriander.OutlineAlpha = 186;
            mStyles.Add(groundCoriander);
  
            // Cardamon
            //<TextStyle FontName="Times New Roman" FontSize="33.32354" FontColor="Color [A=255, R=107, G=125, B=2]"
            //OutLine="True" OutLineColour="Color [A=255, R=63, G=73, B=1]" FontColor2="Color [A=255, R=188, G=221, B=4]" 
            //Gradient="True" ShadowAlpha="71" ShadowOffsetX="2.527622" ShadowOffsetY="3.354269" OutlineLength="3" />
            CTextStyle Cardamon = new CTextStyle();
            Cardamon.Name = "Cardamon";
            Cardamon.FontName = "Times New Roman";
            Cardamon.TextColor = Color.FromArgb(107, 125, 2);
            Cardamon.Bold = false;
            Cardamon.Outline = true;
            Cardamon.OutlineColor = Color.FromArgb(64, 73, 1);
            Cardamon.TextColor2 = Color.FromArgb(188, 221, 4);
            Cardamon.Gradient = true; 
            Cardamon.OutlineLength = 3;
            Cardamon.OutlineAlpha = 240;
            mStyles.Add(Cardamon);


            // Mustard seeds
            // <TextStyle FontName="Arial" FontSize="34.63314" FontColor="Color [A=255, R=223, G=203, B=91]" Bold="True"
            // Shadow="True" ShadowColor="Color [Black]" OutLine="True" OutLineColour="Color [A=255, R=223, G=203, B=91]" Italic="True"
            //FontColor2="Color [A=255, R=4, G=11, B=40]" ShadowAlpha="71" ShadowOffsetX="2.527622" ShadowOffsetY="3.354269" 
            //OutlineLength="3" OutlineAlpha="19" />
            CTextStyle mustardSeeds = new CTextStyle();
            mustardSeeds.Name = "Mustard seeds";
            mustardSeeds.FontName = "Arial";
            mustardSeeds.TextColor = Color.FromArgb(223, 203, 91);
            mustardSeeds.Bold = true;
            mustardSeeds.Shadow = true;
            mustardSeeds.ShadowColor = Color.Black;
            mustardSeeds.Outline = true;
            mustardSeeds.OutlineColor = Color.FromArgb(223, 203, 91);
            mustardSeeds.Italic = true;
            mustardSeeds.ShadowAlpha = 71;
            mustardSeeds.ShadowOffsetX = 2.527622f;
            mustardSeeds.ShadowOffsetY = 3.354269f;
            mustardSeeds.OutlineLength = 3;
            mustardSeeds.OutlineAlpha = 19;
            mStyles.Add(mustardSeeds);

           
            // Ground cumin
            // <TextStyle FontName="Tahoma" FontSize="34.64843" FontColor="Color [A=255, R=159, G=108, B=57]" Shadow="True" 
            // ShadowColor="Color [Black]" OutLine="True" OutLineColour="Color [A=255, R=86, G=58, B=31]" 
            // FontColor2="Color [A=255, R=120, G=82, B=44]" Gradient="True" ShadowAlpha="74"
            // ShadowOffsetX="2.699708" ShadowOffsetY="3.217386" OutlineLength="4" OutlineAlpha="155" />
            CTextStyle groundCumin = new CTextStyle();
            groundCumin.Name = "Ground cumin";
            groundCumin.FontName = "Tahoma";
            groundCumin.TextColor = Color.FromArgb(159, 108, 57);
            groundCumin.Shadow = true;
            groundCumin.ShadowColor = Color.Black;
            groundCumin.Outline = true;
            groundCumin.OutlineColor = Color.FromArgb(86, 58, 31);
            groundCumin.TextColor2 = Color.FromArgb(120, 82, 44);
            groundCumin.Gradient = true;
            groundCumin.ShadowAlpha = 74;
            groundCumin.ShadowOffsetX = 2.699708f;
            groundCumin.ShadowOffsetY = 3.217386f;
            groundCumin.OutlineLength = 4;
            groundCumin.OutlineAlpha = 155; ;
            mStyles.Add(groundCumin);

            // Cloves
            // <TextStyle FontName="Segoe UI" FontSize="26.65001" FontColor="Color [A=255, R=64, G=0, B=0]" 
            // OutLine="True" OutLineColour="Color [Black]" FontColor2="Color [A=255, R=175, G=120, B=65]" 
            // ShadowAlpha="143" ShadowOffsetX="1.9" ShadowOffsetY="3.290896" OutlineLength="3" OutlineAlpha="122" />

            CTextStyle cloves = new CTextStyle();
            cloves.Name = "Cloves";
            cloves.FontName = "Segoe UI";
            cloves.TextColor = Color.FromArgb(64, 0, 0);
            cloves.Outline = true;
            cloves.OutlineColor = Color.Black;
            cloves.TextColor2 = Color.FromArgb(175, 120, 65);
            cloves.Gradient = true;
            cloves.OutlineLength = 3;
            cloves.OutlineAlpha = 122; ;
            mStyles.Add(cloves);

            // Kaffir leaves
            //<TextStyle FontName="Segoe UI" FontSize="31.4561" FontColor="Color [A=255, R=0, G=128, B=64]" Bold="True" 
            //Shadow="True" ShadowColor="Color [Black]" OutLine="True" OutLineColour="Color [Black]" Italic="True"
            //FontColor2="Color [A=255, R=0, G=255, B=64]" Gradient="True" ShadowAlpha="105" ShadowOffsetX="1.9" 
            //ShadowOffsetY="3.290896" OutlineLength="3" OutlineAlpha="122" />
            CTextStyle kaffirLeaves = new CTextStyle();
            kaffirLeaves.Name = "Kaffir leaves";
            kaffirLeaves.FontName = "Segoe UI";
            kaffirLeaves.TextColor = Color.FromArgb(0, 128, 64);
            kaffirLeaves.Bold = true;
            kaffirLeaves.Shadow = true;
            kaffirLeaves.ShadowColor = Color.Black;
            kaffirLeaves.Outline = true;
            kaffirLeaves.OutlineColor = Color.Black;
            kaffirLeaves.Italic = true;
            kaffirLeaves.TextColor2 = Color.FromArgb(0, 255, 64);
            kaffirLeaves.Gradient = true;
            kaffirLeaves.ShadowAlpha = 105;
            kaffirLeaves.ShadowOffsetX = 1.9f;
            kaffirLeaves.ShadowOffsetY = 3.290896f;
            kaffirLeaves.OutlineLength = 3;
            kaffirLeaves.OutlineAlpha = 122; ;
            mStyles.Add(kaffirLeaves);

            // Paprika
            // <TextStyle FontName="Segoe UI" FontSize="31.96659" FontColor="Color [A=255, R=198, G=39, B=0]" Bold="True" 
            // OutLine="True" OutLineColour="Color [A=255, R=143, G=87, B=3]" FontColor2="Color [Maroon]" Gradient="True" 
            //ShadowAlpha="79" ShadowOffsetX="1.9" ShadowOffsetY="3.290896" OutlineLength="3" OutlineAlpha="114" />
            CTextStyle paprika = new CTextStyle();
            paprika.Name = "Paprika";
            paprika.FontName = "Segoe UI";
            paprika.TextColor = Color.FromArgb(198, 39, 0);
            paprika.Bold = true;
            paprika.Outline = true;
            paprika.OutlineColor = Color.FromArgb(143, 87, 3);
            paprika.TextColor2 = Color.Maroon;
            paprika.Gradient = true;
            paprika.OutlineLength = 3;
            paprika.OutlineAlpha = 114; ;
            mStyles.Add(paprika);

            // Red cabbage
            // <TextStyle FontName="Segoe Print" FontSize="31.96424" FontColor="Color [Purple]" Bold="True" OutLine="True" 
            // OutLineColour="Color [White]" FontColor2="Color [A=255, R=74, G=0, B=74]" Gradient="True" ShadowAlpha="79" 
            // ShadowOffsetX="1.9" ShadowOffsetY="3.290896" OutlineLength="5" OutlineAlpha="176" />
            CTextStyle redCabbage = new CTextStyle();
            redCabbage.Name = "Red cabbage";
            redCabbage.FontName = "Segoe Print";
            redCabbage.TextColor = Color.Purple;
            redCabbage.Bold = true;
            redCabbage.Outline = true;
            redCabbage.OutlineColor = Color.White;
            redCabbage.TextColor2 = Color.FromArgb(74, 0, 74);
            redCabbage.Gradient = true;
            redCabbage.OutlineLength = 5;
            redCabbage.OutlineAlpha = 176; ;
            mStyles.Add(redCabbage);

            //Star Anise
            // <TextStyle FontName="MV Boli" FontSize="31.96898" FontColor="Color [A=255, R=142, G=62, B=9]" Bold="True" 
            // OutLine="True" OutLineColour="Color [Maroon]" FontColor2="Color [A=255, R=244, G=155, B=96]" Gradient="True"
            // ShadowAlpha="79" ShadowOffsetX="1.9" ShadowOffsetY="3.290896" OutlineLength="7" OutlineAlpha="124" />
            CTextStyle starAnise = new CTextStyle();
            starAnise.Name = "Star anise";
            starAnise.FontName = "MV Boli";
            starAnise.TextColor = Color.FromArgb(142, 62, 9); ;
            starAnise.Bold = true;
            starAnise.Outline = true;
            starAnise.OutlineColor = Color.Maroon; ;
            starAnise.TextColor2 = Color.FromArgb(244, 155, 96);
            starAnise.Gradient = true;
            starAnise.OutlineLength = 7;
            starAnise.OutlineAlpha = 124;
            mStyles.Add(starAnise);

            // Green chilli
            // <TextStyle FontName="Impact" FontSize="31.98105" FontColor="Color [A=255, R=0, G=128, B=64]" OutLine="True" 
            // OutLineColour="Color [A=255, R=0, G=64, B=0]" FontColor2="Color [Yellow]" Gradient="True" ShadowAlpha="79" 
            // ShadowOffsetX="1.9" ShadowOffsetY="3.290896" OutlineLength="7" OutlineAlpha="124" />
            CTextStyle greenChilli = new CTextStyle();
            greenChilli.Name = "Green chilli";
            greenChilli.FontName = "Impact";
            greenChilli.TextColor = Color.FromArgb(0, 128, 64); ;
            greenChilli.Outline = true;
            greenChilli.OutlineColor = Color.FromArgb(0, 64, 0); ;
            greenChilli.TextColor2 = Color.Yellow;
            greenChilli.Gradient = true;
            greenChilli.OutlineLength = 7;
            greenChilli.OutlineAlpha = 124;
            mStyles.Add(greenChilli);

            // Sprinkles
            // <TextStyle FontName="Gentium Book Basic" FontSize="31.96452" FontColor="Color [A=255, R=128, G=128, B=255]" 
            // Shadow="True" ShadowColor="Color [Black]" OutLine="True" OutLineColour="Color [A=255, R=64, G=0, B=64]"
            //Italic="True" FontColor2="Color [A=255, R=255, G=128, B=128]" Gradient="True" ShadowAlpha="119" ShadowOffsetX="1.4"
            //ShadowOffsetY="2.424871" OutlineLength="4" OutlineAlpha="69" />
            CTextStyle sprinkles = new CTextStyle();
            sprinkles.Name = "Sprinkles";
            sprinkles.FontName = "Gentium Book Basic";
            sprinkles.TextColor = Color.FromArgb(128, 128, 255);
            sprinkles.Shadow = true;
            sprinkles.ShadowColor = Color.Black;
            sprinkles.Outline = true;
            sprinkles.OutlineColor = Color.FromArgb(64, 0, 64);
            sprinkles.Italic = true;
            sprinkles.TextColor2 = Color.FromArgb(255, 128, 128);
            sprinkles.Gradient = true;
            sprinkles.ShadowAlpha = 119;
            sprinkles.ShadowOffsetX = 1.4f;
            sprinkles.ShadowOffsetY = 2.424871f;
            sprinkles.OutlineLength = 4;
            sprinkles.OutlineAlpha = 69;
            mStyles.Add(sprinkles);

            //
            // old menu fonts
            //
            CTextStyle menuHeader = new CTextStyle();
            menuHeader.Name = "MenuHeader";
            menuHeader.FontName = "Segoe UI";
            menuHeader.TextColor = Color.White;
            menuHeader.Gradient = false;
            menuHeader.Bold = true;
            menuHeader.Shadow = false;
            menuHeader.Outline = true;
            menuHeader.Italic = true;
            menuHeader.OutlineColor = Color.Black;
            menuHeader.OutlineLength = 5;
            menuHeader.OutlineAlpha = 64;
            mStyles.Add(menuHeader);

            CTextStyle menu = new CTextStyle();
            menu.Name = "Menu";
            menu.FontName = "Segoe UI";
            menu.TextColor = Color.White;
            menu.Gradient = false;
            menu.Bold = true;
            menu.Shadow = false;
            menu.Outline = true;
            menu.OutlineColor = Color.Black;
            menu.OutlineLength = 5;
            menu.OutlineAlpha = 64;
            mStyles.Add(menu);
        }

        //*******************************************************************
        public void AddStyleIfNotAlreadyInDatabase(CTextStyle newStyle)
        {
            if (newStyle.IsSameAs(emptyStyle)) return;

            foreach(CTextStyle style in mStyles)
            {
                if (newStyle.IsSameAs(style) ==true) return ;
            }

            mStyles.Insert(0,newStyle);
        }

        //*******************************************************************
        public List<CTextStyle> Styles
        {
            get
            {
                CleanUp();  // remove and styles in the database which are not used anywhere
                AddNewStyles(); // add any styles that are used which are not in the database
                return mStyles;
            }
        }


        //*******************************************************************
        private bool DoesStyleExistinSlide(CSlide slide, CTextStyle style)
        {
          
            CImageSlide imageSlide = slide as CImageSlide;
            if (imageSlide!=null)
            {
                ArrayList decorations = imageSlide.GetAllAndSubDecorations();
                foreach (CDecoration dec in decorations)
                {
                    CTextDecoration td = dec as CTextDecoration;
                    if (td!=null)
                    {
                        if (td.TextStyle == style)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        //*******************************************************************
        private List<CTextStyle> GetAllStylesInSlide(CSlide slide)
        {
            List<CTextStyle> returnList = new List<CTextStyle>();

            CImageSlide imageSlide = slide as CImageSlide;
            if (imageSlide != null)
            {
                ArrayList decorations = imageSlide.GetAllAndSubDecorations();
                foreach (CDecoration dec in decorations)
                {
                    CTextDecoration td = dec as CTextDecoration;
                    if (td != null && td.TextStyle.Name == "")
                    {
                        returnList.Add(td.TextStyle);
                    }
                }
            }

            return returnList;
        }




        //*******************************************************************
        private void CleanUp()
        {
            List<CTextStyle> toRemove = new List<CTextStyle>();

            foreach (CTextStyle style in mStyles)
            {
                if (style.Name != "") continue;

                bool found = false;

                foreach (CSlideShow slideshow in CGlobals.mCurrentProject.GetAllProjectSlideshows(true))
                {
                    foreach (CSlide slide in slideshow.mSlides)
                    {
                        if (DoesStyleExistinSlide(slide, style) == true)
                        {
                            found = true;
                        }
                    }
                }

                ArrayList menus = CGlobals.mCurrentProject.MainMenu.GetSelfAndAllSubMenus();

                if (found == false)
                {
                    foreach (CMainMenu menu in menus)
                    {
                        if (DoesStyleExistinSlide(menu.BackgroundSlide, style) == true)
                        {
                            found = true;
                        }
                    }
                }

                if (found == false)
                {
                    toRemove.Add(style);
                }
            }

            foreach (CTextStyle style in toRemove)
            {
                mStyles.Remove(style);
            }
        }


        //*******************************************************************
        private void AddNewStyles()
        {
            List<CTextStyle> toAdd = new List<CTextStyle>();
      
            foreach (CSlideShow slideshow in CGlobals.mCurrentProject.GetAllProjectSlideshows(true))
            {
                foreach (CSlide slide in slideshow.mSlides)
                {
                    List<CTextStyle> styles = GetAllStylesInSlide(slide);

                    foreach (CTextStyle style in styles)
                    {
                        AddStyleIfNotAlreadyInDatabase(style);
                    }
                }
            }

            ArrayList menus = CGlobals.mCurrentProject.MainMenu.GetSelfAndAllSubMenus();

           
            foreach (CMainMenu menu in menus)
            {
                List<CTextStyle> styles = GetAllStylesInSlide(menu.BackgroundSlide);

                foreach (CTextStyle style in styles)
                {
                    AddStyleIfNotAlreadyInDatabase(style);
                }
            }
        }

        //*******************************************************************
        // If given style matches any default styles then it returns the style name else return ""
        public string GetMatchingStyleName(CTextStyle style)
        {
            foreach (CTextStyle customStyle in mStyles)
            {
                if (customStyle.IsSameAs(style) == true)
                {
                    return customStyle.Name;
                }
            }
            return "";
        }


        //*******************************************************************
        public CTextStyle GetStyleFromName(string name)
        {
            foreach (CTextStyle customStyle in mStyles)
            {
                if (customStyle.Name == name)
                {
                    return customStyle;
                }
            }
            return null;
        }
    }
}
