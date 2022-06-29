using BarcodeScanner;
using Soneta.Business;
using Soneta.Business.App;
using Soneta.Business.UI;
using Soneta.CRM;
using Soneta.Handel;
using Soneta.Magazyny;
using Soneta.Towary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BarcodeScanner
{
    public class PFTest : ContextBase, INewBarCode
    {
        private Item _focusedItem;
        private Item[] _selectedItems;

        public PFTest(Context context) : base(context) 
        {
            Items = new List<Item>();

            Items.Add(new Item("05022087001", "022087 ZESTAW KLUCZY TRZPIENIOWYCH SW1.5-10", "4013288104816", context));
            Items.Add(new Item("1655892", "0513.15 KLUCZ PŁASKI 13X15 /CAROLUS/", "4036548513159", context));
        }

        [Context]
        public Login Login { get; set; }

        public Item[] SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value;
                if(_selectedItems != null)
                Context.Set(_selectedItems);
            }
        }
        public Item FocusedItem
        {
            get => _focusedItem;
            set
            {
                _focusedItem = value;
                if(_focusedItem != null)
                Context.Set(_focusedItem);
            }
        }
        public string Code { get; set; }
        public Kontrahent Kontrahent { get; set; }
        public Magazyn Magazyn { get; set; }
        public List<Item> Items { get; set; }


        public void DeleteItem()
        {
            Items.Remove(FocusedItem);
            Context.Session.InvokeChanged();
        }

        private HandelModule GetHandelModule(Session session)
        {
            return HandelModule.GetInstance(session);
        }
        private TowaryModule GetTowaryModule(Session session)
        {
            return TowaryModule.GetInstance(session);
        }

        public object GeneratePZ()
        {
            return new MessageBoxInformation("Czy wygenerować PZ?")
            {
                Text = "Potwierdź wygenerowanie PZ",
                YesHandler = () =>
                {
                    using (Session session = Login.CreateSession(false, false))
                    {
                        using (ITransaction t = session.Logout(true))
                        {
                            var handelModule = GetHandelModule(session);
                            var towaryModule = GetTowaryModule(session);
                            DokumentHandlowy dh = new DokumentHandlowy();
                            handelModule.DokHandlowe.AddRow(dh);
                            dh.Definicja = handelModule.DefDokHandlowych.PrzyjęcieMagazynowe2;
                            dh.Magazyn = Magazyn;
                            dh.Kontrahent = Kontrahent;

                            foreach (var item in Items)
                            {
                                PozycjaDokHandlowego pozDH = new PozycjaDokHandlowego(dh);
                                handelModule.PozycjeDokHan.AddRow(pozDH);
                                pozDH.Towar = towaryModule.Towary.WgKodu[item.Kod];
                                pozDH.Ilosc = item.Qty;
                            }
                            t.Commit();
                        }
                        session.Save();
                    }
                    
                    return "Dokument PZ został wygenerowany.";
                },
                NoHandler = () => "Operacja przerwana"
            };
        }

        public object GeneratePZWithForm()
        {
            if(Session.CurrentTransaction != null)
            Session.CurrentTransaction.Dispose();
            
            using (ITransaction t = Session.Logout(true))
            {
                var handelModule = GetHandelModule(Session);
                var towaryModule = GetTowaryModule(Session);
                DokumentHandlowy dh = new DokumentHandlowy();
                handelModule.DokHandlowe.AddRow(dh);
                dh.Definicja = handelModule.DefDokHandlowych.PrzyjęcieMagazynowe2;
                dh.Magazyn = Magazyn;
                dh.Kontrahent = Kontrahent;

                foreach (var item in Items)
                {
                    PozycjaDokHandlowego pozDH = new PozycjaDokHandlowego(dh);
                    handelModule.PozycjeDokHan.AddRow(pozDH);
                    pozDH.Towar = towaryModule.Towary.WgKodu[item.Kod];
                    pozDH.Ilosc = item.Qty;
                }
                t.Commit();
                
                return new FormActionResult
                {
                    EditValue = dh,
                    CommittedHandler = ctx =>
                    {
                        return new MessageBoxInformation("Operacja zakończona");
                    }
                };
            }
            
        }

        public object Enter(Context cx, string code, double quantity)
        {
            Code = code;
            Towar towar = cx.Session.GetTowary().Towary.WgEAN[code].GetFirst();
            Items.Add(new Item(towar.Kod, towar.Nazwa, towar.EAN, cx));
            //OnChanged();

            cx.Session.InvokeChanged();

            return DBNull.Value;
        }
    }
}





