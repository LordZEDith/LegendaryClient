﻿using LegendaryClient.Logic;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace LegendaryClient.Windows
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        #region DLL Stuff

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(
              string deviceName, int modeNum, ref DEVMODE devMode);

        private const int ENUM_CURRENT_SETTINGS = -1;

        private const int ENUM_REGISTRY_SETTINGS = -2;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;

            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;

            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        #endregion DLL Stuff

        private List<string> Resolutions = new List<string>();
        private MainWindow mainWindow;

        private Dictionary<string, WinThemes> list = new Dictionary<string, WinThemes>();
        private Dictionary<WinThemes, string> list2 = new Dictionary<WinThemes, string>();
        private Dictionary<string, string> list3 = new Dictionary<string, string>();
        public SettingsPage(MainWindow window)
        {
            InitializeComponent();
            InsertDefaultValues();
            mainWindow = window;

            StatsCheckbox.IsChecked = Properties.Settings.Default.GatherStatistics;
            ErrorCheckbox.IsChecked = Properties.Settings.Default.SendErrors;

            #region AboutTextbox

            AboutTextBox.Text =
@"Copyright (c) 2013-2014, Eddy5641 (Eddy V)
All rights reserved.



Thanks to " + Client.LoginPacket.AllSummonerData.Summoner.Name + @". Using this client means the most to me. Thank you very much!

Big thanks to Snowl. Created the foundation of this custom client.
Thanks to all the people at #riotcontrol, especially raler (for providing PVPNetConnect).
Thanks to all the people who done the RTMPS work. Your help has been invaluable.

Thanks Riot for providing a pretty awesome game. It might not be perfect, but I have wasted many hours on it.

Thanks to all the people who have supported me.

Uses Data Dragon QA Interface & l3cdn.

This product is not endorsed, certified or otherwise approved in any way by Riot Games, Inc. or any of its affiliates.

External libraries:
Awesomium
jabber-net
MahApps.Metro
Rtmp-sharp
PVPNetConnect
SharpZipLib
sqlite
zlib

Donations are accepted at:
Not accepted yet

Donations will be used in ways that support LegendaryClient. Examples are:
Domain name (LegendaryClient.ca|LegendaryClient.gg)
A code signing license (So you know that you are using LegendaryClient)

";

            #endregion AboutTextbox

            Addtheme("Dark Steel", "pack://application:,,,/LegendaryClient;component/Controls/Steel.xaml");
            Addtheme("Light Steel", "pack://application:,,,/LegendaryClient;component/Controls/Steel.xaml");
            Addtheme("Dark Blue", "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml");
            Addtheme("Light Blue", "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml");
            Addtheme("Dark Red", "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Red.xaml");
            Addtheme("Light Red", "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Red.xaml");
            Addtheme("Dark Green", "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml");
            Addtheme("Light Green", "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml");
            if (Properties.Settings.Default.Theme != null)
                foreach(var x in list2)
                {
                    WinThemes m = x.Key;
                    if (x.Value.Contains("Light") && Properties.Settings.Default.DarkTheme == false)
                    {
                        foreach (var h in list3)
                        {
                            if (h.Value == Properties.Settings.Default.Theme)
                            {
                                ThemeBox.SelectedItem = m;
                            }
                        }
                    }
                    else if (x.Value.Contains("Dark") && Properties.Settings.Default.DarkTheme == true)
                    {
                        foreach (var h in list3)
                        {
                            if (h.Value == Properties.Settings.Default.Theme)
                            {
                                ThemeBox.SelectedItem = m;
                            }
                        }
                    }
                }
        }

        public void Addtheme(string Text, string Value)
        {
            WinThemes theme = new WinThemes();
            theme.Text = Text;
            theme.Value = Value;
            ThemeBox.Items.Add(theme);
            list.Add(Text, theme);
            list2.Add(theme, Text);
            list3.Add(Text, Value);
        }

        public void InsertDefaultValues()
        {
            //Insert resolutions
            DEVMODE vDevMode = new DEVMODE();
            int i = 0;

            while (EnumDisplaySettings(null, i, ref vDevMode))
            {
                if (!Resolutions.Contains(String.Format("{0}x{1}", vDevMode.dmPelsWidth, vDevMode.dmPelsHeight)) && vDevMode.dmPelsWidth >= 1000)
                {
                    Resolutions.Add(String.Format("{0}x{1}", vDevMode.dmPelsWidth, vDevMode.dmPelsHeight));
                    ResolutionComboBox.Items.Add(String.Format("{0}x{1}", vDevMode.dmPelsWidth, vDevMode.dmPelsHeight));
                }
                i++;
            }

            LoginImageBox.Text = Properties.Settings.Default.LoginPageImage;

            ResolutionComboBox.SelectedIndex = ResolutionComboBox.Items.Count - 1;
            WindowModeComboBox.SelectedIndex = 0;
        }

        private void StatsCheckbox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Properties.Settings.Default.GatherStatistics = (bool)StatsCheckbox.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void ErrorCheckbox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Properties.Settings.Default.SendErrors = (bool)ErrorCheckbox.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void LoginImageBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.LoginPageImage = LoginImageBox.Text;
        }

        private void LoginImageBox_DropDownClosed(object sender, EventArgs e)
        {
            string temp = Properties.Settings.Default.LoginPageImage;
            LoginImageBox.Items.Clear();
            LoginImageBox.Text = temp;
        }

        private void LoginImageBox_DropDownOpened(object sender, EventArgs e)
        {
            foreach (string s in Directory.EnumerateFiles(Path.Combine(Client.ExecutingDirectory, "Assets", "champions"), "*", SearchOption.AllDirectories)
                .Select(Path.GetFileName))
            {
                if (s.Contains("Splash")) LoginImageBox.Items.Add(s);
            }
        }

        private void ThemeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ThemeBox.SelectedItem as WinThemes).Text.Contains("Dark"))
                Properties.Settings.Default.DarkTheme = true;
            else
                Properties.Settings.Default.DarkTheme = false;
            Properties.Settings.Default.Theme = (string)(ThemeBox.SelectedItem as WinThemes).Value;
            
            mainWindow.ChangeTheme();
        }
    }
    public class WinThemes
    {
        public string Text { get; set; }
        public object Value { get; set; }
        public override string ToString()
        {
            return Text;
        }
    }
}