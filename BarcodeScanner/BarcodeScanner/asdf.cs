using BarcodeScanner;
using Soneta.Business;
using Soneta.Business.App;
using Soneta.Business.UI;


// Sposób w jaki należy zarejestrować page który będzie wyswietlany jako folderw interfejsie.
[assembly: FolderView("asdf",
    Priority = 100000,
    Description = "Niedziala",
    ObjectType = typeof(asdf),
    ObjectPage = "asdf.ogolne.pageform.xml",
    ReadOnlySession = false,
    ConfigSession = false
)]

namespace BarcodeScanner
{
    public class asdf
    {
        [Context]
        public Session Session { get; set; }

        [Context]
        public Login Login { get; set; }
    }
}
