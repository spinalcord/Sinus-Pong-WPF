using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SinusPong
{
    public delegate void BandeWurdeGetroffen();

    internal class Ball : Border
    {
        public event BandeWurdeGetroffen OnBandeWurdeGetroffen;

        // Die Höhe des Objektes, in welchem der Ball drin is.

        public double ParentHöhe()
        {
            UIElement control = this.Parent as UIElement;
            FrameworkElement fe = control as FrameworkElement;
            if (fe == null) return 0;
                return ((FrameworkElement)control).ActualHeight;
        }
        // Die Breite des Objektes, in welchem der Ball drin ist.
        public double ParentBreite()
        {
            UIElement control = this.Parent as UIElement;
            FrameworkElement fe = control as FrameworkElement;
            if (fe == null) return 0;
            return ((FrameworkElement)control).ActualWidth;
        }

        // Immer diese Eigenschaft benutzen, damit das Programm erkennt das
        // der Ball nicht von oben und unten raus darf.
        private double _YPosition;
        public double YPosition
        {
            get
            {
                return this.Margin.Top;
            }
            set
            {
                if (value < 0) // Der Ball darf oben nicht rausfliegen
                {
                    value = 0;
                    OnBandeWurdeGetroffen?.Invoke();
                }
                else if (value > (ParentHöhe() - ActualHeight)) // Der Ball darf unten nicht rausfliegen
                {
                    value = (ParentHöhe() - ActualHeight);
                    OnBandeWurdeGetroffen?.Invoke();
                }

                // Übernehme die Werte, schließe die Korrektur mit ein.
                _YPosition = value;
                this.Margin = new System.Windows.Thickness(this.Margin.Left, _YPosition, this.Margin.Right, this.Margin.Bottom);
            }
        }


    }
}
