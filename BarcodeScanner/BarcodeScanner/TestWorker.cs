using BarcodeScanner;
using Soneta.Business;
using Soneta.Business.UI;
using Soneta.Towary;
using System;

[assembly: Worker(typeof(TestWorker), typeof(Towary))]

namespace BarcodeScanner
{
    public class TestWorker
    {


        // TODO -> Należy podmienić podany opis akcji na bardziej czytelny dla uzytkownika
        [Action("TestWorker/ToDo", Mode = ActionMode.SingleSession | ActionMode.ConfirmSave | ActionMode.Progress)]
        public MessageBoxInformation ToDo()
        {

            return new MessageBoxInformation("Czy wykonać operację ?")
            {
                Text = "Opis operacji",
                YesHandler = () => "Operacja została zakończona",
                NoHandler = () => "Operacja przerwana"
            };


        }
    }


}
