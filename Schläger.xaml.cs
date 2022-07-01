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


    public delegate void SchlägerWurdeGetroffen(Schläger schläger);


    /// <summary>
    /// Interaktionslogik für Schläger.xaml
    /// </summary>
    public partial class Schläger : UserControl
    {
        public Schläger()
        {
            InitializeComponent();

            if (Application.Current is App)
                Verlauf();


        }

        public double ParentBreite()
        {
            UIElement uie = this.Parent as UIElement;
            FrameworkElement fElement = uie as FrameworkElement;
            if (fElement == null) return 0;
            return ((FrameworkElement)uie).ActualWidth;
        }

        public double ParentHöhe()
        {
            UIElement uie = this.Parent as UIElement;
            FrameworkElement fElement = uie as FrameworkElement;
            if (fElement == null) return 0;
            return ((FrameworkElement)uie).ActualHeight;
        }

        private double _YPosition;

        public double YPosition
        {
            get
            {
                return this.Margin.Top;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > (ParentHöhe() - ActualHeight))
                    value = (ParentHöhe() - ActualHeight);

                _YPosition = value;
                this.Margin = new System.Windows.Thickness(this.Margin.Left, _YPosition, this.Margin.Right, this.Margin.Bottom);
            }
        }


        async void Verlauf()
        {
            await Task.Run(() =>
            {
                    while(true)
                    {
                        Thread.Sleep(1000 / 40);

                        Dispatcher.BeginInvoke(new Action(() => {

                            // Wir wollen, dass der Schläger sich an die Fensterbreite anpasst!
                            // Beim linken Schläger ist das kein Problem, der Nullpunkt liegt Oben Links.
                            // Aufgrund der Layout Einstellung ergibt sich jedoch für den Rechten Schläger mit dem HorizontalAlignment.Right
                            // ein anderer Nullpunkt! Der Nullpunkt liegt Oben Rechts. Möchte Man ein Rect zeichnen, muss man sich jedoch von
                            // oben Links anähren.
                            // 
                            // Manuelle rechnung für den rechten Schläger: BreiteDesÜbergeordnetenElements - AktuelleBreite - AbstandVomNullPunktRechts
                            if (HorizontalAlignment == HorizontalAlignment.Right) // Logischerweise verschiebt sich der Rechte Schläger nach rechts beim Fensterskallieren.
                                HitBox = new Rect(ParentBreite() - this.ActualWidth - Margin.Right, Margin.Top, ActualWidth, ActualHeight); // Erstelle eine Hitbox für diese Instanz
                            else // Gewohnte Rechnung
                                HitBox = new Rect(Margin.Left, Margin.Top, ActualWidth, ActualHeight); // Erstelle eine Hitbox für diese Instanz

                        foreach (var item in (this.Parent as Grid).Children.OfType<Ball>()) // Foreach => suche alle Schläger im übergeordneten Element 
                        {
                                Rect HitBoxFremd = new Rect(item.Margin.Left, item.Margin.Top, item.ActualWidth, item.ActualHeight); // Erstelle ein Rect => Ein Feld an den man die Schnittmenge überprüfen kann.

                                if (HitBox.IntersectsWith(HitBoxFremd)) // Collision mit fremden HitBox
                                {
                                    OnSchlägerWurdeGetroffen(this); // Löse ein Event aus!
                                }
                        }
                        }));

                }
            });
        }



        private Rect _HitBox;

        public Rect HitBox
        {
            get { return _HitBox; }
            set { _HitBox = value; }
        }

        public event SchlägerWurdeGetroffen OnSchlägerWurdeGetroffen;


        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            // nein

            //HitBox.IntersectsWith();
        }
    }
}
