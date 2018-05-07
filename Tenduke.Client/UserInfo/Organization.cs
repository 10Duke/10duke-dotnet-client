using System;
using System.Collections.Generic;
using System.Text;

namespace Tenduke.Client.UserInfo
{
    /// <summary>
    /// Object describing the OpenID Connect user info organization fields supported by the
    /// 10Duke Identity and Entitlement service (10Duke extension).
    /// </summary>
    public class Organization
    {
#pragma warning disable IDE1006 // Naming Styles
        public string id { get; set; }
        public string name { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
