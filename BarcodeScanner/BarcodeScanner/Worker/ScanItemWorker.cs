using BarcodeScanner;
using Soneta.Business;
using Soneta.Business.App;
using Soneta.Business.UI;
using Soneta.Commands;
using Soneta.Handel;
using Soneta.Towary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BarcodeScanner
{
    public class ScanItemWorker
    {
        [Context]
        public DokumentHandlowy Dokument { get; set; }
        [Context]
        public Login Login { get; set; }
        [Context]
        public Session Session { get; set; }

        [Action("Skanuj towar", Mode = ActionMode.IsolatedSession, Icon = ActionIcon.CodeView)]
        public object ScanItem(Context context)
        {
            var @params = new ScanItemWorkerParams(context);
            return new FormActionResult()
            {
                EditValue = @params,
                Context = context,
                CommittedHandler = cx =>
                {
                    try
                    {
                        foreach (var item in @params.Items)
                        {
                            using (ITransaction t = @params.Session.Logout(true))
                            {
                                DokumentHandlowy dokHan = HandelModule.GetInstance(t).DokHandlowe.Rows.FirstOrDefault(e => e.ID == Dokument.ID) as DokumentHandlowy;

                                PozycjaDokHandlowego pozycja = new PozycjaDokHandlowego(dokHan);
                                HandelModule.GetInstance(t).PozycjeDokHan.AddRow(pozycja);
                                pozycja.Towar = TowaryModule.GetInstance(t).Towary.WgKodu[item.Kod];
                                pozycja.Ilosc = item.Qty;
                                t.Commit();
                            }
                        }
                        @params.Items.Clear();
                        Dokument.Session.InvokeChanged();

                        return FormAction.None;
                    }
                    catch(Exception e)
                    {
                        return e.Message;
                    }
                }
            };

        }
    }

    [DataFormStyle(UseDialog = true)]
    public class ScanItemWorkerParams : ContextBase, INewBarCode
    {
        private Item _focusedItem;
        private Item[] _selectedItems;

        public ScanItemWorkerParams(Context context) : base(context)
        {
            Items = new List<Item>();

            Items.Add(new Item("05022087001", "022087 ZESTAW KLUCZY TRZPIENIOWYCH SW1.5-10", "4013288104816", context));
            Items.Add(new Item("1655892", "0513.15 KLUCZ PŁASKI 13X15 /CAROLUS/", "4036548513159", context));
        }

        public Item[] SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value;
                if (_focusedItem != null)
                    Context.Set(_selectedItems);
                OnChanged();
            }
        }
        public Item FocusedItem
        {
            get => _focusedItem;
            set
            {
                _focusedItem = value;
                if (_focusedItem != null)
                    Context.Set(_focusedItem);
                OnChanged();
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
            Code = code;
            Towar towar = cx.Session.GetTowary().Towary.WgEAN[code].GetFirst();
            Items.Add(new Item(towar.Kod, towar.Nazwa, towar.EAN, cx));
            OnChanged();

            return DBNull.Value;
        }


    }

}
