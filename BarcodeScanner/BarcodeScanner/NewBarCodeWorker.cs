using INewBarCodeExample;
using Soneta.Business;
using Soneta.Business.UI;
using Soneta.Handel;
using System;

[assembly: Worker(typeof(NewBarCodeWorker), typeof(DokumentHandlowy))]

namespace INewBarCodeExample
{
    public class NewBarCodeWorker
    {
        [Action("NewBarCodeWorker",
            Priority = 2200,
            Icon = ActionIcon.LightBulb,
            Mode = ActionMode.SingleSession,
            Target = ActionTarget.ToolbarWithText)]
        public object Test(Context context)
        {
            var @params = new CodeParams(context);
            return new FormActionResult()
            {
                EditValue = @params,
                Context = context,
                CommittedHandler = cx =>
                {
                    // Dalsza obsługa workera
                    return $"Code: {@params.Code}";
                }
            };
        }
    }

    [DataFormStyle(UseDialog = true)] // Opcjonalne modyfikacje wyglądu formularza
    public class CodeParams : ContextBase, INewBarCode
    {
        public CodeParams(Context context) : base(context)
        {
        }

        public string Code { get; set; }

        object INewBarCode.Enter(Context cx, string code, double quantity)
        {
            new Log("test", true).WriteLine($"Code entered: {code}");
            Code = code;
            OnChanged();
            return DBNull.Value;
            // Zwrócenie null powoduje, że po wykonaniu Enter kod będzie dalej przetwarzany przez enovę
            // Zwrócenie DBNull.Value powoduje, że obsługa kodu kończy się w tym miejscu
        }
    }
}
