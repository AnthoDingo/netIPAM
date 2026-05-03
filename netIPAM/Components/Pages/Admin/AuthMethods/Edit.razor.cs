using System.Text.Json;
using System.Text.Json.Serialization;

namespace netIPAM.Components.Pages.Admin.AuthMethods
{
    public partial class Edit
    {
        [Inject] private AuthMethodService Methods { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }

        private UserAuthMethod? _method;
        private string? _error;
        private bool _isNew => Id is null;

        private HttpParams   _http   = new();
        private LdapParams   _ldap   = new();
        private AdParams     _ad     = new();
        private RadiusParams _radius = new();
        private Saml2Params  _saml   = new();

        private static readonly JsonSerializerOptions _json = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented          = true,
        };

        protected override async Task OnParametersSetAsync()
        {
            if (_isNew)
            {
                _method = new UserAuthMethod { Type = "local", Protected = "No" };
            }
            else
            {
                _method = await Methods.GetAsync(Id!.Value);
                if (_method is null) { Nav.NavigateTo("/admin/authmethods"); return; }
                DeserializeParams(_method.Type, _method.Params);
            }
        }

        private void OnTypeChanged()
        {
            _http = new(); _ldap = new(); _ad = new(); _radius = new(); _saml = new();
            switch (_method!.Type)
            {
                case "LDAP":
                    _ldap.Port = 389; _ldap.UserAttr = "uid"; _ldap.MailAttr = "mail";
                    _ldap.VerifyCert = true; _ldap.MemberAttr = "memberUid"; _ldap.GroupAttr = "cn";
                    break;
                case "AD":
                    _ad.Port = 389; _ad.VerifyCert = true;
                    break;
                case "Radius":
                    _radius.Port = 1812; _radius.Timeout = 5; _radius.Retry = 3; _radius.Protocol = "PAP";
                    break;
                case "SAML2":
                    _saml.NameIdFormat = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";
                    _saml.AttrUsername = "uid"; _saml.AttrEmail = "mail";
                    break;
            }
        }

