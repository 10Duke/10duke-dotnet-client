using System;
using System.Collections.Generic;
using System.Text;

namespace Tenduke.Client.UserInfo
{
    /// <summary>
    /// Object describing the OpenID Connect user info fields supported by the
    /// 10Duke Identity and Entitlement service.
    /// </summary>
    public class UserInfoData
    {
        // OpenID Connect scope "profile"
#pragma warning disable IDE1006 // Naming Styles
        public string sub { get; set; }
        public string name { get; set; }
        public string nickname { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string gender { get; set; }
        public string birthdate { get; set; }
        public string website { get; set; }

        // OpenID Connect scope "email"
        public string email { get; set; }

        // OpenID Connect scope "address"
        public Address address { get; set; }

        // OpenID Connect scope "phone"
        public string phone_number { get; set; }

        // OpenID Connect scope "organization"
        public Organization organization { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
