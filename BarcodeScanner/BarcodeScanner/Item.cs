using Soneta.Business;
using Soneta.Towary;
using System;

namespace BarcodeScanner
{
    public class Item : ContextBase
    {
        private Quantity _qty = new Quantity(1.0);

        public Item(Context cx) : base(cx)
        {
            OnChanged(EventArgs.Empty);
        }

        public Item(string kod, string nazwa, string eAN, Context cx) : base (cx)
        {
            Kod = kod;
            Nazwa = nazwa;
            EAN = eAN;

            OnChanged(EventArgs.Empty);
        }

        public string Kod { get; set; }
        public string Nazwa { get; set; }
        public string EAN { get; set; }
        public Quantity Qty
        {
            get => _qty;
            set
            {
                _qty = value;
                OnChanged(EventArgs.Empty);
            }
        }
    }
}