        private void DeserializeParams(string type, string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) { OnTypeChanged(); return; }
            try
            {
                switch (type)
                {
                    case "http":   _http   = JsonSerializer.Deserialize<HttpParams>(json,   _json) ?? new(); break;
                    case "LDAP":   _ldap   = JsonSerializer.Deserialize<LdapParams>(json,   _json) ?? new(); break;
                    case "AD":     _ad     = JsonSerializer.Deserialize<AdParams>(json,     _json) ?? new(); break;
                    case "Radius": _radius = JsonSerializer.Deserialize<RadiusParams>(json, _json) ?? new(); break;
                    case "SAML2":  _saml   = JsonSerializer.Deserialize<Saml2Params>(json,  _json) ?? new(); break;
                }
            }
            catch { OnTypeChanged(); }
        }

        private string? SerializeParams() => _method!.Type switch
        {
            "http"   => JsonSerializer.Serialize(_http,   _json),
            "LDAP"   => JsonSerializer.Serialize(_ldap,   _json),
            "AD"     => JsonSerializer.Serialize(_ad,     _json),
            "Radius" => JsonSerializer.Serialize(_radius, _json),
            "SAML2"  => JsonSerializer.Serialize(_saml,   _json),
            _        => null,
        };

        private string? Validate() => _method!.Type switch
        {
            "LDAP"   when string.IsNullOrWhiteSpace(_ldap.Hostname)     => "LDAP: Hostname is required.",
            "LDAP"   when string.IsNullOrWhiteSpace(_ldap.UserBaseDn)   => "LDAP: User base DN is required.",
            "AD"     when string.IsNullOrWhiteSpace(_ad.Hostname)       => "AD: Hostname is required.",
            "AD"     when string.IsNullOrWhiteSpace(_ad.Domain)         => "AD: Domain is required.",
            "AD"     when string.IsNullOrWhiteSpace(_ad.BindUser)       => "AD: Bind account is required.",
            "AD"     when string.IsNullOrWhiteSpace(_ad.BaseDn)         => "AD: Base DN is required.",
            "Radius" when string.IsNullOrWhiteSpace(_radius.Hostname)   => "RADIUS: Hostname is required.",
            "Radius" when string.IsNullOrWhiteSpace(_radius.Secret)     => "RADIUS: Shared secret is required.",
            "SAML2"  when string.IsNullOrWhiteSpace(_saml.IdpEntityId)  => "SAML2: IdP Entity ID is required.",
            "SAML2"  when string.IsNullOrWhiteSpace(_saml.IdpSsoUrl)    => "SAML2: IdP SSO URL is required.",
            "SAML2"  when string.IsNullOrWhiteSpace(_saml.IdpCert)      => "SAML2: IdP certificate is required.",
            "SAML2"  when string.IsNullOrWhiteSpace(_saml.SpEntityId)   => "SAML2: SP Entity ID is required.",
            "SAML2"  when string.IsNullOrWhiteSpace(_saml.SpAcsUrl)     => "SAML2: SP ACS URL is required.",
            "SAML2"  when string.IsNullOrWhiteSpace(_saml.AttrUsername) => "SAML2: Username attribute is required.",
            _ => null,
        };

        private async Task Save()
        {
            if (_method is null) return;
            _error = Validate();
            if (_error is not null) return;

            _method.Params = SerializeParams();

            if (_isNew) await Methods.CreateAsync(_method);
            else        await Methods.UpdateAsync(_method);

            Nav.NavigateTo("/admin/authmethods");
        }

        // ── Modèles de paramètres ──────────────────────────────────────────────

        public class HttpParams
        {
            [JsonPropertyName("auto_create")]  public string AutoCreate  { get; set; } = "false";
            [JsonPropertyName("default_role")] public string DefaultRole { get; set; } = "Guest";
        }

        public class LdapParams
        {
            [JsonPropertyName("hostname")]      public string  Hostname    { get; set; } = string.Empty;
            [JsonPropertyName("port")]          public int     Port        { get; set; } = 389;
            [JsonPropertyName("use_ssl")]       public bool    UseSsl      { get; set; }
            [JsonPropertyName("use_starttls")]  public bool    UseStartTls { get; set; }
            [JsonPropertyName("verify_cert")]   public bool    VerifyCert  { get; set; } = true;
            [JsonPropertyName("bind_dn")]       public string? BindDn      { get; set; }
            [JsonPropertyName("bind_pass")]     public string? BindPass    { get; set; }
            [JsonPropertyName("user_base_dn")]  public string  UserBaseDn  { get; set; } = string.Empty;
            [JsonPropertyName("user_attr")]     public string  UserAttr    { get; set; } = "uid";
            [JsonPropertyName("mail_attr")]     public string  MailAttr    { get; set; } = "mail";
            [JsonPropertyName("user_filter")]   public string? UserFilter  { get; set; }
            [JsonPropertyName("group_base_dn")] public string? GroupBaseDn { get; set; }
            [JsonPropertyName("group_attr")]    public string? GroupAttr   { get; set; }
            [JsonPropertyName("member_attr")]   public string? MemberAttr  { get; set; }
            [JsonPropertyName("admin_group")]   public string? AdminGroup  { get; set; }
        }

        public class AdParams
        {
            [JsonPropertyName("hostname")]     public string  Hostname    { get; set; } = string.Empty;
            [JsonPropertyName("port")]         public int     Port        { get; set; } = 389;
            [JsonPropertyName("domain")]       public string  Domain      { get; set; } = string.Empty;
            [JsonPropertyName("netbios_name")] public string? NetBiosName { get; set; }
            [JsonPropertyName("use_ssl")]      public bool    UseSsl      { get; set; }
            [JsonPropertyName("verify_cert")]  public bool    VerifyCert  { get; set; } = true;
            [JsonPropertyName("bind_user")]    public string  BindUser    { get; set; } = string.Empty;
            [JsonPropertyName("bind_pass")]    public string? BindPass    { get; set; }
            [JsonPropertyName("base_dn")]      public string  BaseDn      { get; set; } = string.Empty;
            [JsonPropertyName("user_ou")]      public string? UserOu      { get; set; }
            [JsonPropertyName("user_filter")]  public string? UserFilter  { get; set; }
            [JsonPropertyName("admin_group")]  public string? AdminGroup  { get; set; }
        }

        public class RadiusParams
        {
            [JsonPropertyName("hostname")]          public string  Hostname         { get; set; } = string.Empty;
            [JsonPropertyName("port")]              public int     Port             { get; set; } = 1812;
            [JsonPropertyName("secret")]            public string  Secret           { get; set; } = string.Empty;
            [JsonPropertyName("timeout")]           public int     Timeout          { get; set; } = 5;
            [JsonPropertyName("retry")]             public int     Retry            { get; set; } = 3;
            [JsonPropertyName("protocol")]          public string  Protocol         { get; set; } = "PAP";
            [JsonPropertyName("nas_id")]            public string? NasId            { get; set; }
            [JsonPropertyName("nas_ip")]            public string? NasIp            { get; set; }
            [JsonPropertyName("failover_hostname")] public string? FailoverHostname { get; set; }
        }

        public class Saml2Params
        {
            [JsonPropertyName("idp_entity_id")]     public string  IdpEntityId     { get; set; } = string.Empty;
            [JsonPropertyName("idp_sso_url")]       public string  IdpSsoUrl       { get; set; } = string.Empty;
            [JsonPropertyName("idp_slo_url")]       public string? IdpSloUrl       { get; set; }
            [JsonPropertyName("idp_cert")]          public string  IdpCert         { get; set; } = string.Empty;
            [JsonPropertyName("sp_entity_id")]      public string  SpEntityId      { get; set; } = string.Empty;
            [JsonPropertyName("sp_acs_url")]        public string  SpAcsUrl        { get; set; } = string.Empty;
            [JsonPropertyName("sp_cert")]           public string? SpCert          { get; set; }
            [JsonPropertyName("sp_private_key")]    public string? SpPrivateKey    { get; set; }
            [JsonPropertyName("attr_username")]     public string  AttrUsername    { get; set; } = "uid";
            [JsonPropertyName("attr_email")]        public string? AttrEmail       { get; set; }
            [JsonPropertyName("attr_groups")]       public string? AttrGroups      { get; set; }
            [JsonPropertyName("admin_group_value")] public string? AdminGroupValue { get; set; }
            [JsonPropertyName("nameid_format")]     public string  NameIdFormat    { get; set; } = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";
        }
    }
}
