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

            Items.Add(new Item("05022087001", "022087 ZESTAW KLUCZY TRZPIENIOWYCH SW1.5-10", "4013288104816", context));
            Items.Add(new Item("1655892", "0513.15 KLUCZ PŁASKI 13X15 /CAROLUS/", "4036548513159", context));
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

        private HandelModule GetHandelModule()
        {
            return Context.Session.GetHandel();
        }
        private TowaryModule GetTowaryModule()
        {
            return Context.Session.GetTowary();
        }

        public object GeneratePZ()
        {
            var handelModule = Context.Session.GetHandel();
            var towaryModule = Context.Session.GetTowary();
            using (ITransaction t = Session.Logout(true))
            {
                DokumentHandlowy dh = new DokumentHandlowy();
                handelModule.DokHandlowe.AddRow(dh);
                dh.Definicja = handelModule.DefDokHandlowych.PrzyjęcieMagazynowe2;
                dh.Magazyn = GetHandelModule().Magazyny.Magazyny.Firma;
                //CreatePozycje(Items, dh);

                foreach (var item in Items)
                {
                    PozycjaDokHandlowego pozDH = new PozycjaDokHandlowego(dh);
                    handelModule.PozycjeDokHan.AddRow(pozDH);
                    pozDH.Towar = towaryModule.Towary.WgKodu[item.Kod];
                }

                t.Commit();
                return new FormActionResult
                {
                    EditValue = dh
                };
            }

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



