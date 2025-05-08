

namespace NewsHeli;

/* wenn ein ped verhaftet wird während pursuit, dann audio
 * wenn ein ped stirbt während pursuit, dann audio _> viewer discretion is advised
 * 
 * im rnui menu allow media on scene ein median van zur scene rufen mit reporter und kamera mann
 * im menu einen media helicopoter anfrodern
 */
public class Main : Plugin
{
    public override void Initialize()
    {
        Game.LogTrivial("NewsHeli by Yasd loaded");
        Game.DisplayNotification($"~g~NewsHeli by Yasd {Assembly.GetExecutingAssembly().GetName().Version} loaded");

        CustomizationXml.Read();

        MainFiber.Start();

        //Menu.Start();
    }

    public override void Finally()
    {
        Game.LogTrivial("NewsHeli unloaded");

        MainFiber.SafeAbort();
    }
}