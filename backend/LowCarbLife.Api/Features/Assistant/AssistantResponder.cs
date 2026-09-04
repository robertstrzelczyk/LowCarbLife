namespace LowCarbLife.Api.Features.Assistant;

public static class AssistantResponder
{
    public static string Reply(string? text, string? language)
    {
        var english = string.Equals(language, "en", StringComparison.OrdinalIgnoreCase);
        var message = (text ?? string.Empty).Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(message))
        {
            return english
                ? "Write a short question — for example about keto, recipes, the blog, or contact."
                : "Napisz krótkie pytanie — na przykład o keto, przepisy, blog albo kontakt.";
        }

        if (ContainsAny(message, "przepis", "recipe", "gotow", "cook", "obiad", "śniadan", "sniadan", "kolacj", "dinner", "breakfast", "lunch"))
        {
            return english
                ? "Recipes are under Recipes in the menu: Keto and Low-carb. You can filter breakfast, lunch and dinner, then open a dish for the method and a YouTube video if there is one."
                : "Przepisy znajdziesz w menu Przepisy: Keto i Lowcarb. Możesz filtrować śniadanie, obiad i kolację, a w szczegółach jest przygotowanie i film na YouTube, jeśli jest.";
        }

        if (ContainsAny(message, "keto"))
        {
            return english
                ? "Keto here means very low carb, higher fat meals. Start with Keto recipes and the blog notes about how the first weeks felt while running."
                : "Keto to tu posiłki bardzo niskowęglowodanowe, z większą ilością tłuszczu. Zacznij od Keto przepisów i wpisów na blogu o pierwszych tygodniach i bieganiu.";
        }

        if (ContainsAny(message, "lowcarb", "low-carb", "low carb", "niskowegl", "niskowęgl"))
        {
            return english
                ? "Low-carb is a bit more flexible than keto. Open Low-carb recipes for meals that still keep carbs in check after a run."
                : "Lowcarb jest trochę luźniejsze niż keto. Wejdź w Lowcarb przepisy — dania, które trzymają węgle w ryzach po treningu.";
        }

        if (ContainsAny(message, "blog", "wpis", "post"))
        {
            return english
                ? "The blog is a journal of a low-carb lifestyle and running — feelings, training, no miracle promises. You will find it in the Blog tab."
                : "Blog to dziennik niskowęglowego stylu życia i biegania — odczucia, treningi, bez cudownych obietnic. Znajdziesz go w zakładce Blog.";
        }

        if (ContainsAny(message, "bieg", "run", "trening", "train"))
        {
            return english
                ? "Running sits next to the diet here: easy pace, electrolytes, less sugary gels. There is more in the blog posts."
                : "Bieganie idzie tu w parze z dietą: spokojne tempo, elektrolity, mniej słodkich żeli. Więcej jest we wpisach na blogu.";
        }

        if (ContainsAny(message, "kontakt", "contact", "telefon", "phone", "mail", "email", "whatsapp", "instagram"))
        {
            return english
                ? "You can reach us from the footer or the Contact page. For now the details are examples: phone +48 500 123 456, email kontakt@lowcarblife.pl, plus WhatsApp and Instagram."
                : "Kontakt jest w stopce i na stronie Kontakt. Na razie to przykładowe dane: telefon +48 500 123 456, email kontakt@lowcarblife.pl oraz WhatsApp i Instagram.";
        }

        if (ContainsAny(message, "admin", "login", "logowan", "hasło", "haslo", "password", "rejestr"))
        {
            return english
                ? "Guests can browse. After sign-up you get a user account. An admin can add posts and recipes — those buttons show up only when you are logged in as admin."
                : "Gość może przeglądać. Po rejestracji masz konto użytkownika. Admin dodaje wpisy i przepisy — przyciski widać dopiero po zalogowaniu jako admin.";
        }

        if (ContainsAny(message, "cześć", "czesc", "hej", "siema", "hello", "hi", "hey"))
        {
            return english
                ? "Hello! Ask me about keto, recipes, the blog, running, or how to get in touch."
                : "Cześć! Zapytaj o keto, przepisy, blog, bieganie albo jak się z nami skontaktować.";
        }

        return english
            ? "I can help with keto, low-carb recipes, the blog, running, and contact. Try a short question like “keto breakfast” or “how do I get in touch?”."
            : "Mogę pomóc w tematach keto, przepisów lowcarb, bloga, biegania i kontaktu. Spróbuj krótkiego pytania, np. „śniadanie keto” albo „jak się z wami skontaktować?”.";
    }

    private static bool ContainsAny(string message, params string[] needles) =>
        needles.Any(needle => message.Contains(needle, StringComparison.Ordinal));
}
