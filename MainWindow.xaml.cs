using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SinusPong
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


         
                Verlauf();
                Steuerung();

 
            links.OnSchlägerWurdeGetroffen += _OnSchlägerWurdeGetroffen;
            rechts.OnSchlägerWurdeGetroffen += _OnSchlägerWurdeGetroffen;
            ball.OnBandeWurdeGetroffen += Ball_OnBandeWurdeGetroffen;

        }

        private void Ball_OnBandeWurdeGetroffen()
        {
            SinusIstGespiegelt = !SinusIstGespiegelt;
        }

        // Die Steuerung muss in einem Thread erfolgen, es würde auch ohne Thread
        // gehen, jedoch würden dadurch mehr if schleifen entstehen, da man überprüfen muss,
        // ob zwei Tasten gleichzeitig gedrückt worden sind. Verbraucht etwas mehr Leistung
        // aber dafür besser in der Handhabung.
        async void Steuerung()
        {
            await Task.Run(() => {

                int BilderProSekunde = 30;

                while (true)
                {
                    Thread.Sleep(1000 / BilderProSekunde);

                    if (GedrückteTasten.Contains(Key.W))
                    {
                        Dispatcher.BeginInvoke(new Action(() => { links.YPosition -= 10;  }));
                        

                    }


                    if (GedrückteTasten.Contains(Key.S))
                    {
                        Dispatcher.BeginInvoke(new Action(() => { links.YPosition += 10; }));

                    }

                    if (GedrückteTasten.Contains(Key.Up))
                    {
                        Dispatcher.BeginInvoke(new Action(() => { rechts.YPosition -= 10; }));

                    }


                    if (GedrückteTasten.Contains(Key.Down))
                    {
                        Dispatcher.BeginInvoke(new Action(() => { rechts.YPosition += 10; }));
                    }


                    // Spielfeld.Focus(); NEIN!


                }
            });

        }

        // Füge die gedrückten Tasten in eine Liste ein
        void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (GedrückteTasten.Contains(e.Key) == false)
            {
                GedrückteTasten.Add(e.Key);
            }
        }

        // Entferne die Tasten von der Liste beim Loslassen
        void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (GedrückteTasten.Contains(e.Key))
            { 
                GedrückteTasten.Remove(e.Key);
            }
        }


        bool Richtung = false;
        
        Schläger MerkeDenVorherigenSchläger = null;

        private void _OnSchlägerWurdeGetroffen(Schläger schläger)
        {
            // Dieses Ereignis ändert den limes (+Unendlich, -Unendlich), wenn der Schläger getroffen worden ist.

            // Fehlerbehebung: Der Ball darf vom Selben Schläger nicht 2x getroffen werden, 
            // Passiert bei bestimmten Y-Werten/Einflugswinkel => Der Trick: wir merken uns den
            // geschlagenen schläger => Die Richtung darf ein weiteres mal nur geändert werden,
            // wenn ein anderer Schläger berührt wird.
            if (MerkeDenVorherigenSchläger != schläger)
            {
                MerkeDenVorherigenSchläger = schläger;
                Richtung = !Richtung;
                Geschwindigkeit.Value += 0.2; // Es wird schwieriger :)
            }
        }

        List<Key> GedrückteTasten = new List<Key>();


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(null, new Pen(Brushes.Black, 2), new Rect(0, 0, ActualWidth, Height));
        }


        bool SinusIstGespiegelt = false;

        // Hier spielt sich die Ballbewegung ab
        async void Verlauf()
        {
            await Task.Run(() =>
            {

                bool AschsenabschnittInvertieren = false; // Vorsicht! Bezieht sich auf die Slider richtung
                bool AmplitudeInvertieren = false; // Vorsicht! Bezieht sich auf die Slider richtung
                while (true)
                {
                    Thread.Sleep(1000 / 40); // "Bilder pro Sekunde..."

                    Dispatcher.BeginInvoke(new Action(() =>
                    {

                        if (ball.Margin.Left < 0) // Ball ist im Negativem
                        {
                            BallFliegtRaus("links");
                        }
                        else if (ball.Margin.Left > (Spielfeld.ActualWidth - ball.ActualWidth)) // Ball ist im Positiven aber nicht im Spielfeld
                        {
                            BallFliegtRaus("rechts");
                        }

                        if (Richtung == true)
                        {
                            ball.Margin = new Thickness(ball.Margin.Left+Geschwindigkeit.Value, ball.Margin.Top, ball.Margin.Right, ball.Margin.Bottom);
                        }
                        else
                        {
                            ball.Margin = new Thickness(ball.Margin.Left +Geschwindigkeit.Value*(-1), ball.Margin.Top, ball.Margin.Right, ball.Margin.Bottom);
                        }



                        // Slider ändern für die variablität
                        if (Achsenabschnitt.Value == Achsenabschnitt.Maximum)
                            AschsenabschnittInvertieren = false;
                        else if (Achsenabschnitt.Value == Achsenabschnitt.Minimum)
                            AschsenabschnittInvertieren = true;

                        
                        if (AschsenabschnittInvertieren == true)
                            Achsenabschnitt.Value += 2; 
                        else
                            Achsenabschnitt.Value -= 1;
                        


                        if (Amplitude.Value == Amplitude.Maximum)
                            AmplitudeInvertieren = false;
                        else if (Amplitude.Value == Amplitude.Minimum)
                            AmplitudeInvertieren = true;

                        if (AmplitudeInvertieren == true)
                            Amplitude.Value += 0.2;
                        else
                            Amplitude.Value -= 0.5;

                        

                        if(SinusIstGespiegelt == false) // Unzwar an der X-Achse !
                        {

                            // "Allgemeine Sinus Formel"
                            double sinusWert = Amplitude.Value * Math.Sin(Periode.Value * (2 * Math.PI) / (360 / ball.Margin.Left)) + Achsenabschnitt.Value;
                            

                            
                            ball.YPosition = (this.Height / 2) + sinusWert;


                            //SinusLabel.Content = Math.Round(sinusWert,1).ToString(); // Für die Vorzeichenüberprüfung 
                        }
                        else
                        {
                            // "Sinus Formel an der x-Achse gespiegelt" => wird für die Abpraller an der Bande benötigt
                            double sinusWert = (Amplitude.Value * Math.Sin(Periode.Value * (2 * Math.PI) / (360 / ball.Margin.Left))) * (-1)+ Achsenabschnitt.Value;

                            ball.YPosition = (this.Height / 2) + sinusWert;

                            //SinusLabel.Content = Math.Round(sinusWert, 2).ToString(); // Für die Vorzeichenüberprüfung 

                        }



                    }));
                }
            });
         }

        int PunkteLinks = 1;
        int PunkteRechts = 1;


        // "Ereignis" wenn der Ball rausfliegt => Punkt für Spieler
        void BallFliegtRaus(string FlugRichtung)
        {
            if (FlugRichtung == "links")
            {
                
                LabelPunkteRechts.Content = "Punkte Rechts: " + PunkteRechts.ToString();

                _OnSchlägerWurdeGetroffen(links); // Hier manuell auslösen => limes wird gedreht => Der linke Schläger darf nicht mehr berühren.

                PunkteRechts++;
            }
            else
            {
                LabelPunkteLinks.Content = "Punkte Links: " + PunkteLinks.ToString();

                _OnSchlägerWurdeGetroffen(rechts); // Hier manuell auslösen => limes wird gedreht => Der rechte Schläger darf nicht mehr berühren.

                PunkteLinks++;
            }


            Geschwindigkeit.Value = Geschwindigkeit.Minimum; // Geschwindigkeit zurücksetzen

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Spieler 1 benutzt W und S\nSpieler 2 benutzt Pfeil oben und Pfeil unten");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sinus Pong ist das Resultat eines Schulprojektes.");
        }
    }
}
