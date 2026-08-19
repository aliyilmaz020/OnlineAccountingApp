namespace OnlineAccountingApp.Domain.Exceptions;

/// <summary>
/// Translates the fixed English error messages thrown across the app (BusinessException /
/// ValidationException top-level messages, plus the two hardcoded 401/403 pipeline messages in
/// ApiConfig.Authentication.cs) to Turkish, keyed by exact English text rather than error code -
/// several distinct messages share the same AppErrorCodes value (e.g. Role.NotFound is used for
/// both "Role not found." and "User not found."), so the code alone can't disambiguate.
/// </summary>
/// <remarks>
/// Scope: only these fixed top-level messages are translated. Per-field validation messages
/// (FluentValidation rule messages, Identity's IdentityResult errors surfaced via the `errors`
/// dictionary) and any message built from interpolated runtime data (e.g. "Company 'xyz' was not
/// found.") are left in English - translating those would mean touching every validator/Identity
/// error describer individually, a much larger follow-up.
/// </remarks>
public static class ErrorMessageTranslator
{
    private static readonly Dictionary<string, string> Turkish = new()
    {
        ["One or more validation errors occurred."] = "Bir veya daha fazla doğrulama hatası oluştu.",
        ["An unexpected error occurred."] = "Beklenmeyen bir hata oluştu.",
        ["Authentication is required to access this resource."] = "Bu kaynağa erişmek için kimlik doğrulaması gerekiyor.",
        ["You do not have permission to access this resource."] = "Bu kaynağa erişim izniniz yok.",
        ["You do not have access to this company."] = "Bu şirkete erişiminiz yok.",
        // Matches ICompanyContext.HeaderName ("X-Company-Id"), interpolated in Application/Persistence -
        // hardcoded here since Domain cannot reference those layers.
        ["The 'X-Company-Id' header is required for this operation."] = "Bu işlem için 'X-Company-Id' başlığı gereklidir.",
        ["Email or password is incorrect."] = "E-posta veya şifre hatalı.",
        ["A user with the same email already exists."] = "Bu e-posta ile kayıtlı bir kullanıcı zaten var.",
        ["Refresh token is invalid, expired or already used."] = "Yenileme jetonu geçersiz, süresi dolmuş veya zaten kullanılmış.",
        ["You do not have permission to update this company."] = "Bu şirketi güncelleme izniniz yok.",
        ["You do not have permission to delete this company."] = "Bu şirketi silme izniniz yok.",
        ["You do not have permission to create a company."] = "Şirket oluşturma izniniz yok.",
        ["Role not found."] = "Rol bulunamadı.",
        ["A role with the same name already exists."] = "Aynı isimde bir rol zaten var.",
        ["A role with the same code already exists."] = "Aynı koda sahip bir rol zaten var.",
        ["User not found."] = "Kullanıcı bulunamadı.",
        ["Company not found."] = "Şirket bulunamadı.",
        ["A company with the same name already exists."] = "Aynı isimde bir şirket zaten var.",
        ["Uniform chart of account not found."] = "Tekdüzen hesap bulunamadı.",
        ["A uniform chart of account with the same code already exists."] = "Aynı koda sahip bir tekdüzen hesap zaten var.",
        ["Entity not found."] = "Kayıt bulunamadı.",
        ["Entity already exists."] = "Kayıt zaten mevcut.",
        ["This role is already assigned to this main role."] = "Bu rol zaten bu ana role atanmış.",
        ["Main role - role relationship not found."] = "Ana rol - rol ilişkisi bulunamadı.",
        ["Main role - user relationship not found."] = "Ana rol - kullanıcı ilişkisi bulunamadı.",
        ["This user already has this main role in this company."] = "Bu kullanıcı bu şirkette zaten bu ana role sahip.",
        ["Main role not found."] = "Ana rol bulunamadı.",
        ["A main role with the same title already exists for this company."] = "Bu şirket için aynı başlığa sahip bir ana rol zaten var.",
        ["The new password and confirmation do not match."] = "Yeni şifre ve onay şifresi eşleşmiyor.",
    };

    /// <summary>Returns the Turkish text for <paramref name="englishMessage"/> if the language tag starts with "tr", else the original message unchanged.</summary>
    public static string Translate(string englishMessage, string? languageTag)
    {
        if (languageTag is not null
            && languageTag.StartsWith("tr", StringComparison.OrdinalIgnoreCase)
            && Turkish.TryGetValue(englishMessage, out string? translated))
        {
            return translated;
        }

        return englishMessage;
    }
}
