using BarcodeScanner;
using Soneta.Business;
using Soneta.Business.App;
using Soneta.Business.UI;
using Soneta.Handel;
using Soneta.Towary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BarcodeScanner
{
    public class PFTest : INewBarCode
    {
        private Item _focusedItem;
        private Item[] _selectedItems;

        public PFTest(Context context) //: base(context)
        {
            Code = "1234";
            Items = new List<Item>();

            Items.Add(new Item("CBA", "FED", "321", context));
            Items.Add(new Item("ABC", "DEF", "123", context));
        }

        [Context]
        public Session Session { get; set; }

        [Context]
        public Login Login { get; set; }
        [Context]
        public Context Context { get; set; }

        public Item[] SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value;
                Context.Set(_selectedItems);
            }
        }
        public Item FocusedItem
        {
            get => _focusedItem;
            set
            {
                _focusedItem = value;
                Context.Set(_focusedItem);
            }
        }
        public string Code { get; set; }
        public List<Item> Items { get; set; }

        public void DeleteItem()
        {
            Items.Remove(FocusedItem);
            Context.Session.InvokeChanged();
        }
        public object Enter(Context cx, string code, double quantity)
        {

            new Log("test", true).WriteLine($"Code entered: {code}");
            Code = code;
            Towar towar = cx.Session.GetTowary().Towary.WgEAN[code].GetFirst();
            Items.Add(new Item(towar.Kod, towar.Nazwa, towar.EAN, cx));
            //OnChanged();

            cx.Session.InvokeChanged();
            
            return FormAction.None;
        }
        

    }
    public class Item : ContextBase
    {
        public Item(Context context) : base(context)
        {
        }

        public Item(string kod, string nazwa, string eAN, Context context) : base(context)
        {
            Kod = kod;
            Nazwa = nazwa;
            EAN = eAN;
        }

        public string Kod { get; set; }
        public string Nazwa { get; set; }
        public string EAN { get; set; }
    }

}

    

