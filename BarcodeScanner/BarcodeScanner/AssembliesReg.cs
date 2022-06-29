using BarcodeScanner;
using Soneta.Business;
using Soneta.Business.UI;
using Soneta.Handel;


// Sposób w jaki należy zarejestrować page który będzie wyswietlany jako folderw interfejsie.
[assembly: FolderView("BarcodeScanner",
    Priority = 2,
    Description = "BarcodeScanner",
    ObjectType = typeof(PFTest),
    ObjectPage = "PFTest.ogolne.pageform.xml",
    ReadOnlySession = false,
    ConfigSession = false,
    IconName = "kod_kreskowy"
)]

[assembly: Worker(typeof(ScanItemWorker), typeof(DokumentHandlowy))]