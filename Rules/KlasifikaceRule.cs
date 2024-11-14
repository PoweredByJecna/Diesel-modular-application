namespace Diesel_modular_application.KlasifikaceRule
{
    public static class KlasifikaceRule
    {
        public static int ZiskejVahu(this string Klasifikace)
        {
            var klasifikaceVaha =  new Dictionary<string,int>
            {
                {"A1",6},
                {"A2",5},
                {"B1",4},
                {"B2",3},
                {"B",2},
                {"C",2},
                {"D1",1}

            };

            return klasifikaceVaha.TryGetValue(Klasifikace,out int vaha) ? vaha:0; //pokud se nenajde žádná klasifikace, vrátí default hodnotu 0
        }

    }
}