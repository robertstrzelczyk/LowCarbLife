using LowCarbLife.Api.Features.Assistant;

namespace LowCarbLife.Api.Tests.Assistant;

public class AssistantResponderTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Reply_EmptyMessage_AsksForShortQuestion(string? text)
    {
        Assert.Contains("Napisz krótkie pytanie", AssistantResponder.Reply(text, "pl"));
        Assert.Contains("Write a short question", AssistantResponder.Reply(text, "en"));
    }

    [Theory]
    [InlineData("jakie macie przepisy?", "Przepisy znajdziesz")]
    [InlineData("co na obiad", "Przepisy znajdziesz")]
    [InlineData("keto", "Keto to tu")]
    [InlineData("lowcarb", "Lowcarb jest")]
    [InlineData("pokaż blog", "Blog to dziennik")]
    [InlineData("bieganie", "Bieganie idzie")]
    [InlineData("jak się skontaktować, kontakt", "Kontakt jest w stopce")]
    [InlineData("jak się zalogować, logowanie", "Gość może przeglądać")]
    [InlineData("hej", "Cześć! Zapytaj o keto")]
    [InlineData("zupełnie coś innego", "Mogę pomóc")]
    public void Reply_Polish_MatchesTopic(string text, string expectedFragment)
    {
        Assert.Contains(expectedFragment, AssistantResponder.Reply(text, "pl"));
    }

    [Theory]
    [InlineData("any recipes?", "Recipes are under")]
    [InlineData("what about dinner", "Recipes are under")]
    [InlineData("keto", "Keto here means")]
    [InlineData("low-carb", "Low-carb is a bit")]
    [InlineData("tell me about the blog", "The blog is a journal")]
    [InlineData("running", "Running sits")]
    [InlineData("contact", "You can reach us")]
    [InlineData("login", "Guests can browse")]
    [InlineData("hello", "Hello! Ask me")]
    [InlineData("weather forecast tomorrow", "I can help with")]
    public void Reply_English_MatchesTopic(string text, string expectedFragment)
    {
        Assert.Contains(expectedFragment, AssistantResponder.Reply(text, "en"));
    }

    [Fact]
    public void Reply_IgnoresCaseOfMessage()
    {
        Assert.Equal(
            AssistantResponder.Reply("keto", "pl"),
            AssistantResponder.Reply("  KETO  ", "pl"));
    }

    [Theory]
    [InlineData("EN")]
    [InlineData("En")]
    public void Reply_LanguageIsCaseInsensitive(string language)
    {
        Assert.Contains("Keto here means", AssistantResponder.Reply("keto", language));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("de")]
    public void Reply_UnknownLanguage_FallsBackToPolish(string? language)
    {
        Assert.Contains("Keto to tu", AssistantResponder.Reply("keto", language));
    }

    [Fact]
    public void Reply_RecipesTakePriorityOverKeto()
    {
        var reply = AssistantResponder.Reply("przepis keto", "pl");

        Assert.Contains("Przepisy znajdziesz", reply);
        Assert.DoesNotContain("Keto to tu", reply);
    }
}
