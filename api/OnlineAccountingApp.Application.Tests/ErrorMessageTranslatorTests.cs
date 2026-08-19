using OnlineAccountingApp.Domain.Exceptions;

namespace OnlineAccountingApp.Application.Tests;

public class ErrorMessageTranslatorTests
{
    [Theory]
    [InlineData("tr")]
    [InlineData("tr-TR")]
    [InlineData("TR")]
    public void Translate_ShouldReturnTurkishText_ForKnownMessage(string languageTag)
    {
        string result = ErrorMessageTranslator.Translate("Role not found.", languageTag);
        Assert.Equal("Rol bulunamadı.", result);
    }

    [Fact]
    public void Translate_ShouldReturnOriginalMessage_ForEnglishLanguage()
    {
        string result = ErrorMessageTranslator.Translate("Role not found.", "en");
        Assert.Equal("Role not found.", result);
    }

    [Fact]
    public void Translate_ShouldReturnOriginalMessage_WhenLanguageIsMissing()
    {
        string result = ErrorMessageTranslator.Translate("Role not found.", null);
        Assert.Equal("Role not found.", result);
    }

    [Fact]
    public void Translate_ShouldReturnOriginalMessage_ForUnknownMessage()
    {
        string result = ErrorMessageTranslator.Translate("Some message with no translation entry.", "tr");
        Assert.Equal("Some message with no translation entry.", result);
    }
}
