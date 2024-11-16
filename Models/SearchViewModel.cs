public class SearchViewModel
{
    // Seznam položek, které se mají zobrazit (např. odstávky nebo lokality)
    public IEnumerable<object> Items { get; set; }

    // Aktuální stránka
    public int CurrentPage { get; set; }

    // Počet záznamů na stránku
    public int PageSize { get; set; }

    // Celkový počet záznamů
    public int TotalItems { get; set; }

    // Výpočet celkového počtu stránek (pokud je to potřeba)
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}
