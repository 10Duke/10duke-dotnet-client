﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tenduke.Client.WinFormsSample.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.7.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://localhost:48443/user/oauth20/authz")]
        public string AuthzUri {
            get {
                return ((string)(this["AuthzUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://localhost:48443/user/oauth20/token")]
        public string TokenUri {
            get {
                return ((string)(this["TokenUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://localhost:48443/userinfo/")]
        public string UserInfoUri {
            get {
                return ((string)(this["UserInfoUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SampleAppWinForms")]
        public string ClientID {
            get {
                return ((string)(this["ClientID"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ClientSecret {
            get {
                return ((string)(this["ClientSecret"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oob:SampleApp")]
        public string RedirectUri {
            get {
                return ((string)(this["RedirectUri"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("openid profile email")]
        public string Scope {
            get {
                return ((string)(this["Scope"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool StoreAuthorization {
            get {
                return ((bool)(this["StoreAuthorization"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA6iLsJRfYeGnqpu2MwURT
rkNtPZ+ItgXD7yCN6NRuX6G4DyrHI6amhXeSRTG7F2/OKmSrIJKkNtSt5+A/gFu9
zFH6OeUfkJqJZHYB8ZLH0cFRftcIf6VvXDkdVcp6hY0wOxO+pGWmQ8QKor55rcIY
fbnAYQBVG81VOHGkCgE0XvaEK+2nubJjLcHdC/t1cxMgsmTBwkJ4GfouIr4kewsG
i5zTQgbJ4XkyUSJF75ihv/GMLHAch/2YcpkTaXaM20+FEjvKj7B1hMj8x7ldDydi
94zVu7Sy3dGcd0dL15QESIh7Tm3ZwcxR1wtH0v8/ZwVZx1CNeWv51qFMMx8lv6KO
VwIDAQAB
-----END PUBLIC KEY-----
")]
        public string SignerKey {
            get {
                return ((string)(this["SignerKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowRememberMe {
            get {
                return ((bool)(this["ShowRememberMe"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AllowInsecureCerts {
            get {
                return ((bool)(this["AllowInsecureCerts"]));
            }
        }
    }
}
