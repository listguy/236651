namespace Test.Project;


public class LegalClass
{
    private const string LEGAL_GLOBAL = "Hello World";
    public const string illegal_GLOBAL = "Hello World";
    private const string ILLEGAL_GLOBAL1 = "Hello World";
}
public class illegalClass
{
    private string BADname;
    private string badName2_;
    private string goodName;
    private string goodName2;
    public string goodProperty => "Abale";
    public string BadProperty => "Abale2";
    void badMethod() {}
    void GoodMethod(string Papi)
    {
        var Aaa = 1;
        int BBB = 2;
        int cCC = 3;
        int dDd = 4;
        int eee = 5;
    }

    string A(string good)
    {
        return "    ";
    }
}