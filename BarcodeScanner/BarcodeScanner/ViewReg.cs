using BarcodeScanner;
using Soneta.Business.UI;


// Sposób w jaki należy zarejestrować page który będzie wyswietlany jako folderw interfejsie.
[assembly: FolderView("BarcodeScanner",
    Priority = 2,
    Description = "BarcodeScanner",
    ObjectType = typeof(PFTest),
    ObjectPage = "PFTest.ogolne.pageform.xml",
    ReadOnlySession = false,
    ConfigSession = false
)]